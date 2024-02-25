using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class CreatureManager : MonoBehaviour
    {

        public List<Creature.CreatureBase> creatures;

        private void Awake()
        {
            Locator.ProvideCreatureManager(this);
        }

        public void AddCreature(Creature.CreatureBase creature) => creatures.Add(creature);
    }
}
