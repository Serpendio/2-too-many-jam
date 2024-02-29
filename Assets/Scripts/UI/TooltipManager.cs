using Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class TooltipManager : MonoBehaviour
    {
        public Tooltip Tooltip;
        public float ShowDelay;

        private void Awake()
        {
            Locator.ProvideTooltipManager(this);

            transform.SetAsLastSibling();
        }

        private void Update()
        {
            var pos = Mouse.current.position.ReadValue();
            var pivot = new Vector2(0, 0)
            {
                x = pos.x < Screen.width / 2f ? 0 : 1,
                y = pos.y < Screen.height / 2f ? 0 : 1
            };

            Tooltip.GetComponent<RectTransform>().pivot = pivot;

            Tooltip.transform.position = pos;
        }
    }
}