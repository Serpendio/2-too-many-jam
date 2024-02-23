using System.Collections.Generic;
using System.Linq;
using Creature;
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

        private List<EnemyBase> _enemies = new();
        
        private void Awake()
        {
            doors = GetComponentsInChildren<Door>().ToList();
            foreach (var door in doors)
            {
                door.room = this;
                door.gameObject.SetActive(false);
            }

            Tilemap = GetComponent<Tilemap>();
        }
        
        public void RegisterEnemy(EnemyBase enemy)
        {
            _enemies.Add(enemy);
            enemy.OnDeath.AddListener(() => UnregisterEnemy(enemy));
        }
        
        public void UnregisterEnemy(EnemyBase enemy)
        {
            _enemies.Remove(enemy);
            if (_enemies.Count == 0)
            {
                foreach (var door in doors)
                {
                    door.gameObject.SetActive(true);
                }
            }
        }
    }
}