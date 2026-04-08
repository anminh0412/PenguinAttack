namespace Other
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public static class AddressableHelper
    {
        private static Dictionary<string, AsyncOperationHandle> loadedHandles
            = new Dictionary<string, AsyncOperationHandle>();

        public static async UniTask<T> LoadComponent<T>(string assetName) where T : class
        {
            if (loadedHandles.ContainsKey(assetName) && loadedHandles[assetName].IsValid())
            {
                var result = loadedHandles[assetName].Result;

                if (result is T directCast)
                    return directCast;

                if (result is GameObject go)
                    return go.GetComponent<T>();
            }

            AsyncOperationHandle handle = Addressables.LoadAssetAsync<object>(assetName);
            await handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedHandles[assetName] = handle;

                if (handle.Result is T directResult)
                {
                    Debug.Log($"Loaded asset directly: {assetName}");

                    return directResult;
                }

                if (handle.Result is GameObject go)
                {
                    T component = go.GetComponent<T>();

                    if (component != null)
                    {
                        Debug.Log($"Loaded GameObject and found component {typeof(T).Name} in: {assetName}");

                        return component;
                    }
                }

                Debug.LogError($"Loaded asset {assetName} but it does not contain a component of type {typeof(T).Name}");

                return null;
            }
            else
            {
                Addressables.Release(handle);
                Debug.LogError($"Failed to load asset: {assetName}");

                return null;
            }
        }

        public static async UniTask<T> LoadAsset<T>(string assetName)
        {
            if (loadedHandles.ContainsKey(assetName) && loadedHandles[assetName].IsValid())
            {
                return (T)loadedHandles[assetName].Result;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetName);
            T                       result = await handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedHandles[assetName] = handle;
                return result;
            }
            else
            {
                Addressables.Release(handle);
                Debug.LogError($"Failed to load asset: {assetName}");

                return default(T);
            }
        }

        public static void ReleaseAsset(string assetName)
        {
            if (loadedHandles.TryGetValue(assetName, out var handle))
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                    Debug.Log($"Released asset: {assetName}");
                }
                else
                {
                    Debug.LogWarning($"Handle for {assetName} is no longer valid (possibly already released).");
                }

                loadedHandles.Remove(assetName);
            }
            else
            {
                Debug.LogWarning($"No asset found to release with name: {assetName}");
            }
        }

        public static void ReleaseAllAsset()
        {
            foreach (var kvp in loadedHandles)
            {
                var assetName = kvp.Key;
                var handle    = kvp.Value;

                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                    Debug.Log($"Released asset: {assetName}");
                }
                else
                {
                    Debug.LogWarning($"Handle for {assetName} is no longer valid (possibly already released).");
                }
            }

            loadedHandles.Clear();
            Debug.Log("Released all loaded assets.");
        }
    }
}