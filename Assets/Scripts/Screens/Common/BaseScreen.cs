using UnityEngine;

namespace Screens.Common
{
    using System;

    public class BaseScreen : MonoBehaviour, IBaseScreen
    {
        private void Start()
        {
            this.Initialize();
        }

        protected virtual void Initialize()
        {
            
        }
        public virtual void OpenScreen()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void CloseScreen()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void TriggerScreen() {  }
    }
}