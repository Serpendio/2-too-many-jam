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

        [HideInInspector] public bool Entered;

        public Vector2Int MapCoord;

        public static readonly UnityEvent<Room> OnEnteredRoom = new();

        private List<EnemyBase> _enemies = new();

        private GameObject _deathSkullPrefab;

        [HideInInspector] public Transform SkullsContainer;
        [HideInInspector] public Transform EnemiesContainer;
        [HideInInspector] public Transform ChestsContainer;

        public bool SpawnEnemiesOnEnter = true;

        private Tile[] lampTiles;

        private Vector3[] posOffsets;
        private int[] rotationOffsets;

        [SerializeField] public GameObject chestPrefab;

        private void Awake()
        {
            doors = GetComponentsInChildren<Door>().ToList();
            foreach (var door in doors)
            {
                door.room = this;

                if (SpawnEnemiesOnEnter) door.gameObject.SetActive(false);
            }

            Tilemap = GetComponent<Tilemap>();
            _deathSkullPrefab = Resources.Load<GameObject>("Prefabs/GroundSkull");

            SkullsContainer = new GameObject("Skulls").transform;
            SkullsContainer.parent = transform;

            EnemiesContainer = new GameObject("Enemies").transform;
            EnemiesContainer.parent = transform;
            
            ChestsContainer = new GameObject("Chests").transform;
            ChestsContainer.parent = transform;


            lampTiles = new Tile[] {Resources.Load<Tile>("Tiles/wall_tile_lamp_top"),
                                    Resources.Load<Tile>("Tiles/wall_tile_lamp_left"),
                                    Resources.Load<Tile>("Tiles/wall_tile_lamp_right"),
                                    Resources.Load<Tile>("Tiles/wall_tile_lamp_bottom")};

            posOffsets = new Vector3[] {Vector3.down, Vector3.right, Vector3.left, Vector3.up};
            rotationOffsets = new int[] {0, 90, 270, 180};


            // Iterate through tiles, if tile is wall, random chance to add lamp tile on higher level
            var tilemapBounds = Tilemap.cellBounds;
            var createdLamps = new List<Vector3Int>();

            int spawnedChests = 0;
            int maximumPossibleChests = 2;

            for (var x = tilemapBounds.xMin; x < tilemapBounds.xMax; x++)
            {
                for (var y = tilemapBounds.yMin; y < tilemapBounds.yMax; y++)
                {
                    var pos = new Vector3Int(x, y, 1);
                    bool spawnLamp = Random.value > 0.9f && !createdLamps.Any(l => Vector3.Distance(l, pos) < 4);


                    bool spawnChest = Random.value > .9f && spawnedChests < maximumPossibleChests;
                    
                    if (!spawnLamp && !spawnChest) continue;

                    var baseTile = Tilemap.GetTile(new Vector3Int(x, y, 0));
                    if (baseTile == null) continue;

                    var sides = new TileBase[4];
                    sides[0] = Tilemap.GetTile(new Vector3Int(x, y + 1, 0));
                    sides[1] = Tilemap.GetTile(new Vector3Int(x - 1, y, 0));
                    sides[2] = Tilemap.GetTile(new Vector3Int(x + 1, y, 0));
                    sides[3] = Tilemap.GetTile(new Vector3Int(x, y - 1, 0));

                    // prevent corners
                    if (sides[0] == null && sides[1] == null) continue;
                    if (sides[0] == null && sides[2] == null) continue;
                    if (sides[3] == null && sides[1] == null) continue;
                    if (sides[3] == null && sides[2] == null) continue;

                    //Loop through all sides
                    for (int i=0; i<4; i++) {
                        if (sides[i] == null) {
                            if (spawnLamp) {
                                Tilemap.SetTile(pos, lampTiles[i]);
                                createdLamps.Add(pos);
                            }
                            //Check that there isn't already a chest or door at potential spot - it makes sense i swear! :-]
                            bool freeSpot = Core.Locator.CreatureManager.creatures.Where(c => c is Chest && c.transform.position == new Vector3(x + 0.5f, y + 0.5f, 1) + posOffsets[i]).Count() == 0 && doors.Where(d => posOffsets.ToList().ConvertAll(o => o += new Vector3(x + 0.5f, y + 0.5f, 0) + posOffsets[i]).Contains(d.transform.position)).Count() == 0;
                            if (spawnChest && freeSpot) {
                                GameObject chest = Instantiate(chestPrefab, new Vector3(x + 0.5f, y + 0.5f, 1) + posOffsets[i], Quaternion.Euler(0, 0, rotationOffsets[i]));
                                chest.transform.parent = ChestsContainer;
                                Core.Locator.CreatureManager.creatures.Add(chest.GetComponent<Chest>());
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
            Locator.CreatureManager.creatures.Remove(enemy);
            _enemies.Remove(enemy);
            if (_enemies.Count == 0)
            {
                foreach (var door in doors)
                {
                    door.gameObject.SetActive(true);
                }
            }
        }
    }
}