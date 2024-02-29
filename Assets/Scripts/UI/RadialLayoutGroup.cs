using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RadialLayoutGroup : LayoutGroup
    {
        public float Radius;
        
        [Range(0f, 360f)]
        public float MinAngle, MaxAngle, StartAngle;

        protected override void OnEnable()
        {
            base.OnEnable();
            CalculateRadial();
        }
        
        public override void SetLayoutHorizontal()
        {
        }
        
        public override void SetLayoutVertical()
        {
        }
        
        public override void CalculateLayoutInputVertical()
        {
            CalculateRadial();
        }
        
        public override void CalculateLayoutInputHorizontal()
        {
            CalculateRadial();
        }
        
        private void CalculateRadial()
        {
            m_Tracker.Clear();
            if (transform.childCount == 0) return;
            
            var offsetAngle = (MaxAngle - MinAngle) / (transform.childCount - 1);

            var angle = StartAngle;
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = (RectTransform)transform.GetChild(i);
                if (child == null) continue;
                
                //Adding the elements to the tracker stops the user from modifiying their positions via the editor.
                m_Tracker.Add(this, child,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.Pivot);
                
                var vPos = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                child.localPosition = vPos * Radius;
                
                //Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
                angle += offsetAngle;
            }
        }
    }
}