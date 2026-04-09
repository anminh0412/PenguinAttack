using Data;
using Manager;
using Other;
using Plugins.Tick;
using SimpleSignalBus;
using Tick;
using TMPro;

namespace Currency
{
    public class EnergyCurrency : CurrencyView, ITickable
    {
        public  int             maxValue;
        public  float           recoveryTime;
        public  TextMeshProUGUI tmpRecovery;
        private GameData        gameData;

        protected override void Start()
        {
            base.Start();
            this.gameData = GameDataManager.Instance.GameData;
            TickManager.Instance.Add(this);
            this.CheckToRecovery();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TickManager.Instance.Remove(this);
        }

        protected override void UpdateCurrency(UpdateCurrency obj)
        {
            base.UpdateCurrency(obj);
            this.CheckToRecovery();
        }

        private void CheckToRecovery()
        {
            if(this.gameData.IsRecovering) return;
            var currentValue = CurrencyData.GetCurrency(this.key);
            if (currentValue < this.maxValue)
            {
                this.gameData.CurrentRecoveryTime = this.recoveryTime;
                this.gameData.IsRecovering        = true;
            }
        }

        public void Tick()
        {
            if (this.gameData.IsRecovering)
            {
                this.tmpRecovery.text = this.gameData.CurrentRecoveryTime.ToMMSS();
            }
            else
            {
                this.tmpRecovery.text = "";
            }
        }
    }
}