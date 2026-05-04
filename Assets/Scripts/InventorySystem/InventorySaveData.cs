using System;
using System.Collections.Generic;

namespace InventorySystem
{
    [Serializable]
    public class InventorySaveData
    {
        public List<string> unlockedIds = new();

        public List<EquippedEntry> equippedEntries = new();

        public bool IsUnlocked(string itemId)
            => unlockedIds.Contains(itemId);

        public void Unlock(string itemId)
        {
            if (!IsUnlocked(itemId))
                unlockedIds.Add(itemId);
        }

        public void Lock(string itemId)
            => unlockedIds.Remove(itemId);

        public string GetEquipped(ItemType type)
        {
            var key = type.ToString();
            foreach (var entry in equippedEntries)
                if (entry.typeKey == key) return entry.itemId;
            return null;
        }

        public void SetEquipped(ItemType type, string itemId)
        {
            var key = type.ToString();
            foreach (var entry in equippedEntries)
            {
                if (entry.typeKey != key) continue;
                entry.itemId = itemId;
                return;
            }
            equippedEntries.Add(new EquippedEntry { typeKey = key, itemId = itemId });
        }

        public void ClearEquipped(ItemType type)
        {
            var key = type.ToString();
            equippedEntries.RemoveAll(e => e.typeKey == key);
        }
    }

    [Serializable]
    public class EquippedEntry
    {
        public string typeKey;
        public string itemId;
    }
}