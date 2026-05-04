using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data;
using Screens.Manager;
using Screens.Popup;
using Screens.Screen;
using UnityEngine;

namespace Manager
{
    public class GamePlayManager : MonoBehaviour
    {
        public Transform           plane;
        public Transform           spawnPoint;
        public PlayerController    playerController;
        public List<BotController> shootAbles = new List<BotController>();

        private enum GameState { InputPhase, ShootPhase }

        private GamePlayData Data => GameDataManager.Instance.GamePlayData;

        private const float PhaseDuration = 10f;

        private GamePlayScreen _gamePlayScreen;
        public GamePlayScreen GamePlayScreen => _gamePlayScreen;
        private async void Start()
        {
            TransitionManager.Instance.Outro(1).Forget();

            await ScreenManager.Instance.OpenScreen<GamePlayScreen>();
            _gamePlayScreen = ScreenManager.Instance.GetScreen<GamePlayScreen>();

            playerController.OnEntityDead = OnEntityDead;
            foreach (var bot in shootAbles)
                bot.OnEntityDead = OnEntityDead;

            EnterInputPhase();
        }

        private void Update()
        {
            if (Data.IsGameOver) return;

            Data.Timer -= Time.deltaTime;

            if (Data.CurrentPhase == (int)GameState.InputPhase)
                _gamePlayScreen?.UpdateAttackTime(Data.Timer, PhaseDuration);

            if (Data.Timer > 0f) return;

            if (Data.CurrentPhase == (int)GameState.InputPhase)
                EnterShootPhase();
            else
                EnterInputPhase();
        }

        private void EnterInputPhase()
        {
            Data.CurrentPhase = (int)GameState.InputPhase;
            Data.Timer        = PhaseDuration;

            _gamePlayScreen?.ResetAttackTime();
            playerController.enabled = true;
            BotShoot();
        }

        private void EnterShootPhase()
        {
            Data.CurrentPhase = (int)GameState.ShootPhase;
            Data.Timer        = PhaseDuration;

            _gamePlayScreen?.ResetAttackTime();
            playerController.enabled = false;
            ShootTime();
        }

        private void ShootTime()
        {
            playerController.Shoot();
            foreach (var bot in shootAbles)
                bot.Shoot();
        }

        private void BotShoot()
        {
            foreach (var bot in shootAbles)
                bot.RandomShot();
        }

        private bool AllBotsDead()
        {
            return shootAbles.Count > 0
                && shootAbles.All(b => b == null || !b.enabled);
        }

        private void OnEntityDead(Entity entity)
        {
            _gamePlayScreen?.ShowEntityDeadNoti(entity.entityData.Name);

            if (entity.IsPlayer)
                HandlePlayerDead().Forget();
            else
                HandleBotDead().Forget();
        }

        private async UniTaskVoid HandlePlayerDead()
        {
            if (Data.IsGameOver) return;

            Data.IsGameOver = true;
            _gamePlayScreen?.ResetAttackTime();

            if (!Data.IsReviveUsed)
            {
                await ScreenManager.Instance.OpenPopup<RevivePopup>();
                var popup = ScreenManager.Instance.GetPopup<RevivePopup>();
                if (popup != null)
                {
                    popup.OnRevive = HandleRevive;
                    popup.OnLose   = HandleLose;
                }
            }
            else
            {
                HandleLose();
            }
        }

        private async UniTaskVoid HandleBotDead()
        {
            if (Data.IsGameOver) return;

            if (AllBotsDead())
            {
                Data.IsGameOver = true;
                Data.IsWon      = true;
                _gamePlayScreen?.ResetAttackTime();
                await ScreenManager.Instance.OpenPopup<WinPopup>();
            }
        }

        private void HandleRevive()
        {
            Data.IsReviveUsed = true;
            Data.IsGameOver   = false;

            if (spawnPoint != null)
            {
                playerController.transform.position = spawnPoint.position;
                playerController.transform.rotation = spawnPoint.rotation;
            }

            var rb = playerController.rb;
            if (rb != null)
            {
                rb.isKinematic     = false;
                rb.linearVelocity  = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            foreach (var comp in playerController.GetComponents<MonoBehaviour>())
                comp.enabled = true;

            EnterInputPhase();
        }

        private async void HandleLose()
        {
            await ScreenManager.Instance.OpenPopup<LosePopup>();
        }
    }
}
