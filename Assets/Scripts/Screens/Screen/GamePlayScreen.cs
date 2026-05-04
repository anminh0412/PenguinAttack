using DG.Tweening;
using Screens.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Screen
{
    public class GamePlayScreen : BaseScreen
    {
        public Button btnPause;
        public Button btnHome;

        public TextMeshProUGUI tmpScore;
        public TextMeshProUGUI tmpLevel;

        public TextMeshProUGUI tmpAttackTime;

        public TextMeshProUGUI tmpNoti;

        private const float           UrgencyThreshold = 5f;
        private       Tween           _pulseTween;
        private       Tween           _notiTween;
        public        GetUIInputEvent uiInputEvent;


        protected override void Initialize()
        {
            base.Initialize();
            InitButtons();

            if (tmpNoti != null)
                tmpNoti.alpha = 0f;
        }

        private void InitButtons()
        {
            btnPause.onClick.AddListener(OnClickPause);
            btnHome.onClick.AddListener(OnClickHome);
        }

        public void UpdateAttackTime(float remaining, float total)
        {
            if (tmpAttackTime == null) return;

            tmpAttackTime.text = Mathf.CeilToInt(remaining).ToString();

            if (remaining <= UrgencyThreshold)
            {
                if (_pulseTween == null || !_pulseTween.IsActive())
                    StartUrgencyPulse();
            }
            else
            {
                StopUrgencyPulse();
            }
        }

        public void ResetAttackTime()
        {
            StopUrgencyPulse();
            if (tmpAttackTime != null)
                tmpAttackTime.rectTransform.localScale = Vector3.one;
        }

        private void StartUrgencyPulse()
        {
            _pulseTween?.Kill();
            _pulseTween = tmpAttackTime.rectTransform
                .DOScale(1.35f, 0.35f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void StopUrgencyPulse()
        {
            _pulseTween?.Kill();
            _pulseTween = null;
        }

        public void ShowEntityDeadNoti(string entityName)
        {
            if (tmpNoti == null) return;

            _notiTween?.Kill();

            tmpNoti.text  = $"{entityName} Ä‘Ã£ bá»‹ tiÃªu diá»‡t!";
            tmpNoti.alpha = 1f;

            _notiTween = tmpNoti.DOFade(0f, 1.2f)
                .SetDelay(1.5f)
                .SetEase(Ease.InQuad);
        }

        private void OnClickPause()
        {
        }

        private void OnClickHome()
        {
        }
    }
}