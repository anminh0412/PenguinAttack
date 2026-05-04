namespace Screens.Popup
{
    using System;
    using Cysharp.Threading.Tasks;
    using InventorySystem;
    using UnityEngine;
    using UnityEngine.UI;

    public class ItemAvatarView : MonoBehaviour
    {
        public Image      imgIcon;
        public GameObject selectedOutline;
        public Button     btnSelect;

        private ItemData _data;

        public string ItemId => this._data?.itemId;

        public Action<ItemAvatarView, ItemData> OnClickSelect;

        private void Awake()
        {
            this.btnSelect.onClick.AddListener(this.OnClick);
        }

        public void Setup(ItemData data)
        {
            this._data = data;
            this.SetSelected(false);

            if (this.imgIcon != null)
            {
                this.imgIcon.sprite = null;
                this.imgIcon.color  = Color.white;
            }

            this.LoadIconAsync().Forget();
        }

        public void SetSelected(bool isSelected)
        {
            if (this.selectedOutline != null)
                this.selectedOutline.SetActive(isSelected);
        }

        private async UniTaskVoid LoadIconAsync()
        {
            if (this._data == null || this.imgIcon == null) return;

            var sprite = await InventoryService.LoadIconAsync(this._data);

            if (this == null || this.imgIcon == null) return;

            this.imgIcon.sprite = sprite;
        }

        private void OnClick()
        {
            this.OnClickSelect?.Invoke(this, this._data);
        }
    }
}