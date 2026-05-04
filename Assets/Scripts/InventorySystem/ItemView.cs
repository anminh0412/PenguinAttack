using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem
{
    public class ItemView : MonoBehaviour
    {
        public Image              imgIcon;
        public Image              imgFrame;
        public Image              imgLockOverlay;
        public TextMeshProUGUI    tmpName;
        public TextMeshProUGUI    tmpPrice;
        public Button             btnSelect;

        public Color colorUnlocked = Color.white;
        public Color colorLocked   = new Color(0.3f, 0.3f, 0.3f, 1f);
        public Color colorEquipped = new Color(1f, 0.85f, 0f, 1f);

        private ItemData _data;
        private bool     _isUnlocked;
        private bool     _isEquipped;

        public string ItemId => _data?.itemId;

        public ItemType ItemType => _data != null ? _data.itemType : default;

        public Action<ItemData> OnClick;

        private void Awake()
        {
            btnSelect.onClick.AddListener(OnClickInternal);
        }

        public void Setup(ItemData data, bool isUnlocked, bool isEquipped)
        {
            _data       = data;
            _isUnlocked = isUnlocked;
            _isEquipped = isEquipped;

            if (tmpName  != null) tmpName.text  = data.displayName;
            if (tmpPrice != null)
            {
                if (!isUnlocked)
                    tmpPrice.text = data.priceCoin > 0
                        ? $"{data.priceCoin} ðŸª™"
                        : data.priceDiamond > 0
                            ? $"{data.priceDiamond} ðŸ’Ž"
                            : "Free";
                else
                    tmpPrice.text = string.Empty;
            }

            if (imgFrame != null)
                imgFrame.color = isEquipped ? colorEquipped : colorUnlocked;

            if (imgLockOverlay != null)
                imgLockOverlay.gameObject.SetActive(!isUnlocked);

            if (imgIcon != null)
                imgIcon.color = isUnlocked ? Color.white : colorLocked;

            LoadIconAsync().Forget();
        }

        private async UniTaskVoid LoadIconAsync()
        {
            if (_data == null || imgIcon == null) return;

            var sprite = await InventoryService.LoadIconAsync(_data);
            if (sprite == null) return;

            if (this == null || imgIcon == null) return;

            imgIcon.sprite = sprite;
            imgIcon.color  = _isUnlocked ? Color.white : colorLocked;
        }

        public void SetEquipped(bool isEquipped)
        {
            _isEquipped = isEquipped;
            if (imgFrame != null)
                imgFrame.color = isEquipped ? colorEquipped : colorUnlocked;
        }

        private void OnClickInternal()
        {
            OnClick?.Invoke(_data);
        }
    }
}