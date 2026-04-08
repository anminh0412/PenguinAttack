using UnityEngine;

namespace Other
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        var singletonObject = new GameObject(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            lock (_lock)
            {
                if (_instance != null && _instance != this)
                {
                    Destroy(gameObject); 
                    return;
                }

                _instance = (T)this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}