using Screens.Common;
using TMPro;
using UnityEngine.UI;

namespace Screens.Screen
{
    public class GamePlayScreen : BaseScreen
    {
        public Button btnPause;
        public Button btnHome;
        public TextMeshProUGUI tmpScore;
        public TextMeshProUGUI tmpLevel;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            btnPause.onClick.AddListener(OnClickPause);
            btnHome.onClick.AddListener(OnClickHome);
        }

        private void OnClickPause()
        {
            
        }

        private void OnClickHome()
        {
            
        }
    }
}
