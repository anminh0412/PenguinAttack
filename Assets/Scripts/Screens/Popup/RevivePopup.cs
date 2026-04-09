using Screens.Common;
using TMPro;
using UnityEngine.UI;

namespace Screens.Popup
{
    public class RevivePopup : BasePopup
    {
        public Button btnReviveByAds;
        public Button btnReviveByGems;
        public Button btnClose;
        public TextMeshProUGUI tmpGemCost;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            btnReviveByAds.onClick.AddListener(OnClickReviveByAds);
            btnReviveByGems.onClick.AddListener(OnClickReviveByGems);
            btnClose.onClick.AddListener(OnClickClose);
        }

        private void OnClickReviveByAds()
        {
            
        }

        private void OnClickReviveByGems()
        {
            
        }

        private void OnClickClose()
        {
            CloseScreen();
        }
    }
}
