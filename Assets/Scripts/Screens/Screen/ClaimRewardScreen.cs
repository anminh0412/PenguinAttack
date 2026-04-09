using Screens.Common;
using TMPro;
using UnityEngine.UI;

namespace Screens.Screen
{
    public class ClaimRewardScreen : BaseScreen
    {
        public Button btnClaim;
        public Button btnClaimX2;
        public TextMeshProUGUI tmpRewardAmount;

        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();
        }

        private void InitButtons()
        {
            btnClaim.onClick.AddListener(OnClickClaim);
            btnClaimX2.onClick.AddListener(OnClickClaimX2);
        }

        private void OnClickClaim()
        {
            
        }

        private void OnClickClaimX2()
        {
            
        }
    }
}
