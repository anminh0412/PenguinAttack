using SimpleSignalBus;
using UnityEngine;

namespace Currency
{
    public static class CurrencyData
    {
        public static void AddCurrency(int amount, string key = "Coin")
        {
            var newValue = GetCurrency(key) + amount;
            PlayerPrefs.SetInt(key, newValue);

            SignalBus.Fire(new UpdateCurrency()
            {
                Key   = key,
                Value = newValue
            });
        }

        public static void PayCurrency(int amount, string key = "Coin")
        {
            if (!IsEnoughCurrency(amount, key)) return;
            var newValue = GetCurrency(key) - amount;
            PlayerPrefs.SetInt(key, newValue);

            SignalBus.Fire(new UpdateCurrency()
            {
                Key   = key,
                Value = newValue
            });
        }
        
        public static int    GetCurrency(string key = "Coin") => PlayerPrefs.GetInt(key, 0);

        public static bool IsEnoughCurrency(int amount, string key = "Coin") => GetCurrency(key) >= amount;
    }
}