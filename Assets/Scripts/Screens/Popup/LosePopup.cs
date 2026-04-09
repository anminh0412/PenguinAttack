using Screens.Common;
using UnityEngine.UI;

namespace Screens.Popup
{
    public class LosePopup : BasePopup
    {
        public Button btnHome;
        public Button btnReplay;
        public Button btnRevive;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            btnHome.onClick.AddListener(OnClickHome);
            btnReplay.onClick.AddListener(OnClickReplay);
            btnRevive.onClick.AddListener(OnClickRevive);
        }

        private void OnClickHome()
        {
            
        }

        private void OnClickReplay()
        {
            
        }

        private void OnClickRevive()
        {
            
        }
    }
}
