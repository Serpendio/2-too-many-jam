using Core;
using Spells.Modifiers;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Spells
{
    public class Inventory : MonoBehaviour
    {
        public int MaxEquippedSpells = 3;
        
        [SerializeField] private InventorySlot _inventorySlotPrefab;
        [SerializeField] private Transform _slotBase, _hotbarBase;
        [SerializeField] private GameObject _inventoryUI;
        [SerializeField] private Button mixBtn;
        // first two slots are for combining spells, then next x slots are hotbar then the rest of the inventory
        // make sure the first two are set in the editor with third left empty
        [SerializeField] private List<InventorySlot> _invSlots; 
        public int GoldAmount { get; private set; }
        private readonly List<IInventoryItem> _items = new();
        private readonly int[] _trueSlotPositions = new int[] { -1, -1 };
        private bool _currentlyViewingSpells = true;
        private int _hoveredSlot = -1, _selectedSlot = -1;

        public static UnityEvent<int> OnGoldChanged = new();

        public void AddGold(int val)
        {
            GoldAmount += val;
            OnGoldChanged.Invoke(GoldAmount);
        }

        // if this isn't a build, have a variable for spells you can set in editor which will be setup in Start()


        private void Awake()
        {
            Locator.ProvideInventory(this);
            _inventoryUI.SetActive(false);
            _invSlots[0].OnHoverBegin.AddListener(StartHover);
            _invSlots[0].OnHoverEnd.AddListener(StopHover);
            _invSlots[1].OnHoverBegin.AddListener(StartHover);
            _invSlots[1].OnHoverEnd.AddListener(StopHover);
            _items.Add(null);
            _items.Add(null);
            _items.Add(null);
            AddToInventory(null);
            var temp = MaxEquippedSpells;
            MaxEquippedSpells = 0;
            for (int i = 0; i < temp; i++)
            {
                IncreaseHotbarSize();
            }

#if UNITY_EDITOR
            foreach (var spell in _initialSpells)
            {
                AddToInventory(spell);
            }
            SwapSlots(2, MaxEquippedSpells + 2);
#endif
        }
#if UNITY_EDITOR        
        [SerializeField] private List<Spell> _initialSpells;
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                // if _invSlots != 2 in size, resize it. Don't you just love co-pilot's helpful and likely unoptimized suggestions?
                if (_invSlots.Count != 2)
                {
                    while (_invSlots.Count < 2)
                    {
                        _invSlots.Add(null);
                    }
                    while (_invSlots.Count > 2)
                    {
                        _invSlots.RemoveAt(_invSlots.Count - 1);
                    }
                }
            }
        }
