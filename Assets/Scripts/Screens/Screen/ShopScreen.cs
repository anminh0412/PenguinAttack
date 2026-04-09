using Screens.Common;
using UnityEngine.UI;

namespace Screens.Screen
{
    public class ShopScreen : BaseScreen
    {
        public Button btnClose;
        public Button btnBuyCoins;
        public Button btnBuyGems;
        public Button btnBuyEnergy;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            btnClose.onClick.AddListener(OnClickClose);
            btnBuyCoins.onClick.AddListener(OnClickBuyCoins);
            btnBuyGems.onClick.AddListener(OnClickBuyGems);
            btnBuyEnergy.onClick.AddListener(OnClickBuyEnergy);
        }

        private void OnClickClose()
        {
            CloseScreen();
        }

        private void OnClickBuyCoins()
        {
            
        }

        private void OnClickBuyGems()
        {
            
        }

        private void OnClickBuyEnergy()
        {
            
        }
    }
}
