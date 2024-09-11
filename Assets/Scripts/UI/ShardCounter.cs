using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ShardCounter : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            UpdateCounter(Locator.Inventory.Currency.ShardAmount);
            Locator.Inventory.Currency.OnSpellShardChanged.AddListener(UpdateCounter);
        }

        private void UpdateCounter(int val)
        {
            _text.text = val.ToString();
        }
    }
}
