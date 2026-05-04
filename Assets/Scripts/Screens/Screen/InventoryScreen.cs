using Screens.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Screen
{
    public class InventoryScreen : BaseScreen
    {
        public Button btnClose;
        public Button btnEquip;
        public Transform contentContainer;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            btnClose.onClick.AddListener(OnClickClose);
            btnEquip.onClick.AddListener(OnClickEquip);
        }

        private void OnClickClose()
        {
            CloseScreen();
        }

        private void OnClickEquip()
        {
            
        }
    }
}