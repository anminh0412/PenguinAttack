namespace Other
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public static class MyExtensions
    {
        public static bool IsNullOrEmpty(this string stringToCheck) { return stringToCheck is null or ""; }

        public static bool AreAllElementsInOtherList<T>(this List<T> listA, List<T> listB)
        {
            if (listA == null || listB == null) return false;

            if (listA.Count == 0) return true;

            if (listB.Count == 0) return false;

            return listA.All(listB.Contains);
        }

        public static bool HasAnyElementInOtherList<T>(this List<T> listA, List<T> listB)
        {
            if (listA == null || listB == null || listA.Count == 0 || listB.Count == 0)
                return false;

            return listA.Any(listB.Contains);
        }

        public static void DestroyAllChild(this Transform parent)
        {
            if (parent == null) return;

            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);

                if (child == null) continue;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    Object.DestroyImmediate(child.gameObject);

                    continue;
                }
#endif
                Object.Destroy(child.gameObject);
            }
        }

        public static async UniTask LoadScene(string sceneName)
        {
            var asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            if (asyncOp == null)
            {
                Debug.LogError($"Failed to load scene: {sceneName}");

                return;
            }

            asyncOp.allowSceneActivation = false;
            await UniTask.WaitUntil(() => asyncOp.progress >= 0.9f);
            asyncOp.allowSceneActivation = true;
            await asyncOp.ToUniTask();
            var scene = SceneManager.GetSceneByName(sceneName);

            if (scene.IsValid() && scene.isLoaded)
            {
                SceneManager.SetActiveScene(scene);
            }
            else
            {
                Debug.LogError($"Scene {sceneName} was not loaded correctly!");
            }
        }

        public static void Shuffle<T>(this List<T> list)
        {
            if (list is not { Count: > 1 }) return;

            var n = list.Count;

            while (n > 1)
            {
                n--;
                var k = Random.Range(0, n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        public static T GetRandomByWeight<T>(Dictionary<T, int> weights)
        {
            if (weights == null || weights.Count == 0)
                throw new ArgumentException("Dictionary is empty.");

            int totalWeight = 0;

            foreach (var pair in weights)
            {
                if (pair.Value > 0)
                    totalWeight += pair.Value;
            }

            if (totalWeight == 0)
                throw new ArgumentException("All weights are zero.");

            int randomValue = Random.Range(0, totalWeight);
            int cumulative  = 0;

            foreach (var pair in weights)
            {
                if (pair.Value <= 0) continue;

                cumulative += pair.Value;

                if (randomValue < cumulative)
                    return pair.Key;
            }

            throw new InvalidOperationException("Failed to select a weighted random value.");
        }

        private static readonly (int value, string symbol)[] RomanNumerals = new[]
        {
            (1000, "M"),
            (900, "CM"),
            (500, "D"),
            (400, "CD"),
            (100, "C"),
            (90, "XC"),
            (50, "L"),
            (40, "XL"),
            (10, "X"),
            (9, "IX"),
            (5, "V"),
            (4, "IV"),
            (1, "I"),
        };

        public static string ToRoman(this int number)
        {
            if (number <= 0)
                return string.Empty;

            var result = string.Empty;

            foreach (var (value, symbol) in RomanNumerals)
            {
                while (number >= value)
                {
                    result += symbol;
                    number -= value;
                }
            }

            return result;
        }

        public static T GetRandomElement<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
                throw new ArgumentException("List is null or empty");

            var index = Random.Range(0, list.Count);

            return list[index];
        }

        public static int DamageByCriticalRate(this int baseDamage, int criticalRate, out bool isCritical)
        {
            var random = Random.Range(0, 100);
            isCritical = random < criticalRate;

            return isCritical ? baseDamage * 2 : baseDamage;
        }

        public static string ToMMSS(this float seconds, RoundMode roundMode = RoundMode.Floor)
        {
            int totalSeconds = roundMode switch
            {
                RoundMode.Floor => Mathf.FloorToInt(seconds),
                RoundMode.Ceil => Mathf.CeilToInt(seconds),
                RoundMode.Round => Mathf.RoundToInt(seconds),
                _ => Mathf.FloorToInt(seconds)
            };

            int minutes = totalSeconds / 60;
            int secs    = totalSeconds % 60;

            return $"{minutes}:{secs:D2}";
        }

        public enum RoundMode
        {
            Floor,
            Round,
            Ceil
        }
#if UNITY_EDITOR
        public static void SaveScriptableObjectAtRuntime(this ScriptableObject scriptableObject)
        {
            if (scriptableObject == null)
            {
                Debug.LogError("ScriptableObject is null!");

                return;
            }

            Undo.RecordObject(scriptableObject, "Modify ScriptableObject at Runtime");
            EditorUtility.SetDirty(scriptableObject);

            AssetDatabase.SaveAssets();

            Debug.Log($"[Runtime Save]: {AssetDatabase.GetAssetPath(scriptableObject)}");
        }
#endif
        public static void PlayCollectAnimation(Transform coinTrans, Transform target, float randomDelayRange = 0.35f, Action onComplete = null)
        {
            Vector3 startPos      = coinTrans.position;
            Vector3 randomLandPos = startPos + (Vector3)Random.insideUnitCircle * 1.8f;

            void FlyToTarget()
            {
                Vector3 dir     = (target.position - coinTrans.position).normalized;
                Vector3 backPos = coinTrans.position - dir * 1.25f;

                Sequence flySeq = DOTween.Sequence();
                flySeq.SetRecyclable(true)
                    .Append(coinTrans.DOMove(backPos, 0.12f).SetEase(Ease.OutQuad))
                    .Append(coinTrans.DOMove(target.position, 0.75f).SetEase(Ease.OutSine))
                    .OnComplete(() =>
                    {
                        DOVirtual.DelayedCall(0.01f, () =>
                        {
                            if (coinTrans && coinTrans.gameObject.activeSelf)
                                coinTrans.gameObject.Despawn();
                        });
                        onComplete?.Invoke();
                        target.DOPunchScale(Vector3.one * 0.25f, 0.3f).SetEase(Ease.OutElastic);
                    });
            }

            Sequence seq = DOTween.Sequence();
            seq.SetRecyclable(true)
                .Append(coinTrans.DOJump(randomLandPos, 2f, 1, 0.45f).SetEase(Ease.OutQuad))
                .AppendInterval(0.5f)                                 
                .AppendCallback(FlyToTarget);                      

            seq.Play();
        }
    }
}