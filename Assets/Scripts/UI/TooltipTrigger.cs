using System;
using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [TextArea]
        public string Content;
        
        private void ShowTooltip()
        {
            Locator.TooltipManager.Tooltip.ShowWithContent(Content);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            StartCoroutine(InvokeRealtime(ShowTooltip, Locator.TooltipManager.ShowDelay));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            Locator.TooltipManager.Tooltip.Hide();
        }
        
        private IEnumerator InvokeRealtime(Action action, float time)
        {
            yield return new WaitForSecondsRealtime(time);
            action();
        }
    }
}