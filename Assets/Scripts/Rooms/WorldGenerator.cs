using System.Collections.Generic;
using Core;
using Creature;
using NavMeshPlus.Components;
using Tweens;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Rooms
{
    public class WorldGenerator : MonoBehaviour
    {
        public static WorldGenerator Instance { get; private set; }

        [SerializeField] private CanvasGroup _fadeToBlack;
        [SerializeField] private NavMeshSurface _navMeshSurface;

        [SerializeField] private List<Room> _roomPrefabs;
        [SerializeField] private int nextRoom;

        [SerializeField] private WeightedSpawnPool _weightedSpawnPool = new();

        public List<Room> WorldRooms = new();
        private Room _currentRoom;

        [SerializeField] [Range(0, 1)] private float hueVariety;

        [SerializeField] private Player _player;

        private void Awake()
        {
            Instance = this;

            _navMeshSurface.hideEditorLogs = true;

            Door.OnPlayerEnterDoor.AddListener((door, _) =>
            {
                if (door.GetLinkedDoor() != null)
                {
                    // go to already-generated room
                    GoThroughDoor(door);
                }
                else
                {
                    // make new room
                    var newRoom = GenerateNewRoom(door);
                    newRoom.gameObject.SetActive(false);
                    WorldRooms.Add(newRoom);
                    GoThroughDoor(door);
                }
            });
        }

        private void Start()
        {
            _currentRoom = GenerateNewRoom();
            WorldRooms.Add(_currentRoom);

            GoThroughDoor(_currentRoom.doors[Random.Range(0, _currentRoom.doors.Count)], true);
        }

        private Room GenerateNewRoom(Door comingFrom = null)
        {
            var room = nextRoom > _roomPrefabs.Count - 1
                ? _roomPrefabs[Random.Range(0, _roomPrefabs.Count)]
                : _roomPrefabs[nextRoom];

            var roomObj = Instantiate(room, transform);

            roomObj.MapCoord = comingFrom != null
                ? comingFrom.GetLinkedMapCoord()
                : new Vector2Int(0, 0);

            // todo: Randomise hue, removed until we have a better way to handle this
            // by default unity just multiplies the whole sprite with the new colour 
            // which looks kinda ugly sometimes and messes with the s+v :/
            // maybe some shader nonsense?

            // float randHueOffset = Random.Range(0, hueVariety);
            // foreach (Renderer renderer in roomObj.GetComponentsInChildren<Renderer>()) {
            //     Color.RGBToHSV(renderer.material.color, out var h, out _, out _);
            //     renderer.material.color = Color.HSVToRGB(h + randHueOffset, 0.5f, 1f);
            // }

            return roomObj;
        }

        private void GoThroughDoor(Door door, bool preLinkedDoor = false)
        {
            Time.timeScale = 0;

            var toBlackTween = new FloatTween
            {
                from = 0f,
                to = 1f,
                duration = 0.25f,
                easeType = EaseType.CubicIn,
                useUnscaledTime = true,
                onUpdate = (_, val) => _fadeToBlack.alpha = val
            };

            var fromBlackTween = new FloatTween
            {
                from = 1f,
                to = 0f,
                duration = 0.25f,
                easeType = EaseType.CubicOut,
                useUnscaledTime = true,
                onUpdate = (_, val) => _fadeToBlack.alpha = val,
                onEnd = _ => Time.timeScale = 1
            };

            var linkedDoor = preLinkedDoor ? door : door.GetLinkedDoor();

            toBlackTween.onEnd = _ =>
            {
                _currentRoom.gameObject.SetActive(false);
                _currentRoom = linkedDoor.room;
                _currentRoom.gameObject.SetActive(true); 

                _player.transform.position = linkedDoor.transform.position + linkedDoor.direction switch
                {
                    Direction.North => -Vector3.up,
                    Direction.East => -Vector3.right,
                    Direction.South => Vector3.up,
                    Direction.West => Vector3.right,
                    _ => Vector3.zero
                } * 1.5f;

                _navMeshSurface.BuildNavMesh();

                if (!_currentRoom.Entered)
                {
                    _currentRoom.Entered = true;
                 
                    if (_currentRoom.SpawnEnemiesOnEnter)
                    {
                        // Generate enemies on random tiles in the room, not too close to player
                        var bounds = _currentRoom.Tilemap.cellBounds;

                        var spawnablePositions = new List<Vector3>();
                        // iterate through bounds, if tile is not null and collider is none add to vector3 list
                        for (var x = bounds.xMin; x < bounds.xMax; x++)
                        {
                            for (var y = bounds.yMin; y < bounds.yMax; y++)
                            {
                                var pos = new Vector3Int(x, y, 0);
                                if (_currentRoom.Tilemap.GetTile(pos) == null) continue;
                                if (_currentRoom.Tilemap.GetColliderType(pos) != Tile.ColliderType.None) continue;
                                if (Vector3.Distance(pos, _player.transform.position) < 5) continue;

                                spawnablePositions.Add(_currentRoom.Tilemap.GetCellCenterWorld(pos));
                            }
                        }

                        // some arbitrary amount based on room size
                        var enemiesToSpawn = 2 + (spawnablePositions.Count / 30);

                        for (var i = 0; i < enemiesToSpawn; i++)
                        {
                            var creaturePrefab = _weightedSpawnPool.GetRandom();

                            var enemy = Instantiate(creaturePrefab, _currentRoom.EnemiesContainer);
                            var randomPos = spawnablePositions[Random.Range(0, spawnablePositions.Count)];

                            enemy.transform.position = randomPos;
                            enemy.target = _player;

                            _currentRoom.RegisterEnemy(enemy);
                        }
                    }
                }

                Room.OnEnteredRoom.Invoke(_currentRoom);

                _fadeToBlack.gameObject.AddTween(fromBlackTween);
            };

            _fadeToBlack.gameObject.AddTween(toBlackTween);
        }
    }
}