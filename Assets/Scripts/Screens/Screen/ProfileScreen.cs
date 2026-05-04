namespace Screens.Screen
{
    using Data;
    using Other;
    using Screens.Common;
    using Services;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ProfileScreen : BaseScreen
    {
        public  TMP_InputField  tmpName;
        public  TextMeshProUGUI tmpWins;
        public  TextMeshProUGUI tmpAces;
        public  Image           imgAvatar;
        public  Image           imgFrame;
        public  Button          btnEditAvatar;
        public  Button          btnEditFrame;
        private UserData        userData;
        
        protected override void Initialize()
        {
            base.Initialize();
            this.InitButtons();
            this.InitData();
        }
        
        private void InitButtons()
        {
            btnEditAvatar.onClick.AddListener(OnClickEditAvatar);
            btnEditFrame.onClick.AddListener(OnClickEditFrame);
        }
        
        private void OnClickEditAvatar()
        {
            
        }
        
        private void OnClickEditFrame()
        {
            
        }

        private void InitData()
        {
            this.userData = UserDataService.UserData;
            this.InitAvatar();
            this.InitFrame();
            this.tmpName.text = this.userData.Username;
            this.tmpWins.text = $"Wins: {this.userData.Wins}";
            this.tmpAces.text = $"Aces: {this.userData.Aces}";
        }

        private async void InitAvatar()
        {
            var avatarData = await AddressableHelper.LoadAsset<Sprite>(userData.CurrentAvatarPath);

            if (avatarData != null)
            {
                this.imgAvatar.sprite = avatarData;
            }
        }
        
        private async void InitFrame()
        {
            var frameData = await AddressableHelper.LoadAsset<Sprite>(userData.CurrentFramePath);

            if (frameData != null)
            {
                this.imgFrame.sprite = frameData;
            }
        }
    }
}