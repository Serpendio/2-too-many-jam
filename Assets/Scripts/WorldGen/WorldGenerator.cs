using System.Collections.Generic;
using System.Linq;
using Tweens;
using UnityEngine;

namespace WorldGen
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _fadeToBlack;

        [SerializeField] private List<Room> _roomPrefabs;

        private List<Room> _generatedRooms = new();
        private Room _currentRoom;

        private void Awake()
        {
            Door.OnPlayerEnterDoor.AddListener((door, player) =>
            {
                if (door.linkedDoor != null)
                {
                    // go to already-generated room
                    GoThroughDoor(door, player);
                }
                else
                {
                    // make new room
                    var newRoom = GenerateNewRoom(door);
                    _generatedRooms.Add(newRoom);
                    GoThroughDoor(door, player);
                }
            });

            _currentRoom = FindObjectsByType<Room>(FindObjectsSortMode.None).First();
        }

        private Room GenerateNewRoom(Door comingFrom)
        {
            // rooms where the room has at least one door with the opposite direction that the player is coming from
            var filteredRooms = _roomPrefabs
                .Where(room => room.doors.Count(d => d.direction == Door.GetOpposite(comingFrom.direction)) > 0)
                .ToArray();

            var room = filteredRooms[Random.Range(0, filteredRooms.Length)];
            var roomObj = Instantiate(room, transform);

            var entranceDoor = roomObj.GetComponent<Room>().doors
                .First(d => d.direction == Door.GetOpposite(comingFrom.direction));

            comingFrom.linkedDoor = entranceDoor;
            entranceDoor.linkedDoor = comingFrom;

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
                _currentRoom = door.linkedDoor.room;
                _currentRoom.gameObject.SetActive(true);

                player.transform.position = door.linkedDoor.transform.position + door.linkedDoor.direction switch
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