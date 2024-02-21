using System.Collections.Generic;
using System.Linq;
using Tweens;
using UnityEngine;

namespace WorldGen
{
    public class WorldGenerator : MonoBehaviour
    {
        public static WorldGenerator Instance { get; private set; }
        
        [SerializeField] private CanvasGroup _fadeToBlack;

        [SerializeField] private List<Room> _roomPrefabs;

        public List<Room> WorldRooms = new();
        private Room _currentRoom;

        private void Awake()
        {
            Instance = this;
            
            Door.OnPlayerEnterDoor.AddListener((door, player) =>
            {
                if (door.GetLinkedDoor() != null) {
                    // go to already-generated room
                    GoThroughDoor(door, player);
                }
                else {
                    // make new room
                    var newRoom = GenerateNewRoom(door);
                    newRoom.gameObject.SetActive(false);
                    WorldRooms.Add(newRoom);
                    GoThroughDoor(door, player);
                }
            });

            _currentRoom = FindObjectsByType<Room>(FindObjectsSortMode.None).First();
            WorldRooms.Add(_currentRoom);
        }

        private Room GenerateNewRoom(Door comingFrom)
        {
            var room = _roomPrefabs[Random.Range(0, _roomPrefabs.Count)];
            var roomObj = Instantiate(room, transform);

            roomObj.MapCoord = comingFrom.GetLinkedMapCoord();

            // temp
            roomObj.GetComponent<Renderer>().material.color =
                new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            return roomObj;
        }

        private void GoThroughDoor(Door door, PlayerTemp player)
        {
            Time.timeScale = 0;

            var toBlackTween = new FloatTween()
            {
                from = 0f,
                to = 1f,
                duration = 0.25f,
                easeType = EaseType.CubicIn,
                useUnscaledTime = true,
                onUpdate = (_, val) => _fadeToBlack.alpha = val
            };

            var fromBlackTween = new FloatTween()
            {
                from = 1f,
                to = 0f,
                duration = 0.25f,
                easeType = EaseType.CubicOut,
                useUnscaledTime = true,
                onUpdate = (_, val) => _fadeToBlack.alpha = val
            };

            fromBlackTween.onEnd = _ => Time.timeScale = 1;

            toBlackTween.onEnd = _ =>
            {
                _currentRoom.gameObject.SetActive(false);
                _currentRoom = door.GetLinkedDoor().room;
                _currentRoom.gameObject.SetActive(true);

                player.transform.position = door.GetLinkedDoor().transform.position + door.GetLinkedDoor().direction switch
                {
                    Direction.North => -Vector3.up,
                    Direction.East => -Vector3.right,
                    Direction.South => Vector3.up,
                    Direction.West => Vector3.right,
                    _ => Vector3.zero
                } * 1.5f;

                _fadeToBlack.gameObject.AddTween(fromBlackTween);
            };

            _fadeToBlack.gameObject.AddTween(toBlackTween);
        }
    }
}