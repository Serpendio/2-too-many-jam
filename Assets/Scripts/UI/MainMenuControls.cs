using Tweens;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class MainMenuControls : MonoBehaviour
    {
        private RectTransform _box;
        private Button _button;

        private bool _isOpen;

        [SerializeField] private float _hiddenHeight = 96;
        [SerializeField] private float _openHeight = 784;

        private void Awake()
        {
            _box = transform.parent.GetComponentInChildren<RectTransform>();
            _button = GetComponent<Button>();

            _button.onClick.AddListener(OnButtonClick);

            _box.sizeDelta = new Vector2(_box.sizeDelta.x, _hiddenHeight);
        }

        private void OnButtonClick()
        {
            _box.gameObject.AddTween(new Vector2Tween
            {
                from = _isOpen
                    ? new Vector2(_box.sizeDelta.x, _openHeight)
                    : new Vector2(_box.sizeDelta.x, _hiddenHeight),
                to = _isOpen
                    ? new Vector2(_box.sizeDelta.x, _hiddenHeight)
                    : new Vector2(_box.sizeDelta.x, _openHeight),
                duration = 0.4f,
                easeType = EaseType.BackIn,
                onUpdate = (_, value) => _box.sizeDelta = value,
                onEnd = _ =>
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_box);
                }
            });

            _isOpen = !_isOpen;
        }
    }
}