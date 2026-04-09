using Screens.Common;
using UnityEngine.UI;

namespace Screens.Popup
{
    public class ConfirmBackHomePopup : BasePopup
    {
        public Button btnConfirm;
        public Button btnCancel;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            btnConfirm.onClick.AddListener(OnClickConfirm);
            btnCancel.onClick.AddListener(OnClickCancel);
        }

        private void OnClickConfirm()
        {
            
        }

        private void OnClickCancel()
        {
            CloseScreen();
        }
    }
}
