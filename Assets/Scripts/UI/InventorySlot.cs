using Core;
using Inventory;
using Spells;
using Spells.Modifiers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public enum InventoryGroup
    {
        None,
        Items,
        Hotbar,
        Mix
    }

    [RequireComponent(typeof(Outline))]
    public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public IInventoryItem Item { get; private set; }

        public InventoryGroup Group = InventoryGroup.None;

        [SerializeField] private Transform _imagesContainer;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private Image _baseImage;
        [SerializeField] private Transform _modifierBase;

        private Outline _outline;

        private bool _dragInProgress;

        private void Awake()
        {
            _outline = GetComponent<Outline>();
            SetItem(null);
        }

        public void SetItem(IInventoryItem item)
        {
            _outline.effectColor = Color.clear;

            _baseImage.sprite = item?.Icon;
            _baseImage.enabled = item != null;

            foreach (Transform child in _modifierBase) Destroy(child.gameObject);

            Item = item;
            if (Item == null) return;

            if (Item is Spell spell)
            {
                foreach (var modifier in spell.Modifiers)
                {
                    var modifierImage = new GameObject(modifier.Name, typeof(Image)).GetComponent<Image>();
                    modifierImage.transform.SetParent(_modifierBase);
                    modifierImage.sprite = modifier.Icon;

                    var rect = modifierImage.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(24, 24);
                    rect.localScale = Vector3.one;
                }

                if ((Group != InventoryGroup.Hotbar && spell.IsOnHotbar) || (Group == InventoryGroup.Hotbar && spell == Locator.Player.ActiveSpell))
                {
                    _outline.effectColor = new Color(0.9f, 0.5f, 0.2f);
                }
            }
            else if (Item is SpellModifier modifier)
            {
                foreach (Transform child in _modifierBase) Destroy(child.gameObject);

                _baseImage.sprite = modifier.Icon;
            }
        }

        public void Swap(InventorySlot otherSlot)
        {
            var item = Item;
            var otherItem = otherSlot.Item;

            switch (Group)
            {
                case InventoryGroup.Items when otherSlot.Group == InventoryGroup.Items:
                {
                    if (otherItem == null)
                        item.GridIndex = otherSlot.transform.GetSiblingIndex();
                    else
                        (item.GridIndex, otherItem.GridIndex) = (otherItem.GridIndex, item.GridIndex);
                    break;
                }
                case InventoryGroup.Hotbar when otherSlot.Group == InventoryGroup.Hotbar:
                {
                    if (otherItem == null)
                        item.GridIndex = otherSlot.transform.GetSiblingIndex();
                    else
                        (item.GridIndex, otherItem.GridIndex) = (otherItem.GridIndex, item.GridIndex);
                    break;
                }
                case InventoryGroup.Items when otherSlot.Group == InventoryGroup.Hotbar:
                {
                    if (item is not Spell spell) return;
                    if (otherItem == null)
                    {
                        item.GridIndex = otherSlot.transform.GetSiblingIndex();
                        spell.IsOnHotbar = true;
                    }
                    else
                    {
                        (item.GridIndex, otherItem.GridIndex) = (otherItem.GridIndex, item.GridIndex);
                        (spell.IsOnHotbar, ((Spell)otherItem).IsOnHotbar) = (true, false);
                    }

                    break;
                }
                case InventoryGroup.Hotbar when otherSlot.Group == InventoryGroup.Items:
                {
                    if (otherItem == null)
                    {
                        item.GridIndex = otherSlot.transform.GetSiblingIndex();
                        ((Spell)item).IsOnHotbar = false;
                    }
                    else
                    {
                        if (otherItem is not Spell otherSpell) return;
                        (item.GridIndex, otherItem.GridIndex) = (otherItem.GridIndex, item.GridIndex);
                        (otherSpell.IsOnHotbar, ((Spell)item).IsOnHotbar) = (true, false);
                    }

                    break;
                }
            }

            otherSlot.SetItem(item);
            SetItem(otherItem);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Item == null) return;
            _dragInProgress = true;

            // _imagesContainer.transform.parent = null;

            _canvasGroup.alpha = 0.5f;
            _canvasGroup.blocksRaycasts = false;
            _imagesContainer.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_dragInProgress) return;
            _imagesContainer.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_dragInProgress) return;
            _dragInProgress = false;

            // _imagesContainer.transform.parent = transform;
            _imagesContainer.position = transform.position;

            if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent<InventorySlot>(out var other))
            {
                Swap(other);
            }

            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}