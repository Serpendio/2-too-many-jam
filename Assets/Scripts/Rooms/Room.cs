using System.Collections.Generic;
using System.Linq;
using Core;
using Creature;
using NavMeshPlus.Components;
using Tweens;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace Rooms
{
    [RequireComponent(typeof(NavMeshModifier))]
    [RequireComponent(typeof(NavMeshModifierTilemap))]
    [RequireComponent(typeof(Tilemap))]
    public class Room : MonoBehaviour
    {
        [HideInInspector] public List<Door> doors;
        [HideInInspector] public Tilemap Tilemap;
        [SerializeField] private GameObject glow;

        [HideInInspector] public bool Entered;

        public Vector2Int MapCoord;

        public static readonly UnityEvent<Room> OnEnteredRoom = new();

        private List<EnemyBase> _enemies = new();

        private static GameObject _deathSkullPrefab;
        private static Chest _chestPrefab;

        [HideInInspector] public Transform SkullsContainer;
        [HideInInspector] public Transform EnemiesContainer;
        [HideInInspector] public Transform ChestsContainer;
        [HideInInspector] public Transform glowContainer;

        public bool SpawnEnemiesOnEnter = true;

        private Tile[] lampTiles;

        private Vector3[] posOffsets;
        private int[] rotationOffsets;
        
        private void Awake()
        {
            doors = GetComponentsInChildren<Door>().ToList();
            foreach (var door in doors)
            {
                door.room = this;

                if (SpawnEnemiesOnEnter || GetComponent<BossRoom>() != null) door.gameObject.SetActive(false);
            }

            Tilemap = GetComponent<Tilemap>();
            
            if (_deathSkullPrefab == null) _deathSkullPrefab = Resources.Load<GameObject>("Prefabs/GroundSkull");
            if (_chestPrefab == null) _chestPrefab = Resources.Load<Chest>("Prefabs/Chest");

            SkullsContainer = new GameObject("Skulls").transform;
            SkullsContainer.parent = transform;

            EnemiesContainer = new GameObject("Enemies").transform;
            EnemiesContainer.parent = transform;
            
            ChestsContainer = new GameObject("Chests").transform;
            ChestsContainer.parent = transform;

            glowContainer = new GameObject("Glow").transform;
            glowContainer.parent = transform;

            lampTiles = new[] {Resources.Load<Tile>("Tiles/wall_tile_lamp_top"),
                                    Resources.Load<Tile>("Tiles/wall_tile_lamp_left"),
                                    Resources.Load<Tile>("Tiles/wall_tile_lamp_right"),
                                    Resources.Load<Tile>("Tiles/wall_tile_lamp_bottom")};

            posOffsets = new[] {Vector3.down, Vector3.right, Vector3.left, Vector3.up};
            rotationOffsets = new[] {0, 90, 270, 180};


            // Iterate through tiles, if tile is wall, random chance to add lamp tile on higher level
            var tilemapBounds = Tilemap.cellBounds;
            var createdLamps = new List<Vector3Int>();

            int spawnedChests = 0;
            int maximumPossibleChests = GenerateSpawnablePositions().Count/175;

            for (var x = tilemapBounds.xMin; x < tilemapBounds.xMax; x++)
            {
                for (var y = tilemapBounds.yMin; y < tilemapBounds.yMax; y++)
                {
                    var pos = new Vector3Int(x, y, 1);
                    bool spawnLamp = Random.value > 0.9f && !createdLamps.Any(l => Vector3.Distance(l, pos) < 4);
                    
                    bool spawnChest = Random.value > 0.9f && spawnedChests < maximumPossibleChests;
                    
                    if (!spawnLamp && !spawnChest) continue;

                    var baseTile = Tilemap.GetTile(new Vector3Int(x, y, 0));
                    if (baseTile == null) continue;

                    var sides = new TileBase[4];
                    sides[0] = Tilemap.GetTile(new Vector3Int(x, y + 1, 0)); // top
                    sides[1] = Tilemap.GetTile(new Vector3Int(x - 1, y, 0)); // left
                    sides[2] = Tilemap.GetTile(new Vector3Int(x + 1, y, 0)); // right
                    sides[3] = Tilemap.GetTile(new Vector3Int(x, y - 1, 0)); // bottom

                    // prevent corners
                    if (sides[0] == null && sides[1] == null) continue;
                    if (sides[0] == null && sides[2] == null) continue;
                    if (sides[3] == null && sides[1] == null) continue;
                    if (sides[3] == null && sides[2] == null) continue;

                    //Loop through all sides
                    for (int i=0; i<4; i++) {
                        if (sides[i] == null) {
                            if (spawnLamp && Tilemap.GetColliderType(pos - new Vector3Int(0, 0, 1) + Vector3Int.FloorToInt(posOffsets[i])) == Tile.ColliderType.None) {
                                Tilemap.SetTile(pos, lampTiles[i]);
                                createdLamps.Add(pos);
                                Instantiate(glow, new Vector3(x + 0.5f, y + 0.5f, 0f), Quaternion.Euler(0, 0, 180 + rotationOffsets[i]), glowContainer);
                            }
                            
                            //Check that there isn't already a chest, door, or wall at potential spot - it makes sense i swear! :-]
                            bool freeSpot = Locator.CreatureManager.creatures.Where(c => c is Chest && c.isActiveAndEnabled && c.transform.position == new Vector3(x + 0.5f, y + 0.5f, 1) + posOffsets[i]).Count() == 0 && doors.Where(d => posOffsets.ToList().ConvertAll(o => o += new Vector3(x + 0.5f, y + 0.5f, 0) + posOffsets[i]).Contains(d.transform.position)).Count() == 0 && !Physics2D.OverlapCircle(new Vector3(x + 0.5f, y + 0.5f, 1) + posOffsets[i], 0.1f);
                            if (spawnChest && freeSpot) {
                                Chest chest = Instantiate(_chestPrefab, new Vector3(x + 0.5f, y + 0.5f, 0f) + posOffsets[i], Quaternion.Euler(0, 0, rotationOffsets[i]));
                                chest.transform.parent = ChestsContainer;
                                spawnedChests += 1;
                            }
                        }
                    }
                }
            }
        }

        public void RegisterEnemy(EnemyBase enemy)
        {
            Locator.CreatureManager.AddCreature(enemy);
            _enemies.Add(enemy);
            enemy.OnDeath.AddListener(() =>
            {
                //Give player some XP based on enemy's type
                Locator.LevelManager.addXP(enemy.getXPDropAmount());

                var skull = Instantiate(_deathSkullPrefab, enemy.transform.position, Quaternion.identity);
                skull.transform.parent = SkullsContainer;
                skull.AddTween(new SpriteRendererColorTween
                {
                    from = Color.white,
                    to = Color.clear,
                    duration = 30,
                    easeType = EaseType.Linear,
                    onEnd = _ => Destroy(skull)
                });

                UnregisterEnemy(enemy);
            });
        }

        public void UnregisterEnemy(EnemyBase enemy)
        {
            Locator.CreatureManager.RemoveCreature(enemy);
            _enemies.Remove(enemy);
            if (_enemies.Count == 0)
            {
                foreach (var door in doors)
                {
                    door.gameObject.SetActive(true);
                }
            }
        }


        public List<Vector3> GenerateSpawnablePositions()
        {
            var bounds = Tilemap.cellBounds;

            var spawnablePositions = new List<Vector3>();
            // iterate through bounds, if tile is not null and collider is none add to vector3 list
            for (var x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (var y = bounds.yMin; y < bounds.yMax; y++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    if (Tilemap.GetTile(pos) == null) continue;
                    if (Tilemap.GetColliderType(pos) != Tile.ColliderType.None) continue;
                    if (Vector3.Distance(pos, Core.Locator.Player.transform.position) < 5) continue;

                    spawnablePositions.Add(Tilemap.GetCellCenterWorld(pos));
                }
            }

            return spawnablePositions;
        }
    }
}