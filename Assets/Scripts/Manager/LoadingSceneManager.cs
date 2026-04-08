using Cysharp.Threading.Tasks;
using Screens.Manager;
using Screens.Screen;
using UnityEngine;

namespace Manager
{
    public class LoadingSceneManager : MonoBehaviour
    {
        private void Start()
        {
            ScreenManager.Instance.OpenScreen<LoadingScreen>().Forget();
        }
    }
}