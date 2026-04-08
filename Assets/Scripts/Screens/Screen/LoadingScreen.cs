using Screens.Common;

namespace Screens.Screen
{
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using global::Manager;
    using Other;
    using UnityEngine.UI;

    public class LoadingScreen : BaseScreen
    {
        public Image imgFill;
        private Tween fillTween;
        protected override void Initialize()
        {
            base.Initialize();
            this.imgFill.fillAmount = 0f;
        }

        public override async void OpenScreen()
        {
            base.OpenScreen();

            var duration = 3f;
            this.fillTween = this.imgFill.DOFillAmount(1f, duration);
            await UniTask.WaitForSeconds(0.95f);
            await TransitionManager.Instance.Intro(1);
            this.fillTween?.Kill();
            this.CloseScreen();
            await MyExtensions.LoadScene(GameConstants.MainSceneName);
        }

    }
}