using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour 
{

    public GameObject[] spellIconPrefabs;
    private Vector3[] spellLocations;

    void Start() {
        spellLocations = new[] { new Vector3(-6f, 1f, 0f), new Vector3(-1f, 1f, 0f), new Vector3(4, 1f, 0f) };

        //Generate 3 random and unique spells
        List<int> chosenSpellIndices = new List<int>();
        for (int i = 0; i < 3; i++) {
            //Get random spells until unique one found
            int randIndex;
            do
            {
                randIndex = Random.Range(0, spellIconPrefabs.Length);
            } while (chosenSpellIndices.Contains(randIndex));

            //Instantiate spell
            GameObject spell = Instantiate(spellIconPrefabs[randIndex]);
            chosenSpellIndices.Add(randIndex);
            spell.transform.position = spellLocations[i];

        }

    }

}