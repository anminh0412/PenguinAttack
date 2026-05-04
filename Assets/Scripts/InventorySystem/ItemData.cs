using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(
        fileName = "ItemData_",
        menuName  = "InventorySystem/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string   itemId;

        public string   displayName;

        public string   description;

        public ItemType itemType;

        public int      priceCoin;

        public int      priceDiamond;

        public bool     isDefault;

        public string   iconPath;

        public string   prefabPath;

        public int      sortOrder;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(itemId))
                Debug.LogWarning($"[ItemData] '{name}' chÆ°a cÃ³ itemId!", this);
        }
    }
}