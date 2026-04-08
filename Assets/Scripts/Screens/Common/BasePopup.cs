namespace Screens.Common
{
    using DG.Tweening;
    using Screens.Manager;
    using Screens.Screen;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.UI;

    public class BasePopup : BaseScreen
    {
        public Image     imgBg;
        public Transform content;
        public bool      closeWhenTapBackground;
        protected override void Initialize()
        {
            base.Initialize();

            if (this.closeWhenTapBackground)
            {
                var btn = this.imgBg.AddComponent<Button>();
                btn.targetGraphic = this.imgBg;
                btn.onClick.AddListener(this.OnClickBgClose);
            }
        }

        private void OnClickBgClose()
        {
            this.CloseScreen();
        }

        public override void OpenScreen()
        {
            this.content.localScale = Vector3.zero;
            this.gameObject.SetActive(true);
            this.imgBg.DOFade(0.75f, 0.75f);
            this.content.DOScale(1, 0.75f).SetEase(Ease.OutBack);
        }

        public override void CloseScreen()
        {
            this.imgBg.DOFade(0, 0.75f);
            this.content.DOScale(0, 0.75f).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    this.gameObject.SetActive(false);
                });
        }
    }
}