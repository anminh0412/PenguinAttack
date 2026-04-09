using DG.Tweening;
using Other;
using SimpleSignalBus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Currency
{
    public class CurrencyView : MonoBehaviour
    {
        public  string          key;
        public  TextMeshProUGUI tmpAmount;
        public  Button          btnAdd;
        private int             currentValue;

        protected virtual void Start()
        {
            if (this.key.IsNullOrEmpty())
            {
                Debug.LogError($"{this.gameObject.name}: key is empty!!!");
                return;
            }
            var amount = CurrencyData.GetCurrency(this.key);
            this.tmpAmount.text = amount.ToString();
            this.currentValue   = amount;
            SignalBus.Subscribe<UpdateCurrency>(this.UpdateCurrency);
            this.btnAdd.onClick.AddListener(this.OnClickAdd);
        }

        protected virtual void OnDestroy() { SignalBus.Unsubscribe<UpdateCurrency>(this.UpdateCurrency); }

        protected virtual void UpdateCurrency(UpdateCurrency obj)
        {
            if (!this.key.Equals(obj.Key)) return;
            if (!this.tmpAmount) return;

            this.tmpAmount.transform.DOKill();
            DOTween.Kill($"counter_{this.GetInstanceID()}");
            this.tmpAmount.transform.localScale = Vector3.one;
            this.tmpAmount.transform
                .DOPunchScale(Vector3.one * 0.2f, 0.3f, 1, 0.5f)
                .SetUpdate(true);

            var startValue = this.currentValue;
            this.currentValue = obj.Value;

            DOTween.To(
                    () => startValue,
                    x =>
                    {
                        startValue          = x;
                        this.tmpAmount.text = x.ToString();
                    },
                    obj.Value,
                    0.6f
                )
                .SetEase(Ease.OutQuad)
                .SetId($"counter_{this.GetInstanceID()}")
                .SetUpdate(true);
        }

        private void OnClickAdd() { }
    }
}