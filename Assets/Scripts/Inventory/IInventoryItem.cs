using UnityEngine;

namespace Inventory
{
    public interface IInventoryItem
    {
        public string Name { get; }
        public string Description { get; }
        public Sprite Icon { get; }
        public int GridIndex { get; set; }
    }
}