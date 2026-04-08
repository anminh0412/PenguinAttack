using Cysharp.Threading.Tasks;
using Screens.Manager;
using Screens.Screen;
using UnityEngine;

namespace Manager
{
    public class MainSceneManager : MonoBehaviour
    {
        private void Start()
        {
            LoadMain();
        }

        private async void LoadMain()
        {
             ScreenManager.Instance.OpenScreen<MainScreen>().Forget();
        }
    }
}