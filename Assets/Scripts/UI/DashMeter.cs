using Core;
using Tweens;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DashMeter : MonoBehaviour
    {
        [SerializeField] private Image _barImage;
        private CanvasGroup _canvasGroup;

        private float _hiddenAlpha = 0f;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = _hiddenAlpha;
        }

        private void Start()
        {
            Locator.Player.OnDash.AddListener(UpdateDashMeter);
        }

        private void Update()
        {
            // Show underneath player
            transform.position = Camera.main.WorldToScreenPoint(Locator.Player.transform.position)
                                 - new Vector3(0, 35, 0);
        }

        private void UpdateDashMeter()
        {
            _barImage.gameObject.CancelTweens();
            _canvasGroup.gameObject.CancelTweens();

            _barImage.fillAmount = 0;

            // Bar appears
            _canvasGroup.gameObject.AddTween(new FloatTween
            {
                from = _hiddenAlpha,
                to = 1f,
                duration = 0.1f,
                onUpdate = (_, value) => _canvasGroup.alpha = value,
            });

            // Bar fills up, then hide canvas group
            _barImage.gameObject.AddTween(new FloatTween
            {
                from = 0f,
                to = 1f,
                duration = Locator.Player.DashCooldown,
                onUpdate = (_, value) => _barImage.fillAmount = value,
                onEnd = _ =>
                {
                    _canvasGroup.gameObject.AddTween(new FloatTween
                    {
                        from = 1f,
                        to = _hiddenAlpha,
                        duration = 0.25f,
                        delay = 0.5f,
                        easeType = EaseType.CubicInOut,
                        onUpdate = (_, value) => _canvasGroup.alpha = value,
                    });
                }
            });
        }
    }
}