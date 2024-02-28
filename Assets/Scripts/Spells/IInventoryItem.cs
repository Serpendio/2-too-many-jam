using UnityEngine;

namespace Spells
{
    public interface IInventoryItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Sprite Icon { get; set; }
    }
}