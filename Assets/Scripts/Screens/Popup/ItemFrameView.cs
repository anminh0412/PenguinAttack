namespace Screens.Popup
{
    using System;
    using Cysharp.Threading.Tasks;
    using InventorySystem;
    using UnityEngine;
    using UnityEngine.UI;

    public class ItemFrameView : MonoBehaviour
    {
        public Image      imgIcon;
        public GameObject selectedOutline;
        public Button     btnSelect;

        private ItemData _data;

        public string ItemId => _data?.itemId;

        public Action<ItemFrameView, ItemData> OnClickSelect;

        private void Awake()
        {
            btnSelect.onClick.AddListener(OnClick);
        }

        public void Setup(ItemData data)
        {
            _data = data;
            SetSelected(false);

            if (imgIcon != null)
            {
                imgIcon.sprite = null;
                imgIcon.color  = Color.white;
            }

            LoadIconAsync().Forget();
        }

        public void SetSelected(bool isSelected)
        {
            if (selectedOutline != null)
                selectedOutline.SetActive(isSelected);
        }

        private async UniTaskVoid LoadIconAsync()
        {
            if (_data == null || imgIcon == null) return;

            var sprite = await InventoryService.LoadIconAsync(_data);

            if (this == null || imgIcon == null) return;

            imgIcon.sprite = sprite;
        }

        private void OnClick()
        {
            OnClickSelect?.Invoke(this, _data);
        }
    }
}