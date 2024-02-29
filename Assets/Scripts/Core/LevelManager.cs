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

            currentLevel = 10;
            currentXP = 0;
            xpToLevelUp = 20;

            maxLevel = 30;
        }

        public int getCurrentLevel() {
            return currentLevel;
        }

        public int getXPToLevelUp() {
            return xpToLevelUp;
        }

        public void addXP(int val) {
            currentXP += val;
            //While-loop in case player levels up more than once in same frame
            while (currentXP > xpToLevelUp)
            {
                currentXP -= xpToLevelUp;
                currentLevel += 1;
                xpToLevelUp += currentLevel * 10;
            }
            PlayerLevelUp.Invoke();
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
