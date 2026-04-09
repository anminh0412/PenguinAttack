using Screens.Common;
using UnityEngine.UI;

namespace Screens.Screen
{
    public class HomeScreen : BaseScreen
    {
        public Button btnPlay;
        public Button btnShop;
        public Button btnInventory;
        public Button btnSetting;
        public Button btnNoAds;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            btnPlay.onClick.AddListener(OnClickPlay);
            btnShop.onClick.AddListener(OnClickShop);
            btnInventory.onClick.AddListener(OnClickInventory);
            btnSetting.onClick.AddListener(OnClickSetting);
            btnNoAds.onClick.AddListener(OnClickNoAds);
        }

        private void OnClickPlay()
        {
            
        }

        private void OnClickShop()
        {
            
        }

        private void OnClickInventory()
        {
            
        }

        private void OnClickSetting()
        {
            
        }

        private void OnClickNoAds()
        {
            
        }
    }
}
