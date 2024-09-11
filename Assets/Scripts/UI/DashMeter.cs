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

        private Camera _cam;
        
        private CanvasScaler _cs;

        private float _hiddenAlpha = 0f;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = _hiddenAlpha;

            _cam = Camera.main;
            _cs = GetComponentInParent<CanvasScaler>();
        }

        private void Start()
        {
            Locator.Player.OnDash.AddListener(UpdateDashMeter);
        }

        private void Update()
        {
            // Show underneath player
            var screenPoint = _cam.WorldToScreenPoint(Locator.Player.transform.position - (Vector3.up * 0.6f));
            transform.position = screenPoint / _cs.scaleFactor;
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