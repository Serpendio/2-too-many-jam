using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WorldGen
{
    public class Room : MonoBehaviour
    {
        public List<Door> doors;

        public Vector2Int MapCoord;

        public static readonly UnityEvent<Room> OnEnteredRoom = new();

        private void Awake()
        {
            foreach (var door in doors)
            {
                door.room = this;
            }
        }
    }
}
