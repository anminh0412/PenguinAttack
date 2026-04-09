using Screens.Common;
using TMPro;
using UnityEngine.UI;

namespace Screens.Popup
{
    public class NoAdsPopup : BasePopup
    {
        public Button btnBuy;
        public Button btnClose;
        public TextMeshProUGUI tmpPrice;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            btnBuy.onClick.AddListener(OnClickBuy);
            btnClose.onClick.AddListener(OnClickClose);
        }

        private void OnClickBuy()
        {
            
        }

        private void OnClickClose()
        {
            CloseScreen();
        }
    }
}
