using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;


namespace Core
{
    public class LevelManager : MonoBehaviour
    {
        private int currentLevel;
        private int currentXP; //Gets reset to 0 when levelling up
        private int xpToLevelUp;

        private int maxLevel;

        private int maxHealthIncreasePerLevelUp;
        private int maxManaIncreasePerLevelUp;

        [HideInInspector] public UnityEvent PlayerLevelUp = new();

        private void Awake() {

            Locator.ProvideLevelManager(this);

            currentLevel = 1;
            currentXP = 0;
            xpToLevelUp = 20;

            maxLevel = 30;
        }

        private void Start() {
            PlayerLevelUp.AddListener(() =>
            {
                currentLevel += 1;
                currentXP = 0;
                xpToLevelUp += currentLevel * 10;
            });
        }

        private void Update() {
            if (currentXP >= xpToLevelUp) {
                PlayerLevelUp.Invoke();
            }
        }

        public int getCurrentLevel() {
            return currentLevel;
        }

        public int getXPToLevelUp() {
            return xpToLevelUp;
        }

        public void addXP(int val) {
            currentXP += val;
        }

        public int getMaxHealthIncreasePerLevelUp() {
            return maxHealthIncreasePerLevelUp;
        }

        public int getMaxManaIncreasePerLevelUp() {
            return maxManaIncreasePerLevelUp;
        }

        public int getMaxLevel() {
            return maxLevel;
        }
    }

}