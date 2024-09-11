using System.Globalization;
using Core;
using Inventory;
using Spells;
using Spells.Modifiers;
using UnityEngine;
using UnityEngine.Events;
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

        public UnityEvent<IInventoryItem> OnItemChanged = new();

        private bool _dragInProgress;

        private Outline _outline;
        private TooltipTrigger _tooltipTrigger;
        private Canvas _parentCanvas;

        private void Awake()
        {
            _outline = GetComponent<Outline>();
            _tooltipTrigger = GetComponent<TooltipTrigger>();
            _parentCanvas = GetComponentInParent<Canvas>();

            SetItem(null);
        }

        public void SetItem(IInventoryItem item)
        {
            // for some reason this can just . not run (some setActive shit?)
            if (_outline == null) Awake();
            
            _outline.effectColor = Color.clear;
            _tooltipTrigger.Content = null;

            _baseImage.sprite = item?.Icon;
            _baseImage.enabled = item != null;

            foreach (Transform child in _modifierBase) Destroy(child.gameObject);

            Item = item;

            OnItemChanged.Invoke(Item);

            if (Item == null) return;

            _tooltipTrigger.Content = $"<size=120%><b>{Item.Name}</b></size>\n{Item.Description}\n\n";

            if (Item is Spell spell)
            {
                var baseStats = spell.BaseStats;
                var computed = spell.ComputedStats;
                var diff = computed - baseStats;

                var damageDiff = diff.DamageOnHit > 0
                    ? $"+{diff.DamageOnHit}"
                    : diff.DamageOnHit.ToString(CultureInfo.InvariantCulture);
                _tooltipTrigger.Content += $"Damage: {baseStats.DamageOnHit} ({damageDiff})";

                var castCooldownDiff = diff.CastCooldown > 0
                    ? $"+{diff.CastCooldown}"
                    : diff.CastCooldown.ToString(CultureInfo.InvariantCulture);
                _tooltipTrigger.Content += $"\nCooldown: {baseStats.CastCooldown} ({castCooldownDiff})";

                var manaUsageDiff = diff.ManaUsage > 0
                    ? $"+{diff.ManaUsage}"
                    : diff.ManaUsage.ToString(CultureInfo.InvariantCulture);
                _tooltipTrigger.Content += $"\nMana Usage: {baseStats.ManaUsage} ({manaUsageDiff})";

                var projectileSpeedDiff = diff.ProjectileSpeed > 0
                    ? $"+{diff.ProjectileSpeed}"
                    : diff.ProjectileSpeed.ToString(CultureInfo.InvariantCulture);
                _tooltipTrigger.Content += $"\nSpeed: {baseStats.ProjectileSpeed} ({projectileSpeedDiff})";

                var rangeDiff = diff.Range > 0 ? $"+{diff.Range}" : diff.Range.ToString(CultureInfo.InvariantCulture);
                _tooltipTrigger.Content += $"\nRange: {baseStats.Range} ({rangeDiff})";

                _tooltipTrigger.Content += "\n\n-";

                foreach (var modifier in spell.Modifiers)
                {
                    var modifierImage = new GameObject(modifier.Name, typeof(Image)).GetComponent<Image>();
                    modifierImage.transform.SetParent(_modifierBase);
                    modifierImage.sprite = modifier.Icon;

                    var rect = modifierImage.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(24, 24);
                    rect.localScale = Vector3.one;

                    _tooltipTrigger.Content += $"\n\n<b>{modifier.Name}</b>\n{modifier.Description}";
                }

                if (spell == Locator.Player.ActiveSpell)
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

                    otherSlot.SetItem(item);
                    SetItem(otherItem);
                    break;
                }
                case InventoryGroup.Hotbar when otherSlot.Group == InventoryGroup.Hotbar:
                {
                    if (otherItem == null)
                        item.GridIndex = otherSlot.transform.GetSiblingIndex();
                    else
                        (item.GridIndex, otherItem.GridIndex) = (otherItem.GridIndex, item.GridIndex);

                    otherSlot.SetItem(item);
                    SetItem(otherItem);
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

                    otherSlot.SetItem(item);
                    SetItem(otherItem);
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

                    otherSlot.SetItem(item);
                    SetItem(otherItem);
                    break;
                }
                case InventoryGroup.Items or InventoryGroup.Hotbar when otherSlot.Group == InventoryGroup.Mix:
                {
                    otherSlot.SetItem(item);
                    break;
                }
                case InventoryGroup.Mix when otherSlot.Group is InventoryGroup.Items or InventoryGroup.Hotbar:
                {
                    SetItem(otherItem);
                    break;
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Item == null) return;
            _dragInProgress = true;

            _imagesContainer.transform.SetParent(_parentCanvas.transform);

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
            _imagesContainer.transform.SetParent(transform);
            _imagesContainer.position = transform.position;

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent<InventorySlot>(out var other))
                {
                    Swap(other);
                }
            }

            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}