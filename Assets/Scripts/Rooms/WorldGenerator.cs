using System.Collections.Generic;
using System.Linq;
using Core;
using Creature;
using NavMeshPlus.Components;
using Tweens;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rooms
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _fadeToBlack;
        [SerializeField] private NavMeshSurface _navMeshSurface;

        [SerializeField] private int nextRoom;
        [SerializeField] private List<Room> _roomPrefabs;
        [SerializeField] private Room bossRoomPrefab;
        [SerializeField] private Room _shopPrefab;

        [SerializeField] private WeightedSpawnPool _weightedSpawnPool = new();

        public List<Room> WorldRooms = new();
        private Room _currentRoom;

        [SerializeField] [Range(0, 1)] private float hueVariety;

        [SerializeField] private Player _player;

        public bool QueueBossRoom;

        // arbitrary, up for balance discussion
        [SerializeField] private int _shopMinRoomGap = 2;
        [SerializeField] private int _shopMaxRoomGap = 6;
        [SerializeField] [Range(0, 1)] private float _shopGenChance = 0.2f;
        private int _roomsSinceShop;

        private void Awake()
        {
            Locator.ProvideWorldGenerator(this);

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

            GoThroughDoor(_currentRoom.doors[Random.Range(0, _currentRoom.doors.Count)], true, true);

            Locator.LevelManager.OnPlayerLevelUp.AddListener(level =>
            {
                if (level == Locator.StageManager.LevelsPerStage * Locator.StageManager.Stage) QueueBossRoom = true;
            });
        }

        private Room GenerateNewRoom(Door comingFrom = null)
        {
            Room room;
            if (QueueBossRoom)
            {
                room = bossRoomPrefab;
                QueueBossRoom = false;

                _roomsSinceShop = _shopMaxRoomGap; // give em a treat :3
            }
            else if (_roomsSinceShop >= _shopMaxRoomGap)
            {
                room = _shopPrefab;
                _roomsSinceShop = 0;
            }
            else if (_roomsSinceShop >= _shopMinRoomGap && Random.value <= _shopGenChance)
            {
                room = _shopPrefab;
                _roomsSinceShop = 0;
            }
            else
            {
                room = nextRoom > _roomPrefabs.Count - 1
                    ? _roomPrefabs[Random.Range(0, _roomPrefabs.Count)]
                    : _roomPrefabs[nextRoom];
                
                _roomsSinceShop++;
            }
            
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

        private void GoThroughDoor(Door door, bool preLinkedDoor = false, bool instant = false)
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

                    Core.Locator.Player.SetHealth(Core.Locator.Player.health +
                                                  Core.Locator.Player.maxHealth /
                                                  10); //Increase player's health by 10% of maxHealth

                    if (_currentRoom.SpawnEnemiesOnEnter)
                    {
                        // Generate enemies on random tiles in the room, not too close to player
                        var spawnablePositions = _currentRoom.GenerateSpawnablePositions();
                        float lnlvl = Mathf.Log(Core.Locator.LevelManager.getCurrentLevel());
                        int enemiesToSpawn = (int)(((lnlvl + 2) * (lnlvl + 2)) / 2); //f(x) = ((ln(x)+2)^2)/2
                        enemiesToSpawn = Mathf.Clamp(enemiesToSpawn, 2, spawnablePositions.Count / 3); //Don't let more than a third of spawnable positions have enemies

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

                if (instant) fromBlackTween.onEnd.Invoke(null);
                else _fadeToBlack.gameObject.AddTween(fromBlackTween);
            };

            if (instant) toBlackTween.onEnd.Invoke(null);
            else _fadeToBlack.gameObject.AddTween(toBlackTween);
        }
    }
}