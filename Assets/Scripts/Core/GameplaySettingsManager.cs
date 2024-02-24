using UnityEngine;

namespace Core
{
    public class GameplaySettingsManager : MonoBehaviour
    {
        [Range(0, 1)] public float CoinDropChance;
        public PotentiallyRandomFloat CoinDropValue;
        
        private void Awake()
        {
            Locator.ProvideGameplaySettingsManager(this);
        }
    }
}