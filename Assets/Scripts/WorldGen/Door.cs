using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace WorldGen
{
    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public class Door : MonoBehaviour
    {
        public Direction direction = Direction.North;

        public static readonly UnityEvent<Door, PlayerTemp> OnPlayerEnterDoor = new();
        public static Direction GetOpposite(Direction dir) => (Direction)(((int)dir + 2) % 4);

        [HideInInspector] public Room room;
        
        public Door GetLinkedDoor()
        {
            var currentCoord = room.MapCoord;
            var targetCoord = currentCoord + direction switch
            {
                Direction.North => Vector2Int.up,
                Direction.East => Vector2Int.right,
                Direction.South => Vector2Int.down,
                Direction.West => Vector2Int.left,
                _ => Vector2Int.zero
            };
            
            var targetRoom = WorldGenerator.Instance.WorldRooms.FirstOrDefault(r => r.MapCoord == targetCoord);
            if (targetRoom == null) return null;
            
            return targetRoom.doors.FirstOrDefault(d => d.direction == GetOpposite(direction));
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // if (Time.timeScale == 0f) return;
                OnPlayerEnterDoor.Invoke(this, other.GetComponent<PlayerTemp>());
            }
        }
    }
}