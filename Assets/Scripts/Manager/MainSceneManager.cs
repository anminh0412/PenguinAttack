using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Manager
{
    public class MainSceneManager : MonoBehaviour
    {
        private void Start()
        {
            TransitionManager.Instance.Outro(0.5f).Forget();
        }
    }
}