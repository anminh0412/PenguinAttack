using Cysharp.Threading.Tasks;
using Manager;
using Other;
using Screens.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Screen
{
    public class MainScreen: BaseScreen
    {
        [SerializeField] private Button btnStart;
        protected override void Initialize()
        {
            base.Initialize();
            TransitionManager.Instance.Outro(0.65f).Forget();
            InitButtons();
        }

        private void InitButtons()
        {
            this.btnStart.onClick.AddListener(this.OnClickStart);
        }

        private async void OnClickStart()
        {
            await TransitionManager.Instance.Intro(1);
            this.CloseScreen();
            await MyExtensions.LoadScene(GameConstants.GameSceneName);
        }
    }
}