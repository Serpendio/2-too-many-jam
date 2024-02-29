using UnityEngine;

namespace Core
{
    public class GameplaySettingsManager : MonoBehaviour
    {
        [Range(0, 1)] public float CoinDropChance;
        public PotentiallyRandomFloat CoinDropValue;

        public PotentiallyRandomFloat ChestDropValue;
        
        public int InitialMaxEquippedSpells = 3;
        public int InitialMaxInventorySlots = 10;
        
        private void Awake()
        {
            Locator.ProvideGameplaySettingsManager(this);
        }
    }
}