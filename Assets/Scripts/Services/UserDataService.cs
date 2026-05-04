using Data;
using UnityEngine;

namespace Services
{
    public static class UserDataService
    {
        private const string USER_DATA_KEY = "USER_DATA";

        public static UserData UserData;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Load();
            Application.quitting += Save;
        }

        private static void Load()
        {
            var json = PlayerPrefs.GetString(USER_DATA_KEY, null);
            UserData = !string.IsNullOrEmpty(json)
                ? JsonUtility.FromJson<UserData>(json)
                : new UserData();
        }

        private static void Save()
        {
            var json = JsonUtility.ToJson(UserData);
            PlayerPrefs.SetString(USER_DATA_KEY, json);
            PlayerPrefs.Save();
        }

        public static void SaveNow() => Save();
    }
}