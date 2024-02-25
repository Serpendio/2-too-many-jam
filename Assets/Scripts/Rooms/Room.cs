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

        public bool SpawnEnemiesOnEnter = true;

        private Tile _lampTopTile;
        private Tile _lampLeftTile;
        private Tile _lampRightTile;
        private Tile _lampBottomTile;

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

            _lampTopTile = Resources.Load<Tile>("Tiles/wall_tile_lamp_top");
            _lampLeftTile = Resources.Load<Tile>("Tiles/wall_tile_lamp_left");
            _lampRightTile = Resources.Load<Tile>("Tiles/wall_tile_lamp_right");
            _lampBottomTile = Resources.Load<Tile>("Tiles/wall_tile_lamp_bottom");

            // Iterate through tiles, if tile is wall, random chance to add lamp tile on higher level
            var tilemapBounds = Tilemap.cellBounds;
            var createdLamps = new List<Vector3Int>();
            for (var x = tilemapBounds.xMin; x < tilemapBounds.xMax; x++)
            {
                for (var y = tilemapBounds.yMin; y < tilemapBounds.yMax; y++)
                {
                    var pos = new Vector3Int(x, y, 1);
                    if (Random.value < 0.9f || createdLamps.Any(l => Vector3.Distance(l, pos) < 4)) continue;

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

                    if (sides[0] == null)
                    {
                        Tilemap.SetTile(pos, _lampTopTile);
                        createdLamps.Add(pos);
                    }

                    if (sides[3] == null)
                    {
                        Tilemap.SetTile(pos, _lampBottomTile);
                        createdLamps.Add(pos);
                    }

                    if (sides[1] == null)
                    {
                        Tilemap.SetTile(pos, _lampLeftTile);
                        createdLamps.Add(pos);
                    }

                    if (sides[2] == null)
                    {
                        Tilemap.SetTile(pos, _lampRightTile);
                        createdLamps.Add(pos);
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