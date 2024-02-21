using TMPro;
using UnityEngine;

namespace UI
{
    public class ShowMoney : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        private void Awake() => _text = GetComponent<TextMeshProUGUI>();
        
        private void Update() => _text.text = CurrencyManager.Instance.currencyAmount.ToString();
    }
}
