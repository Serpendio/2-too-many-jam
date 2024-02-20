using System.Collections.Generic;
using UnityEngine;

namespace WorldGen
{
    public class Room : MonoBehaviour
    {
        public List<Door> doors;

        public Vector2Int MapCoord;

        private void Awake()
        {
            foreach (var door in doors)
            {
                door.room = this;
            }
        }
    }
}
