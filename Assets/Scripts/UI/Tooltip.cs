using TMPro;
using Tweens;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Tooltip : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private TextMeshProUGUI _text;
        
        private void Awake()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            Hide();
        }

        public void ShowWithContent(string content)
        {
            if(content == null) return;
            gameObject.SetActive(true);
            
            _text.text = content;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_text.rectTransform);

            gameObject.AddTween(new FloatTween
            {
                from = 0f,
                to = 1f,
                duration = 0.25f,
                easeType = EaseType.CubicInOut,
                onUpdate = (_, val) => _canvasGroup.alpha = val,
                useUnscaledTime = true
            });
        }
        
        public void Hide()
        {
            
            gameObject.SetActive(false);
        }
    }
}