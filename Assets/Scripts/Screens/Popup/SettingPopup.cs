using Screens.Common;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Popup
{
    public class SettingPopup : BasePopup
    {
        public Toggle togSound;
        public Toggle togMusic;
        public Toggle togVibration;
        public Button btnClose;

        private bool _skipListener;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        public override void OpenScreen()
        {
            base.OpenScreen();
            LoadSettings();
        }

        private void InitButtons()
        {
            togSound.onValueChanged.AddListener(OnToggleSound);
            togMusic.onValueChanged.AddListener(OnToggleMusic);
            togVibration.onValueChanged.AddListener(OnToggleVibration);
            btnClose.onClick.AddListener(OnClickClose);
        }

        private void LoadSettings()
        {
            var data = UserDataService.UserData;

            _skipListener = true;
            togSound.isOn     = data.IsSound;
            togMusic.isOn     = data.IsMusic;
            togVibration.isOn = data.IsVibration;
            _skipListener = false;

            ApplySound(data.IsSound);
            ApplyMusic(data.IsMusic);
        }

        private void OnToggleSound(bool isOn)
        {
            if (_skipListener) return;

            UserDataService.UserData.IsSound = isOn;
            UserDataService.SaveNow();
            ApplySound(isOn);
        }

        private void OnToggleMusic(bool isOn)
        {
            if (_skipListener) return;

            UserDataService.UserData.IsMusic = isOn;
            UserDataService.SaveNow();
            ApplyMusic(isOn);
        }

        private void OnToggleVibration(bool isOn)
        {
            if (_skipListener) return;

            UserDataService.UserData.IsVibration = isOn;
            UserDataService.SaveNow();
        }

        private static void ApplySound(bool isOn)
        {
            AudioListener.pause = !isOn;
        }

        private static void ApplyMusic(bool isOn)
        {
            AudioListener.volume = isOn ? 1f : 0f;
        }

        private void OnClickClose()
        {
            CloseScreen();
        }
    }
}