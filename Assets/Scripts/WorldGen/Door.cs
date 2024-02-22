using System.Linq;
using Creature;
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

        public static readonly UnityEvent<Door, Player> OnPlayerEnterDoor = new();
        public static Direction GetOpposite(Direction dir) => (Direction)(((int)dir + 2) % 4);

        [HideInInspector] public Room room;

        public Vector2Int GetLinkedMapCoord()
        {
            return room.MapCoord + direction switch
            {
                Direction.North => Vector2Int.up,
                Direction.East => Vector2Int.right,
                Direction.South => Vector2Int.down,
                Direction.West => Vector2Int.left,
                _ => Vector2Int.zero
            };
        }
        
        public Door GetLinkedDoor()
        {
            var targetCoord = GetLinkedMapCoord();
            var targetRoom = WorldGenerator.Instance.WorldRooms.FirstOrDefault(r => r.MapCoord == targetCoord);
            return targetRoom == null ? null : targetRoom.doors.FirstOrDefault(d => d.direction == GetOpposite(direction));
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                OnPlayerEnterDoor.Invoke(this, other.GetComponent<Player>());
                player.Rb.velocity = Vector2.zero;
            }
        }
    }
}