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
            _bar.gameObject.AddTween(new FloatTween
            {
                from = _bar.fillAmount,
                to = value / max,
                duration = 0.25f,
                easeType = EaseType.CubicOut,
                onUpdate = (_, val) => _bar.fillAmount = val
            });
            _text.text = $"{value}";
        }
    }
}