#endif

        private void Start() {
            Locator.LevelManager.PlayerLevelUp.AddListener(() =>
            {
                //Update max hotbar size every maxLevel / maxHotbarSize levels to ensure even distribution between level ups
                //(e.g. every 5 levels for max level = 30, max hotbar size = 6)
                if (Locator.LevelManager.getCurrentLevel() % (Locator.LevelManager.getMaxLevel() / 6) == 0) {
                    IncreaseHotbarSize();
                }
            });
        }

        public void AddToInventory(IInventoryItem item)
        {
            _items[^1] = item;
            _items.Add(null);
            var newSlot = Instantiate(_inventorySlotPrefab, _slotBase);
            newSlot.OnHoverBegin.AddListener(StartHover);
            newSlot.OnHoverEnd.AddListener(StopHover);
            newSlot.RelatedSlot = _items.Count;
            _invSlots.Add(newSlot);

            RemoveGaps();
            ResetIndices();

            // lazy display new inventory state
            DisplayInventory(_currentlyViewingSpells);
        }

        public void IncreaseHotbarSize()
        {
            MaxEquippedSpells++;
            var newSlot = Instantiate(_inventorySlotPrefab, _hotbarBase);
            newSlot.OnHoverBegin.AddListener(StartHover);
            newSlot.OnHoverEnd.AddListener(StopHover);
            newSlot.RelatedSlot = _items.Count;
            _invSlots.Insert(1 + MaxEquippedSpells, newSlot);
            _items.Insert(1 + MaxEquippedSpells, null);

            ResetIndices();
        }

        private void ResetIndices()
        {
            for (int i = 0; i < _invSlots.Count; i++)
            {
                _invSlots[i].RelatedSlot = i;
            }
        }

        public void RemoveFromInventory(int index)
        {
            if (index < 0 || index >= _items.Count)
                return;

            if (index < 2)
            {
                if (_trueSlotPositions[index] != -1)
                {
                    _invSlots[_trueSlotPositions[index]].SetFade(false);
                    _invSlots[index].SetItem(null);
                    _trueSlotPositions[index] = -1;
                }
            }
            if (index < MaxEquippedSpells + 2)
            { 
                transform.GetChild(index).GetComponent<InventorySlot>().SetItem(null);
                _items[index + 2] = null;
            }
            else
            { 
                Destroy(_invSlots[index].gameObject);
                _items.RemoveAt(index);
            }

            ResetIndices();

            // lazy display new inventory state
            DisplayInventory(_currentlyViewingSpells);
        }

        public void SwapSlots(int slotA, int slotB)
        {
            if (slotA == slotB || slotA < 0 || slotA >= _items.Count || slotB < 0 || slotB >= _items.Count
                || slotA < MaxEquippedSpells && _items[slotB] is SpellModifier
                || slotB < MaxEquippedSpells && _items[slotA] is SpellModifier)
                return;
            if (slotA < 2)
            {
                // A is a combine slot
                if (slotB < 2)
                {
                    // both are combine slots, no need to do anything
                    _invSlots[slotA].SetItem(_items[_trueSlotPositions[1]]);
                    _invSlots[slotB].SetItem(_items[_trueSlotPositions[0]]);
                    (_trueSlotPositions[0], _trueSlotPositions[1]) = (_trueSlotPositions[1], _trueSlotPositions[0]);
                }
                else if (_trueSlotPositions[0] != -1)
                {
                    // A is already filled
                    _invSlots[_trueSlotPositions[0]].SetFade(false);
                    _trueSlotPositions[0] = slotB;
                    _invSlots[_trueSlotPositions[0]].SetFade(true);
                    _invSlots[slotA].GetComponent<InventorySlot>().SetItem(_items[_trueSlotPositions[0]]);
                }
                else
                {
                    // A is empty
                    _trueSlotPositions[0] = slotB;
                    _invSlots[_trueSlotPositions[0]].SetFade(true);
                    _invSlots[slotA].GetComponent<InventorySlot>().SetItem(_items[_trueSlotPositions[0]]);
                }
            }
            if (slotB < 2)
            {
                // B is a combine slot, A is not
                if (_trueSlotPositions[1] != -1)
                {
                    // B is already filled
                    _invSlots[_trueSlotPositions[1]].SetFade(false);
                    _trueSlotPositions[1] = slotA;
                    _invSlots[_trueSlotPositions[1]].SetFade(true);
                    _invSlots[slotB].SetItem(_items[_trueSlotPositions[1]]);
                }
                else
                {
                    // B is empty
                    _trueSlotPositions[1] = slotA;
                    _invSlots[_trueSlotPositions[1]].SetFade(true);
                    _invSlots[slotB].SetItem(_items[_trueSlotPositions[1]]);
                }
            }
            else
            {
                // neither are combine slots, just swap them
                (_items[slotA], _items[slotB]) = (_items[slotB], _items[slotA]);
                _invSlots[slotA].SetItem(_items[slotA]);
                _invSlots[slotB].SetItem(_items[slotB]);
            }

            RemoveGaps();
            ResetIndices();
            
            // if final slot is filled, add a new slot
            if (_items[^1] != null)
                AddToInventory(null);

            // lazy display new inventory state
            DisplayInventory(_currentlyViewingSpells);

            // if both combine slots are filled, & they aren't both modifiers, enable the mix button
            mixBtn.interactable = _trueSlotPositions[0] != -1 && _trueSlotPositions[1] != -1 &&
                                  !(_items[_trueSlotPositions[0]] is SpellModifier && _items[_trueSlotPositions[1]] is SpellModifier);
        }

        private void RemoveGaps()
        {
            // lazy check if gap in main part of inventory, destroy the gap
            for (var i = MaxEquippedSpells + 2; i < _items.Count - 1; i++)
            {
                if (_items[i] == null)
                {
                    Destroy(_invSlots[i].gameObject);
                    _invSlots.RemoveAt(i);
                    _items.RemoveAt(i);
                }
            }
            ResetIndices();
        }

        public void DisplayInventory(bool showSpells)
        {
            _currentlyViewingSpells = showSpells;
            for (int i = 5; i < _invSlots.Count; i++)
            {
                _invSlots[i].gameObject.SetActive(_items[i] is Spell && showSpells || _items[i] is SpellModifier && !showSpells || _items[i] == null);
            }
        }
        
        private void StartHover(int index)
        {
            _hoveredSlot = index;
        }
        
        private void StopHover()
        {
            _hoveredSlot = -1;
        }

        public void Drag(InputAction.CallbackContext context)
        {
            if (!_inventoryUI.activeSelf)
                return;

            if (context.performed)
            {                
                _selectedSlot = _hoveredSlot;
            }
            else if (context.canceled)
            {
                SwapSlots(_selectedSlot, _hoveredSlot);
            }
        }

        public void SetVisibility(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _inventoryUI.SetActive(!_inventoryUI.activeSelf);
            }
        }

        public void Combine()
        {
            if (_items[_invSlots[0].RelatedSlot] is Spell spell)
            {
                AddToInventory(spell);
                RemoveFromInventory(_invSlots[0].RelatedSlot);
                RemoveFromInventory(_invSlots[1].RelatedSlot);
            }
        }

        public Spell GetEquippedSpell(int activeSpellSlot)
        {
            if (activeSpellSlot < 0 || activeSpellSlot >= MaxEquippedSpells)
                throw new Exception($"Invalid spell slot, must be between 0 and {MaxEquippedSpells}.");
            return _items[activeSpellSlot + 2] as Spell;
        }
    }
}