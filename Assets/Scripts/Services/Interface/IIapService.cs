namespace Services.Interface
{
    using System;

    public interface IIapService
    {
        void   Initialize();
        bool   IsInitialized { get; }
        void   Purchase(string productId, Action<bool, string> onComplete);
        bool   IsProductOwned(string productId);
        void   RestorePurchases();
        string GetLocalizedPrice(string productId);
    }
}