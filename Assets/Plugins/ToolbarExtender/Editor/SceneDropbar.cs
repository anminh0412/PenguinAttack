using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public class SceneDropbar
{
    static SceneDropbar()
    {
        ToolbarExtender.RightToolbarGUI.Add(OnRight_ToolbarGUI);
    }

    static void OnRight_ToolbarGUI()
    {
        string currentSceneName = EditorSceneManager.GetActiveScene().name;
        if (string.IsNullOrEmpty(currentSceneName))
        {
            currentSceneName = "No Scene";
        }
        
        if (EditorGUILayout.DropdownButton(
                new GUIContent(currentSceneName), 
                FocusType.Passive, 
                EditorStyles.toolbarDropDown, 
                GUILayout.Width(120)
                ))
        {
            var data = AssetDatabase.FindAssets("t:ToolbarData");
            var path = AssetDatabase.GUIDToAssetPath(data[0]);
            var myScriptableObject = AssetDatabase.LoadAssetAtPath<ToolbarData>(path);

            string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene",  myScriptableObject.ScenePaths.ToArray());
            if (sceneGUIDs.Length <= 0)
            {
                Debug.LogWarning("No Scenes found in Assets/Scenes");
                return;
            }
            
            GenericMenu menu = new GenericMenu();
            foreach (string guid in sceneGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath); // Lấy tên scene không có .unity
                menu.AddItem(new GUIContent(sceneName), false, () => LoadScene(scenePath));
            }
            Rect buttonRect = GUILayoutUtility.GetLastRect();
            Rect menuRect = new Rect(buttonRect.x, buttonRect.y + buttonRect.height, buttonRect.width, 20);
            menu.DropDown(menuRect);
        }
    }
    static void LoadScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // Load Asset from path
            Object sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (sceneAsset != null)
            {
                // Highlight the scene asset in the Project tab
                EditorGUIUtility.PingObject(sceneAsset);

                // Optionally focus the Project window
                // EditorUtility.FocusProjectWindow();
                Selection.activeObject = sceneAsset;
            }
        }
    }
    
    private static Texture2D MakeSolidTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}