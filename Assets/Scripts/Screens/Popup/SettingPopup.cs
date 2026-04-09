using Screens.Common;
using UnityEngine.UI;

namespace Screens.Popup
{
    public class SettingPopup : BasePopup
    {
        public Toggle togSound;
        public Toggle togMusic;
        public Toggle togVibration;
        public Button btnClose;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            togSound.onValueChanged.AddListener(OnToggleSound);
            togMusic.onValueChanged.AddListener(OnToggleMusic);
            togVibration.onValueChanged.AddListener(OnToggleVibration);
            btnClose.onClick.AddListener(OnClickClose);
        }

        private void OnToggleSound(bool isOn)
        {
            
        }

        private void OnToggleMusic(bool isOn)
        {
            
        }

        private void OnToggleVibration(bool isOn)
        {
            
        }

        private void OnClickClose()
        {
            CloseScreen();
        }
    }
}
