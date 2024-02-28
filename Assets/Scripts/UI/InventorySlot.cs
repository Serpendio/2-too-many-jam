using Spells;
using Spells.Modifiers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _modifierImg;
    [SerializeField] private Image _baseImg;
    [SerializeField] private Image overlay;

    public int RelatedSlot;
    public bool IsGreyedOut;

    public UnityEvent<int> OnHoverBegin = new();
    public UnityEvent OnHoverEnd = new();

    public void SetFade(bool shouldGrey)
    {
        IsGreyedOut = shouldGrey;
        overlay.color = new Color(0, 0, 0, shouldGrey ? 0.5f : 0);
    }

    public void SetItem(IInventoryItem item, bool greyedOut = false)
    {
        if (item is null)
        {
            _baseImg.gameObject.SetActive(false);
            return;
        }

        _baseImg.gameObject.SetActive(true);
        _baseImg.sprite = item.Icon;

        if (item is Spell spell)
        {
            foreach (Transform child in _baseImg.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var modifier in spell.Modifiers)
            {
                if (modifier == null) continue;

                Instantiate(_modifierImg, _baseImg.transform).sprite = modifier.Icon;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverBegin.Invoke(RelatedSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverEnd.Invoke();
    }
}
