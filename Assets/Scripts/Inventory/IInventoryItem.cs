using UnityEngine;

namespace Inventory
{
    public interface IInventoryItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Sprite Icon { get; }
        public int GridIndex { get; set; }
    }
}