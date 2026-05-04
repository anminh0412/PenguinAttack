namespace Screens.Popup
{
    using System.Collections.Generic;
    using InventorySystem;
    using Other;
    using Screens.Common;
    using UnityEngine;
    using UnityEngine.UI;

    public class SelectAvatarPopup : BasePopup
    {
        public Transform gridParent;

        public ItemAvatarView itemAvatarPrefab;

        public Button btnClose;

        public ScrollRect scrollRect;

        private readonly List<ItemAvatarView> _spawnedViews = new List<ItemAvatarView>();
        private ItemAvatarView                _selectedView;

        protected override void Initialize()
        {
            base.Initialize();
            this.btnClose.onClick.AddListener(this.CloseScreen);
        }

        public override void OpenScreen()
        {
            base.OpenScreen();
            this.SpawnAvatars();
        }

        public override void CloseScreen()
        {
            this.DespawnAll();
            base.CloseScreen();
        }

        private void SpawnAvatars()
        {
            this.DespawnAll();

            if (!InventoryService.IsReady)
            {
                Debug.LogWarning("[SelectAvatarPopup] InventoryService chưa sẵn sàng.");
                return;
            }

            var avatars = InventoryService.GetUnlockedByType(ItemType.Avatar);
            if (avatars == null || avatars.Count == 0)
            {
                Debug.Log("[SelectAvatarPopup] Không có Avatar nào được unlock.");
                return;
            }

            var equippedId = InventoryService.GetEquippedId(ItemType.Avatar);

            foreach (var data in avatars)
            {
                var view = this.itemAvatarPrefab.Spawn<ItemAvatarView>(parent: this.gridParent);
                if (view == null) continue;

                view.Setup(data);
                view.OnClickSelect = this.OnAvatarClicked;

                bool isEquipped = data.itemId == equippedId;
                view.SetSelected(isEquipped);

                if (isEquipped)
                    this._selectedView = view;

                this._spawnedViews.Add(view);
            }

            if (this.scrollRect != null)
                this.scrollRect.normalizedPosition = new Vector2(0f, 1f);
        }

        private void OnAvatarClicked(ItemAvatarView view, ItemData data)
        {
            if (data == null) return;

            if (this._selectedView != null)
                this._selectedView.SetSelected(false);

            view.SetSelected(true);
            this._selectedView = view;

            InventoryService.Equip(data.itemId);
        }

        private void DespawnAll()
        {
            foreach (var view in this._spawnedViews)
            {
                if (view != null)
                    view.Despawn();
            }
            this._spawnedViews.Clear();
            this._selectedView = null;
        }
    }
}