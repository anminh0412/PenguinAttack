using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Screens.Manager;
using Screens.Screen;
using SimpleSignalBus;
using UnityEngine;

namespace Manager
{
    public class GamePlayManager : MonoBehaviour
    {
        public Transform           plane;
        public PlayerController    playerController;
        public List<BotController> shootAbles = new();

        private enum GameState { InputPhase, ShootPhase }
        private GameState       currentState;
        private float           timer;

        private const float PhaseDuration  = 10f;
        private const float UrgencySeconds = 5f;   // khoảng cuối có hiệu ứng scale

        // ── Screen cache ──────────────────────────────────────────────────────
        private GamePlayScreen _gamePlayScreen;

        // ─────────────────────────────────────────────────────────────────────
        private async void Start()
        {
            TransitionManager.Instance.Outro(1).Forget();

            // Mở và cache GamePlayScreen
            await ScreenManager.Instance.OpenScreen<GamePlayScreen>();
            _gamePlayScreen = ScreenManager.Instance.GetScreen<GamePlayScreen>();

            // Subscribe EntityDeadSignal
            SignalBus.Subscribe<EntityDeadSignal>(OnEntityDead);

            EnterInputPhase();
        }

        private void OnDestroy()
        {
            SignalBus.Unsubscribe<EntityDeadSignal>(OnEntityDead);
        }

        private void Update()
        {
            timer -= Time.deltaTime;

            // Cập nhật đồng hồ ShootPhase lên UI
            if (currentState == GameState.ShootPhase)
                _gamePlayScreen?.UpdateAttackTime(timer, PhaseDuration);

            if (timer > 0f) return;

            if (currentState == GameState.InputPhase)
                EnterShootPhase();
            else
                EnterInputPhase();
        }

        // ─────────────────────────────────────────────────────────────────────
        private void EnterInputPhase()
        {
            currentState = GameState.InputPhase;
            timer        = PhaseDuration;

            _gamePlayScreen?.ResetAttackTime();

            playerController.enabled = true;
            BotShoot();
        }

        private void EnterShootPhase()
        {
            currentState = GameState.ShootPhase;
            timer        = PhaseDuration;

            playerController.enabled = false;
            ShootTime();
        }

        private void ShootTime()
        {
            playerController.Shoot();
            foreach (var shoot in shootAbles)
                shoot.Shoot();
        }

        private void BotShoot()
        {
            foreach (var shoot in shootAbles)
                shoot.RandomShot();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Signal handler
        // ─────────────────────────────────────────────────────────────────────
        private void OnEntityDead(EntityDeadSignal signal)
        {
            _gamePlayScreen?.ShowEntityDeadNoti(signal.EntityName);
        }
    }
}