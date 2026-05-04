namespace SimpleSignalBus
{
    public class StartGameSignal
    {
        
    }

    public class EndGameSignal
    {
       public bool IsWon { get; set; }
    }

    public class LoseGameSignal
    {
        
    }

    public class UpdateGamePlayCoinSignal
    {
        public int OldCoin { get; set; }
        public   int NewCoin { get; set; }
    }

    public class ShowToastSignal
    {
        public string Content { get; set; }
    }
    
    public class UpdateCurrency
    {
        public string Key;
        public int    Value;
    }

    public class EntityDeadSignal
    {
        public string EntityName { get; set; }
    }

    public class PlayerDeadSignal { }

    public class BotDeadSignal
    {
        public string EntityName { get; set; }
    }
}