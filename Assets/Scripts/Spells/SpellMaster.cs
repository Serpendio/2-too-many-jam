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

        static Type[] modifierTypes = new[] { typeof(SineModifier) };

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public SpellModifier GetModifier(ModifierTier tier, Element element) // could be infinite
        {
            while (true)
            {
                int index = UnityEngine.Random.Range(0, modifierTypes.Length);
                // ugh, but I can't find a way around it without static abstract fields existing
                SpellModifier modifier = Activator.CreateInstance(modifierTypes[index]) as SpellModifier;
                if (modifier.Tier == tier && modifier.Element == element)
                {
                    return modifier;
                }
            }
        }
    }
}
