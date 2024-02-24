using UnityEngine;

namespace Core
{
    public class CurrencyManager : MonoBehaviour
    {
        public int GoldAmount;

        private void Awake()
        {
            Locator.ProvideCurrencyManager(this);
        }

        public void AddGold(int val) => GoldAmount += val;
    }
}