using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace WorldGen
{
    public class Room : MonoBehaviour
    {
        [HideInInspector] public List<Door> doors;

        public Vector2Int MapCoord;

        public static readonly UnityEvent<Room> OnEnteredRoom = new();

        private void Awake()
        {
            doors = GetComponentsInChildren<Door>().ToList();
            foreach (var door in doors)
            {
                door.room = this;
            }
        }
    }
}
