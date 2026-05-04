using System.Collections.Generic;
using Other;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class ItemUIManager : MonoBehaviour
    {
        public Transform gridParent;

        public ItemView itemViewPrefab;

        public List<Button> tabButtons = new List<Button>();

        public ScrollRect scrollRect;

        private ItemType          _currentFilter;
        private bool              _filterEnabled;
        private ItemView          _selectedView;
        private List<ItemView>    _activeViews = new List<ItemView>();

        private void Start()
        {
            InitTabs();

            Refresh();
        }

        private void InitTabs()
        {
            if (tabButtons == null || tabButtons.Count == 0) return;

            var types = System.Enum.GetValues(typeof(ItemType));

            for (int i = 0; i < tabButtons.Count && i < types.Length; i++)
            {
                var type   = (ItemType)types.GetValue(i);
                var button = tabButtons[i];

                button.onClick.AddListener(() => OnTabSelected(type));
            }
        }

        private void OnTabSelected(ItemType type)
        {
            _currentFilter = type;
            _filterEnabled = true;
            Refresh();
        }

        public void ClearFilter()
        {
            _filterEnabled = false;
            Refresh();
        }

        public void Refresh()
        {
            if (!InventoryService.IsReady)
            {
                Debug.LogWarning("[ItemUIManager] InventoryService chÆ°a sáºµn sÃ ng.");
                return;
            }

            DespawnAll();

            var items = _filterEnabled
                ? InventoryService.GetByType(_currentFilter)
                : InventoryService.GetAll();

            var equippedMap = BuildEquippedMap();

            foreach (var data in items)
            {
                var view = itemViewPrefab.Spawn<ItemView>(parent: gridParent);
                if (view == null) continue;

                bool isUnlocked = InventoryService.IsUnlocked(data.itemId);
                bool isEquipped = equippedMap.TryGetValue(data.itemId, out var eq) && eq;

                view.Setup(data, isUnlocked, isEquipped);
                view.OnClick = OnItemViewClicked;
                _activeViews.Add(view);
            }

            if (scrollRect != null)
                scrollRect.normalizedPosition = new Vector2(0f, 1f);
        }

        private void OnItemViewClicked(ItemData data)
        {
            bool isUnlocked = InventoryService.IsUnlocked(data.itemId);

            if (!isUnlocked)
            {
                TryUnlock(data);
            }
            else
            {
                InventoryService.Equip(data.itemId);

                RefreshEquippedVisual(data);
            }
        }

        protected virtual void TryUnlock(ItemData data)
        {
            bool canAfford = data.priceCoin == 0 && data.priceDiamond == 0;

            if (canAfford)
            {
                InventoryService.Unlock(data.itemId);
                Refresh();
            }
            else
            {
                Debug.Log($"[ItemUIManager] Cáº§n má»Ÿ popup mua: {data.displayName} â€“ {data.priceCoin} coin / {data.priceDiamond} gem");
                OnRequestPurchase(data);
            }
        }

        protected virtual void OnRequestPurchase(ItemData data) { }

        private void RefreshEquippedVisual(ItemData newlyEquipped)
        {
            var equippedId = InventoryService.GetEquippedId(newlyEquipped.itemType);

            foreach (var view in _activeViews)
            {
                if (view == null) continue;
                if (view.ItemType != newlyEquipped.itemType) continue;

                view.SetEquipped(view.ItemId == equippedId);
            }
        }

        private void DespawnAll()
        {
            foreach (var view in _activeViews)
            {
                if (view != null)
                    view.Despawn();
            }
            _activeViews.Clear();
            _selectedView = null;
        }

        private Dictionary<string, bool> BuildEquippedMap()
        {
            var map   = new Dictionary<string, bool>();
            var types = System.Enum.GetValues(typeof(ItemType));

            foreach (ItemType type in types)
            {
                var id = InventoryService.GetEquippedId(type);
                if (!string.IsNullOrEmpty(id))
                    map[id] = true;
            }
            return map;
        }

        private void OnDestroy()
        {
            DespawnAll();
        }
    }
}