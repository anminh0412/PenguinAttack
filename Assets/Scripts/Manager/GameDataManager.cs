using Data;
using Other;
using SimpleSignalBus;

namespace Manager
{
    public class GameDataManager : MonoSingleton<GameDataManager>
    {
        public GameData            GameData;
        public GamePlayData GamePlayData;
        
        protected override void Awake()
        {
            base.Awake();
            this.GameData = new GameData();
        }

        private void Start()
        {
            SignalBus.Subscribe<StartGameSignal>(this.OnStartGame);
        }

        private void OnDestroy()
        {
            SignalBus.Unsubscribe<StartGameSignal>(this.OnStartGame);
        }

        private void OnStartGame(StartGameSignal obj)
        {
            this.GamePlayData = new GamePlayData();
        }
    }
}