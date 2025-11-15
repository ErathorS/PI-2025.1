#if UNITY_EDITOR && !UNITY_2021_1_OR_NEWER
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneSwitcherWindow : EditorWindow
{
    string[] sceneNames = new string[0];
    string[] scenePaths = new string[0];
    int selectedIndex = 0;
    int currentMode = 0; // 0 = Enabled Build Scenes, 1 = All Build Scenes, 2 = All Scenes

    readonly string[] modeOptions = { "Enabled Build Scenes", "All Build Scenes", "All Scenes" };

    [MenuItem("Tools/Scene Switcher")]
    public static void ShowWindow() => GetWindow<SceneSwitcherWindow>("Scene Switcher");

    void OnEnable()
    {
        currentMode = EditorPrefs.GetInt("SceneSwitcher_CurrentMode_Old", 0);
        RefreshSceneList();
        SelectCurrentScene();
        EditorSceneManager.activeSceneChangedInEditMode += (prev, current) => SelectCurrentScene();
    }

    void OnDisable() => EditorPrefs.SetInt("SceneSwitcher_CurrentMode_Old", currentMode);

    void OnGUI()
    {
        GUILayout.Label("Scene Switcher", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Dropdown de modos
        int newMode = EditorGUILayout.Popup("Mode:", currentMode, modeOptions);
        if (newMode != currentMode)
        {
            currentMode = newMode;
            RefreshSceneList();
            SelectCurrentScene();
        }

        EditorGUILayout.Space();

        if (sceneNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes found.", MessageType.Warning);
            return;
        }

        // Dropdown de cenas
        int newIndex = EditorGUILayout.Popup("Select Scene:", selectedIndex, sceneNames);
        if (newIndex != selectedIndex)
        {
            selectedIndex = newIndex;
            LoadScene(scenePaths[selectedIndex]);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Refresh Scenes"))
        {
            RefreshSceneList();
            SelectCurrentScene();
        }
    }

    void RefreshSceneList()
    {
        switch (currentMode)
        {
            case 0: // Enabled Build Scenes
                scenePaths = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
                break;

            case 1: // All Build Scenes
                scenePaths = EditorBuildSettings.scenes.Select(scene => scene.path).Distinct().ToArray();
                break;

            case 2: // All Project Scenes
                scenePaths = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);
                break;
        }

        sceneNames = scenePaths.Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
    }

    void SelectCurrentScene()
    {
        string currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
        int index = System.Array.IndexOf(sceneNames, currentScene);

        if (index != -1) selectedIndex = index;
        else if (sceneNames.Length > 0) selectedIndex = 0;
    }

    void LoadScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(scenePath);
    }
}
#endif
