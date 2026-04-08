using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Other
{
    using System;
    using Object = UnityEngine.Object;

    public static class PoolHelper
    {
        public const int DefaultMaxPoolSize = 20;

        private static readonly Dictionary<string, Queue<GameObject>> pools = new();
        private static readonly Dictionary<string, int> poolLimits = new();
        private static Transform poolRoot;

        private static Transform PoolRoot
        {
            get
            {
                if (poolRoot != null) return poolRoot;
                var rootObj = new GameObject("[PoolRoot]");
                Object.DontDestroyOnLoad(rootObj);
                poolRoot = rootObj.transform;
                return poolRoot;
            }
        }

        private static Queue<GameObject> GetPool(string key)
        {
            if (!pools.TryGetValue(key, out var pool))
            {
                pool = new Queue<GameObject>();
                pools[key] = pool;
            }
            return pool;
        }

        // Skips destroyed objects still sitting in the queue
        private static bool TryDequeue(Queue<GameObject> pool, out GameObject obj)
        {
            while (pool.Count > 0)
            {
                obj = pool.Dequeue();
                if (obj != null) return true;
            }
            obj = null;
            return false;
        }

        private static GameObject SpawnInternal(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var pool = GetPool(prefab.name);
            GameObject obj;

            if (TryDequeue(pool, out obj))
            {
                obj.transform.SetParent(parent ?? PoolRoot, false);
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.SetActive(true);
            }
            else
            {
                obj = Object.Instantiate(prefab, position, rotation, parent ?? PoolRoot);
            }

            return obj;
        }

        public static void SetPoolLimit(string key, int max) => poolLimits[key] = max;

        private static int GetPoolLimit(string key) =>
            poolLimits.TryGetValue(key, out var limit) ? limit : DefaultMaxPoolSize;

        private static void DespawnInternal(GameObject go)
        {
            string key = go.name.Replace("(Clone)", "").Trim();
            go.SetActive(false);

            var pool = GetPool(key);
            if (pool.Count >= GetPoolLimit(key))
            {
                Object.Destroy(go);
                return;
            }

            go.transform.SetParent(PoolRoot);
            pool.Enqueue(go);
        }

        public static GameObject Spawn(this GameObject prefab, Vector3 position = default, Quaternion rotation = default,
            Transform parent = null)
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is null!");
                return null;
            }
            return SpawnInternal(prefab, position, rotation, parent);
        }

        public static T Spawn<T>(this GameObject prefab, Vector3 position = default, Quaternion rotation = default,
            Transform parent = null) where T : Behaviour
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is null!");
                return null;
            }

            var obj = SpawnInternal(prefab, position, rotation, parent);
            if (obj.TryGetComponent<T>(out var result)) return result;

            Debug.LogError($"Component {typeof(T).Name} not found on '{obj.name}'!");
            DespawnInternal(obj);
            return null;
        }

        public static T Spawn<T>(this T prefabComponent, Vector3 position = default, Quaternion rotation = default,
            Transform parent = null) where T : Behaviour
        {
            if (prefabComponent == null)
            {
                Debug.LogError("Prefab component is null!");
                return null;
            }
            return prefabComponent.gameObject.Spawn<T>(position, rotation, parent);
        }

        public static void Despawn<T>(this T obj) where T : Behaviour
        {
            if (obj == null)
            {
                Debug.LogWarning("Trying to despawn a null object.");
                return;
            }
            DespawnInternal(obj.gameObject);
        }

        public static async UniTaskVoid Despawn<T>(this T obj, float delay) where T : Behaviour
        {
            if (obj == null)
            {
                Debug.LogWarning("Trying to despawn a null object.");
                return;
            }
            await UniTask.WaitForSeconds(delay);
            if (obj == null) return;
            DespawnInternal(obj.gameObject);
        }

        public static void Despawn(this GameObject obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("Trying to despawn a null GameObject.");
                return;
            }
            DespawnInternal(obj);
        }

        public static async UniTask Despawn(this GameObject obj, float delay, Action callback = null)
        {
            if (obj == null)
            {
                Debug.LogWarning("Trying to despawn a null GameObject.");
                return;
            }
            if (delay > 0)
                await UniTask.WaitForSeconds(delay);
            if (obj == null) return;
            DespawnInternal(obj);
            callback?.Invoke();
        }

        public static void ClearPool()
        {
            foreach (var pool in pools.Values)
            {
                while (pool.Count > 0)
                {
                    var obj = pool.Dequeue();
                    if (obj != null) Object.Destroy(obj);
                }
            }
            pools.Clear();

            if (poolRoot != null)
            {
                Object.Destroy(poolRoot.gameObject);
                poolRoot = null;
            }
        }
    }
}
