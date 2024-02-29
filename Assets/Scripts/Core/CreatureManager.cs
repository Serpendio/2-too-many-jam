using System.Collections.Generic;
using Creature;
using UnityEngine;

namespace Core
{
    public class CreatureManager : MonoBehaviour
    {
        public List<CreatureBase> creatures = new();

        private void Awake()
        {
            Locator.ProvideCreatureManager(this);
        }

        public void AddCreature(CreatureBase creature) => creatures.Add(creature);
        public void RemoveCreature(CreatureBase creature) => creatures.Remove(creature);
    }
}
