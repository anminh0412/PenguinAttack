using DG.Tweening;
using Screens.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Screen
{
    public class GamePlayScreen : BaseScreen
    {
        [Header("Buttons")]
        public Button btnPause;
        public Button btnHome;

        [Header("Info")]
        public TextMeshProUGUI tmpScore;
        public TextMeshProUGUI tmpLevel;

        [Header("Attack Timer")]
        /// <summary>Hiển thị thời gian còn lại của ShootPhase.</summary>
        public TextMeshProUGUI tmpAttackTime;

        [Header("Notification")]
        /// <summary>Thông báo khi 1 Entity bị tiêu diệt.</summary>
        public TextMeshProUGUI tmpNoti;

        // ── internal ──────────────────────────────────────────────────────────
        private const float UrgencyThreshold = 5f;   // giây cuối gây scale pulse
        private Tween _pulseTween;
        private Tween _notiTween;

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

        // ─────────────────────────────────────────────────────────────────────
        // Attack-Time display
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Gọi mỗi frame trong ShootPhase để cập nhật đồng hồ đếm ngược.
        /// <param name="remaining">Thời gian còn lại (giây).</param>
        /// <param name="total">Tổng thời gian của pha (để tính %).</param>
        /// </summary>
        public void UpdateAttackTime(float remaining, float total)
        {
            if (tmpAttackTime == null) return;

            tmpAttackTime.text = Mathf.CeilToInt(remaining).ToString();

            // Khi vào 5 giây cuối – bắt đầu pulse scale tạo cảm giác gấp gáp
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

        /// <summary>Gọi khi ShootPhase kết thúc để reset display về trạng thái bình thường.</summary>
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

        // ─────────────────────────────────────────────────────────────────────
        // Entity-dead notification
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Hiển thị thông báo "<entityName> đã bị tiêu diệt!" rồi tự fade-out.
        /// </summary>
        public void ShowEntityDeadNoti(string entityName)
        {
            if (tmpNoti == null) return;

            _notiTween?.Kill();

            tmpNoti.text  = $"{entityName} đã bị tiêu diệt!";
            tmpNoti.alpha = 1f;

            _notiTween = tmpNoti.DOFade(0f, 1.2f)
                .SetDelay(1.5f)
                .SetEase(Ease.InQuad);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Button handlers
        // ─────────────────────────────────────────────────────────────────────

        private void OnClickPause()
        {
        }

        private void OnClickHome()
        {
        }
    }
}
