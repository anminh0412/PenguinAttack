using Currency;
using Other;
using Plugins.Tick;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class GameData : ITickable
    {
        public int CurrentLevelIndex;
        public bool  IsRecovering;
        public float CurrentRecoveryTime = 0;
        
        public void Tick()
        {
            if (this.IsRecovering)
            {
                if (this.CurrentRecoveryTime > 0)
                {
                    this.CurrentRecoveryTime -= Time.deltaTime;
                }
                else
                {
                    this.IsRecovering = false;
                    CurrencyData.AddCurrency(1, GameConstants.CurrencyKey.Energy);
                }
            }
        }
    }

    [System.Serializable]
    public class GamePlayData
    {
        public bool  IsWon;
        public bool  IsGameOver;
        public bool  IsReviveUsed;
        public float Timer;
        public int   CurrentPhase;
    }
}