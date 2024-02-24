using System.Collections.Generic;
using UnityEngine;

namespace Rooms
{
    public class Shop : MonoBehaviour 
    {

        public GameObject[] spellIconPrefabs;
        private Vector3[] spellLocations;

        void Start() {
            spellLocations = new[] { new Vector3(4.5f, 8.5f, 0f), new Vector3(9.5f, 8.5f, 0f), new Vector3(14.5f, 8.5f, 0f) };

            //Generate 3 random and unique spells
            List<int> chosenSpellIndices = new List<int>();
            for (int i = 0; i < 3; i++) {
                //Get random spells until unique one found
                int randIndex;
                do {
                    randIndex = Random.Range(0, spellIconPrefabs.Length);
                } while (chosenSpellIndices.Contains(randIndex));

                //Instantiate spell
                GameObject spell = Instantiate(spellIconPrefabs[randIndex]);
                spell.transform.parent = this.transform;
                chosenSpellIndices.Add(randIndex);
                spell.transform.position = spellLocations[i];

            }
        }
    }
}