using Screens.Common;
using UnityEngine.UI;

namespace Screens.Popup
{
    public class WinPopup : BasePopup
    {
        public Button btnNext;
        public Button btnHome;
        public Button btnReplay;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            btnNext.onClick.AddListener(OnClickNext);
            btnHome.onClick.AddListener(OnClickHome);
            btnReplay.onClick.AddListener(OnClickReplay);
        }

        private void OnClickReplay()
        {
            
        }

        private void OnClickHome()
        {
            
        }

        private void OnClickNext()
        {
            
        }
    }
}