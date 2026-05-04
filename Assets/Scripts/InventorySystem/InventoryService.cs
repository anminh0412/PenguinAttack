using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Other;
using UnityEngine;

namespace InventorySystem
{
    public static class InventoryService
    {
        private const string SAVE_KEY        = "INVENTORY_DATA";
        private const string CATALOG_ADDRESS = "ItemCatalog";      // Addressable key cá»§a ItemCatalog SO

        private static ItemCatalog       _catalog;
        private static InventorySaveData _saveData;

        public static bool IsReady { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            return;
            InitializeAsync().Forget();
            Application.quitting += Save;
        }

        private static async UniTaskVoid InitializeAsync()
        {
            _catalog = await AddressableHelper.LoadAsset<ItemCatalog>(CATALOG_ADDRESS);

            if (_catalog == null)
            {
                Debug.LogError("[InventoryService] KhÃ´ng tÃ¬m tháº¥y ItemCatalog táº¡i address: " + CATALOG_ADDRESS);
                return;
            }

            Load();

            foreach (var item in _catalog.GetAll().Where(i => i.isDefault))
                _saveData.Unlock(item.itemId);

            IsReady = true;
            Debug.Log($"[InventoryService] Sáºµn sÃ ng. Catalog: {_catalog.GetAll().Count} items, Unlocked: {_saveData.unlockedIds.Count}");
        }

        public static void Save()
        {
            if (_saveData == null) return;
            PlayerPrefs.SetString(SAVE_KEY, JsonUtility.ToJson(_saveData));
            PlayerPrefs.Save();
        }

        public static void Load()
        {
            var json  = PlayerPrefs.GetString(SAVE_KEY, null);
            _saveData = !string.IsNullOrEmpty(json)
                ? JsonUtility.FromJson<InventorySaveData>(json)
                : new InventorySaveData();
        }

        public static void Reset()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            _saveData = new InventorySaveData();

            if (_catalog != null)
                foreach (var item in _catalog.GetAll().Where(i => i.isDefault))
                    _saveData.Unlock(item.itemId);

            Save();
            Debug.Log("[InventoryService] ÄÃ£ reset inventory.");
        }

        public static bool Unlock(string itemId)
        {
            if (!EnsureReady()) return false;
            if (_catalog.GetById(itemId) == null)
            {
                Debug.LogWarning($"[InventoryService] Unlock tháº¥t báº¡i â€“ khÃ´ng tÃ¬m tháº¥y item: {itemId}");
                return false;
            }

            _saveData.Unlock(itemId);
            Save();
            Debug.Log($"[InventoryService] ÄÃ£ unlock: {itemId}");
            return true;
        }

        public static bool Unlock(ItemData item) => Unlock(item.itemId);

        public static bool Lock(string itemId)
        {
            if (!EnsureReady()) return false;
            _saveData.Lock(itemId);
            Save();
            return true;
        }

        public static bool IsUnlocked(string itemId)
        {
            if (!EnsureReady()) return false;
            return _saveData.IsUnlocked(itemId);
        }

        public static bool IsUnlocked(ItemData item) => IsUnlocked(item.itemId);

        public static bool Equip(string itemId)
        {
            if (!EnsureReady()) return false;
            var data = _catalog.GetById(itemId);
            if (data == null)
            {
                Debug.LogWarning($"[InventoryService] Equip tháº¥t báº¡i â€“ khÃ´ng tÃ¬m tháº¥y item: {itemId}");
                return false;
            }

            if (!IsUnlocked(itemId))
            {
                Debug.LogWarning($"[InventoryService] Equip tháº¥t báº¡i â€“ item chÆ°a unlock: {itemId}");
                return false;
            }

            _saveData.SetEquipped(data.itemType, itemId);
            Save();
            Debug.Log($"[InventoryService] ÄÃ£ equip: {itemId} ({data.itemType})");
            return true;
        }

        public static string GetEquippedId(ItemType type)
        {
            if (!EnsureReady()) return null;
            return _saveData.GetEquipped(type);
        }

        public static ItemData GetEquipped(ItemType type)
        {
            var id = GetEquippedId(type);
            return id != null ? _catalog.GetById(id) : null;
        }

        public static void Unequip(ItemType type)
        {
            if (!EnsureReady()) return;
            _saveData.ClearEquipped(type);
            Save();
        }

        public static List<ItemData> GetAll()
        {
            if (!EnsureReady()) return new List<ItemData>();
            return _catalog.GetAll();
        }

        public static List<ItemData> GetByType(ItemType type)
        {
            if (!EnsureReady()) return new List<ItemData>();
            return _catalog.GetByType(type);
        }

        public static List<ItemData> GetAllUnlocked()
        {
            if (!EnsureReady()) return new List<ItemData>();
            return _catalog.GetAll()
                           .Where(i => _saveData.IsUnlocked(i.itemId))
                           .ToList();
        }

        public static List<ItemData> GetUnlockedByType(ItemType type)
        {
            if (!EnsureReady()) return new List<ItemData>();
            return _catalog.GetByType(type)
                           .Where(i => _saveData.IsUnlocked(i.itemId))
                           .ToList();
        }

        public static List<ItemData> GetAllLocked()
        {
            if (!EnsureReady()) return new List<ItemData>();
            return _catalog.GetAll()
                           .Where(i => !_saveData.IsUnlocked(i.itemId))
                           .ToList();
        }

        public static ItemData GetById(string itemId)
        {
            if (!EnsureReady()) return null;
            return _catalog.GetById(itemId);
        }

        public static UniTask<Sprite> LoadIconAsync(ItemData item)
        {
            if (string.IsNullOrEmpty(item.iconPath))
            {
                Debug.LogWarning($"[InventoryService] Item '{item.itemId}' khÃ´ng cÃ³ iconPath.");
                return UniTask.FromResult<Sprite>(null);
            }

            return AddressableHelper.LoadAsset<Sprite>(item.iconPath);
        }

        public static UniTask<Sprite> LoadIconAsync(string itemId)
        {
            var data = GetById(itemId);
            return data != null
                ? LoadIconAsync(data)
                : UniTask.FromResult<Sprite>(null);
        }

        public static void PrintDebug()
        {
            if (!IsReady) { Debug.LogWarning("[InventoryService] ChÆ°a sáºµn sÃ ng."); return; }

            Debug.Log($"[InventoryService] === INVENTORY DEBUG ===");
            Debug.Log($"  Total items  : {_catalog.GetAll().Count}");
            Debug.Log($"  Unlocked     : {_saveData.unlockedIds.Count}");
            foreach (var id in _saveData.unlockedIds)
                Debug.Log($"    âœ“ {id}");

            Debug.Log($"  Equipped:");
            foreach (var entry in _saveData.equippedEntries)
                Debug.Log($"    [{entry.typeKey}] â†’ {entry.itemId}");
        }

        private static bool EnsureReady()
        {
            if (!IsReady)
            {
                Debug.LogWarning("[InventoryService] ChÆ°a sáºµn sÃ ng â€“ hÃ£y gá»i sau khi IsReady == true.");
                return false;
            }
            return true;
        }
    }
}