using System;
using Spells.Modifiers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    public class SpellMaster : MonoBehaviour
    {
        // doesn't need to be singleton until we can access from inspector, but is prepared for it
        public static SpellMaster Instance;
        [SerializeField] Type type;

        List<Type> modifierTypes = new List<Type>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                // Add the types of the child classes to the list
                modifierTypes.Add(typeof(SineModifier));
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            // Add the types of the child classes to the list
            modifierTypes.Add(typeof(SineModifier));
        }

        public SpellModifier GetModifier(ModifierTier tier) // could be infinite
        {
            while (true)
            {
                int index = UnityEngine.Random.Range(0, modifierTypes.Count);
                // ugh, but I can't find a way around it without static abstract fields existing
                SpellModifier modifier = Activator.CreateInstance(modifierTypes[index]) as SpellModifier;
                if (modifier.Tier == tier)
                {
                    return modifier;
                }
            }
        }
    }
}
