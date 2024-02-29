using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class MoneyCounter : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        private void Start()
        { 
            _text = GetComponent<TextMeshProUGUI>();
            UpdateCounter(Locator.Inventory.Currency.GoldAmount);
            Locator.Inventory.Currency.OnGoldChanged.AddListener(UpdateCounter);
        }

        private void UpdateCounter(int val)
        {
            _text.text = val.ToString();
        }
    }
}