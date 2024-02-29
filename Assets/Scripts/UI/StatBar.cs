using TMPro;
using Tweens;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatBar : MonoBehaviour
    {
        [SerializeField] private Image _bar;
        [SerializeField] protected TextMeshProUGUI _text;

        public virtual void SetFill(float value, float max)
        {
            _bar.gameObject.CancelTweens();
            _bar.gameObject.AddTween(new ImageFillAmountTween
            {
                from = _bar.fillAmount,
                to = value / max,
                duration = 0.25f,
                easeType = EaseType.CubicOut,
            });
            
            _text.text = $"{Mathf.CeilToInt(value)}";
        }
    }
}