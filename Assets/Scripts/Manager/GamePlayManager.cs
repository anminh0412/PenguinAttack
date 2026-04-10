using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Manager
{
    public class GamePlayManager : MonoBehaviour
    {
        public Transform plane;
        public PlayerController playerController;
        public List<BotController> shootAbles = new();

        private enum GameState { InputPhase, ShootPhase }
        private GameState currentState;
        private float timer;

        private const float PhaseDuration = 10f;

        private void Start()
        {
            TransitionManager.Instance.Outro(1).Forget();
            EnterInputPhase();
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer > 0f) return;

            if (currentState == GameState.InputPhase)
                EnterShootPhase();
            else
                EnterInputPhase();
        }

        private void EnterInputPhase()
        {
            currentState = GameState.InputPhase;
            timer = PhaseDuration;
            playerController.enabled = true;
            BotShoot();
        }

        private void EnterShootPhase()
        {
            currentState = GameState.ShootPhase;
            timer = PhaseDuration;
            playerController.enabled = false;
            ShootTime();
        }

        private void ShootTime()
        {
            playerController.Shoot();
            foreach (var shoot in shootAbles)
            {
                shoot.Shoot();
            }
        }

        private void BotShoot()
        {
            foreach (var shoot in shootAbles)
            {
                shoot.RandomShot();
            }
        }
    }
}