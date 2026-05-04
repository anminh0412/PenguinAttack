using System;
using Screens.Common;
using TMPro;
using UnityEngine.UI;

namespace Screens.Popup
{
    public class RevivePopup : BasePopup
    {
        public Button btnReviveByAds;
        public Button btnReviveByGems;
        public Button btnLose;
        public TextMeshProUGUI tmpGemCost;

        public Action OnRevive;

        public Action OnLose;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            btnReviveByAds.onClick.AddListener(OnClickReviveByAds);
            btnReviveByGems.onClick.AddListener(OnClickReviveByGems);
            btnLose.onClick.AddListener(OnClickLose);
        }

        private void OnClickReviveByAds()
        {
            OnRevive?.Invoke();
            CloseScreen();
        }

        private void OnClickReviveByGems()
        {
            OnRevive?.Invoke();
            CloseScreen();
        }

        private void OnClickLose()
        {
            OnLose?.Invoke();
            CloseScreen();
        }
    }
}