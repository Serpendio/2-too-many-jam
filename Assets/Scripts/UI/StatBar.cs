using TMPro;
using Tweens;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatBar : MonoBehaviour
    {
        [SerializeField] private Image _bar;
        [SerializeField] private TextMeshProUGUI _text;

        public void SetFill(float value, float max)
        {
            _bar.gameObject.CancelTweens();
            _bar.gameObject.AddTween(new ImageFillAmountTween
            {
                from = _bar.fillAmount,
                to = value / max,
                duration = 0.25f,
                easeType = EaseType.CubicOut,
            });
            
            _text.text = $"{value:N0}";
        }
    }
}