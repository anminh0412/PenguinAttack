using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(
        fileName = "ItemCatalog",
        menuName  = "InventorySystem/Item Catalog")]
    public class ItemCatalog : ScriptableObject
    {
        public List<ItemData> items = new();

        private Dictionary<string, ItemData> _byId;

        private void OnEnable()  => BuildCache();
        private void OnValidate() => BuildCache();

        private void BuildCache()
        {
            _byId = items
                .Where(i => i != null && !string.IsNullOrEmpty(i.itemId))
                .ToDictionary(i => i.itemId);
        }

        public ItemData GetById(string id)
        {
            if (_byId == null) BuildCache();
            return _byId.TryGetValue(id, out var data) ? data : null;
        }

        public List<ItemData> GetByType(ItemType type)
            => items.Where(i => i != null && i.itemType == type)
                    .OrderBy(i => i.sortOrder)
                    .ToList();

        public List<ItemData> GetAll()
            => items.Where(i => i != null)
                    .OrderBy(i => i.sortOrder)
                    .ToList();
    }
}