using TMPro;
using Tweens;
using UnityEngine;

namespace UI
{
    public class HealthChangeIndicator : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        
        private float _tweenDuration = 1f;
        public float Change;

        private void Awake() => _text = GetComponentInChildren<TextMeshProUGUI>();

        private void Start()
        {
            transform.position += new Vector3(Random.Range(-0.1f, 0.25f), Random.Range(-0.1f, 0.25f), 0);
            
            _text.color = Change > 0 ? Color.green : Color.red;
            _text.text = $"{(Change > 0 ? "+" : "")}{Change:N0}";
            
            // Tween pos
            gameObject.AddTween(new PositionTween
            {
                from = transform.position,
                to = transform.position + new Vector3(Random.Range(0.25f, 0.5f), Random.Range(0.25f, 0.5f), transform.position.z),
                duration = _tweenDuration,
                easeType = EaseType.CubicOut,
            });
            
            // Tween z rot
            gameObject.AddTween(new RotationTween
            {
                from = transform.rotation,
                to = Quaternion.Euler(0, 0, Random.Range(-30, 30)),
                duration = _tweenDuration,
                easeType = EaseType.CubicOut,
            });
            
            // Tween text alpha
            _text.gameObject.AddTween(new GraphicAlphaTween
            {
                from = 1,
                to = 0,
                duration = _tweenDuration,
                easeType = EaseType.CubicOut, 
                onEnd= _ => Destroy(gameObject),
            });
        }
    }
}