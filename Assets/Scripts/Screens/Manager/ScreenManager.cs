using System.Collections.Generic;
using Other;
using Screens.Common;

namespace Screens.Manager
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ScreenManager : MonoSingleton<ScreenManager>
    {
        private Dictionary<string, BaseScreen> screens = new Dictionary<string, BaseScreen>();
        private string CurrentScreen { get; set; }
        public async UniTask OpenScreen<T>() where T : BaseScreen
        {
            var screenName = typeof(T).Name;
            if (screenName.IsNullOrEmpty())  return;
            var screenAddress = await AddressableHelper.LoadComponent<BaseScreen>(screenName);
            
            this.CloseCurrentScreen();

            var newScreen = this.screens.TryGetValue(screenName, out var screen) ? screen : screenAddress.Spawn();
            newScreen.OpenScreen();
            this.screens[screenName] = newScreen;
            this.CurrentScreen       = screenName;
        }

        public T GetScreen<T>() where T : BaseScreen
        {
            var screenName = typeof(T).Name;
            if (this.screens.TryGetValue(screenName, out var screen) && screen is T typed)
                return typed;
            return null;
        }

        public T GetPopup<T>() where T : BaseScreen
        {
            var popupName = typeof(T).Name;
            if (this.popups.TryGetValue(popupName, out var popup) && popup is T typed)
                return typed;
            return null;
        }

        public void TriggerScreen<T>() where T : BaseScreen
        {
            var screenName = typeof(T).Name;
            if (screenName.IsNullOrEmpty())  return;

            if (this.screens.TryGetValue(screenName, out var screen))
            {
                screen.TriggerScreen();
            }
        }
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            this.screens.Clear();
            this.popups.Clear();
            this.activePopups.Clear();
        }
        
        public void CloseCurrentScreen()
        {
            if (!this.CurrentScreen.IsNullOrEmpty() && this.screens.TryGetValue(this.CurrentScreen, out var currentScreen))
            {
                currentScreen.CloseScreen();
            }
        }
        
        public void CloseScreen<T>() where T : BaseScreen
        {
            var screenName = typeof(T).Name;
            if (screenName.IsNullOrEmpty())  return;
            if (this.screens.TryGetValue(screenName, out var currentScreen))
            {
                currentScreen.CloseScreen();
            }
        }

        private Dictionary<string, BaseScreen> popups       = new Dictionary<string, BaseScreen>();
        private List<string>                   activePopups = new List<string>();
        public async UniTask OpenPopup<T>() where T : BaseScreen
        {
            var popupName = typeof(T).Name;
            if (popupName.IsNullOrEmpty())  return;
            var popupAddress = await AddressableHelper.LoadComponent<BaseScreen>(popupName);
            
            var newPopup = this.popups.TryGetValue(popupName, out var screen) ? screen : popupAddress.Spawn();
            newPopup.OpenScreen();
            this.popups[popupName] = newPopup;
            this.activePopups.Add(popupName);
        }
        
        public async UniTask<TScreen> OpenPopup<TScreen, TParam>(TParam param)
            where TScreen : BaseScreen, IPopupInitializable<TParam>
        {
            var popupName = typeof(TScreen).Name;
            if (popupName.IsNullOrEmpty()) return null;

            var popupAddress = await AddressableHelper.LoadComponent<BaseScreen>(popupName);
            var newPopup     = this.popups.TryGetValue(popupName, out var screen) ? screen : popupAddress.Spawn();

            if (newPopup is TScreen typedPopup)
            {
                typedPopup.Init(param); 
                typedPopup.OpenScreen();
                this.popups[popupName] = typedPopup;
                this.activePopups.Add(popupName);
                return typedPopup;
            }

            return null;
        }
        
        public void ClosePopup<T>() where T : BaseScreen
        {
            var popupName = typeof(T).Name;
            if (popupName.IsNullOrEmpty())  return;
            if (this.popups.TryGetValue(popupName, out var popup))
            {
                popup.CloseScreen();
            }
        }

        public void CloseAllPopups()
        {
            foreach (var activePopup in this.activePopups)
            {
                if (this.popups.TryGetValue(activePopup, out var popup))
                {
                    popup.CloseScreen();
                }
            }
            this.activePopups.Clear();
        }
    }
}