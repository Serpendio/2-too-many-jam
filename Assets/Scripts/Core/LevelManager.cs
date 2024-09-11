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

        [HideInInspector] public UnityEvent<int> OnPlayerLevelUp = new();
        [HideInInspector] public UnityEvent<float, float> OnExperienceChanged = new();

        private void Awake() {

            Locator.ProvideLevelManager(this);

            currentLevel = 1;
            currentXP = 0;
            xpToLevelUp = 20;

            maxLevel = 30;

            maxHealthIncreasePerLevelUp = 5;
            maxManaIncreasePerLevelUp = 5;
        }

        public int getCurrentLevel() {
            return currentLevel;
        }

        public int getXPToLevelUp() {
            return xpToLevelUp;
        }

        private void Update()
        {
#if UNITY_EDITOR
            //Cheat code
            if (Input.GetKeyDown(KeyCode.L)) {
                addXP(5);
            }
#endif
        }

        public void addXP(int val) {
            currentXP += val;
            //While-loop in case player levels up more than once in same frame
            while (currentXP >= xpToLevelUp)
            {
                currentXP -= xpToLevelUp;
                currentLevel += 1;
                xpToLevelUp = currentLevel * 5;
                OnPlayerLevelUp.Invoke(currentLevel);
            }
            OnExperienceChanged.Invoke(currentXP, xpToLevelUp);
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
