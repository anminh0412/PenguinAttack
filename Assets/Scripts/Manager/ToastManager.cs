using SimpleSignalBus;

namespace Manager
{
    using DG.Tweening;
    using Other;
    using TMPro;
    using UnityEngine;

    public class ToastManager : MonoSingleton<ToastManager>
    {
        [SerializeField] private TextMeshProUGUI toastText;
        [SerializeField] private float           fadeInDuration  = 0.3f;
        [SerializeField] private float           moveUpDistance  = 50f;
        [SerializeField] private float           moveDuration    = 0.6f;
        [SerializeField] private float           displayDuration = 1.5f;
        [SerializeField] private float           fadeOutDuration = 0.4f;

        [SerializeField] private CanvasGroup   canvasGroup;
        [SerializeField] private RectTransform rectTransform;
        private                  Vector2       originalPosition;
        private                  Sequence      toastSequence;

        protected override void Awake()
        {
            base.Awake();
            canvasGroup.alpha          = 0f;
            canvasGroup.interactable   = false;
            canvasGroup.blocksRaycasts = false;

            originalPosition = rectTransform.anchoredPosition;
        }

        private void Start() { SignalBus.Subscribe<ShowToastSignal>(this.OnShowToast); }

        private void OnDestroy()
        {
            toastSequence?.Kill();
            SignalBus.Unsubscribe<ShowToastSignal>(this.OnShowToast);
        }

        private void OnShowToast(ShowToastSignal obj) { this.PlayToast(obj.Content); }

        private void PlayToast(string content)
        {
            toastSequence?.Kill();
            StopAllCoroutines();
            canvasGroup.alpha              = 0f;
            rectTransform.anchoredPosition = originalPosition;
            toastText.text                 = content;

            toastSequence = DOTween.Sequence();

            toastSequence.Append(canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad));

            toastSequence.Join(rectTransform.DOAnchorPosY(originalPosition.y + moveUpDistance, moveDuration)
                .SetEase(Ease.OutCubic));

            toastSequence.AppendInterval(displayDuration);

            toastSequence.Append(rectTransform.DOAnchorPosY(originalPosition.y + moveUpDistance + 30f, fadeOutDuration * 1.5f)
                .SetEase(Ease.InQuad));

            toastSequence.Join(canvasGroup.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuad));

            toastSequence.OnComplete(() => { rectTransform.anchoredPosition = originalPosition; });

            toastSequence.Play();
        }

#if UNITY_EDITOR
        [ContextMenu("Test Toast")]
        private void TestToast() { PlayToast("This is a sample toast message!"); }
#endif
    }
}