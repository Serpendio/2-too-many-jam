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

        [HideInInspector] public Door linkedDoor;

        [HideInInspector] public Room room;

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