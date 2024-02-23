using System.Collections.Generic;
using System.Linq;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace WorldGen
{
    [RequireComponent(typeof(NavMeshModifier))]
    [RequireComponent(typeof(NavMeshModifierTilemap))]
    [RequireComponent(typeof(Tilemap))]
    public class Room : MonoBehaviour
    {
        [HideInInspector] public List<Door> doors;
        [HideInInspector] public Tilemap Tilemap;

        [HideInInspector] public bool Entered;

        public Vector2Int MapCoord;

        public static readonly UnityEvent<Room> OnEnteredRoom = new();
        
        private void Awake()
        {
            doors = GetComponentsInChildren<Door>().ToList();
            foreach (var door in doors)
            {
                door.room = this;
            }

            Tilemap = GetComponent<Tilemap>();
        }
    }
}