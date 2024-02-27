using System;
using System.Collections.Generic;
using System.Linq;
using Creature;
using Random = UnityEngine.Random;

namespace Core
{
    [Serializable]
    public class WeightedSpawnPool
    {
        [Serializable]
        public struct WeightedItem
        {
            public int weight;
            public EnemyBase item;
        }
        
        public List<WeightedItem> Items = new();

        public EnemyBase GetRandom()
        {
            var totalWeight = Items.Sum(i => i.weight);
            var random = Random.Range(0, totalWeight);
            var currentWeight = 0;

            foreach (var item in Items)
            {
                currentWeight += item.weight;
                if (random < currentWeight) return item.item;
            }

            return default;
        }
    }
}