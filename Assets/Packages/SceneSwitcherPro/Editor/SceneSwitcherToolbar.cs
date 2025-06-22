#if UNITY_EDITOR && UNITY_2021_1_OR_NEWER
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

[InitializeOnLoad]
public static class SceneSwitcherToolbar
{
    static string[] sceneNames = new string[0];
    static int selectedIndex = 0;
    static string lastActiveScene = "";
    static VisualElement toolbarUI;

    static readonly float positionOffset = 180f; // Move closer to Play button
    static readonly float dropdownBoxHeight = 20f; // Dropdown button height

    static int CurrentMode
    {
        get => EditorPrefs.GetInt("SceneSwitcher_CurrentMode", 0);
        set => EditorPrefs.SetInt("SceneSwitcher_CurrentMode", value);
    }

    static SceneSwitcherToolbar()
    {
        RefreshSceneList();
        SelectCurrentScene();

        //Debug.Log("<b><color=green>Thank you for using Scene Switcher Pro</color></b>\n" + "If you find this tool helpful, please consider leaving a review on the Asset Store.");

        // Hook into scene change events
        EditorSceneManager.activeSceneChangedInEditMode += (prev, current) => UpdateSceneSelection();
        EditorApplication.playModeStateChanged += OnPlayModeChanged;

        EditorApplication.delayCall += AddToolbarUI;
    }

    static void AddToolbarUI()
    {
        System.Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        if (toolbarType == null) return;

        Object[] toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
        if (toolbars.Length == 0) return;

        Object toolbar = toolbars[0];
        FieldInfo rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
        if (rootField == null) return;

        if (rootField.GetValue(toolbar) is not VisualElement root) return;

        VisualElement leftContainer = root.Q("ToolbarZoneLeftAlign");
        if (leftContainer == null) return;

        // Remove old UI if it exists to prevent duplication
        if (toolbarUI != null)
        {
            leftContainer.Remove(toolbarUI);
        }

        toolbarUI = new IMGUIContainer(OnGUI);
        toolbarUI.style.marginLeft = positionOffset;

        leftContainer.Add(toolbarUI);
    }

    static void OnGUI()
    {
        CheckAndRefreshScenes();

        if (selectedIndex >= sceneNames.Length) selectedIndex = 0;

        bool _isPlaying = EditorApplication.isPlaying;

        GUILayout.BeginHorizontal();

        //Dropdown para selecionar o modo
        EditorGUI.BeginDisabledGroup(_isPlaying);
        string[] _modes = { "Enabled Build Scenes", "All Build Scenes", "All Scenes" };
        int _currentMode = CurrentMode;

        int _newMode = EditorGUILayout.Popup(_currentMode, _modes, GUILayout.Width(150), GUILayout.Height(dropdownBoxHeight));

        if (_newMode != _currentMode)
        {
            CurrentMode = _newMode; // Save the current mode
            RefreshSceneList();
            SelectCurrentScene();
        }
        EditorGUI.EndDisabledGroup();

        //Dropdown para selecionar a cena
        EditorGUI.BeginDisabledGroup(_isPlaying);
        GUIStyle _popupStyle = new(EditorStyles.popup)
        {
            fixedHeight = dropdownBoxHeight
        };

        int _newIndex = EditorGUILayout.Popup(selectedIndex, sceneNames, _popupStyle, GUILayout.Width(150), GUILayout.Height(dropdownBoxHeight));

        if (_newIndex != selectedIndex)
        {
            selectedIndex = _newIndex;
            LoadScene(sceneNames[selectedIndex]);
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();
    }


    static void RefreshSceneList()
    {
        switch (CurrentMode)
        {
            case 0:
                sceneNames = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => Path.GetFileNameWithoutExtension(scene.path)).ToArray();
                break;
            case 1:
                sceneNames = EditorBuildSettings.scenes.Select(scene => Path.GetFileNameWithoutExtension(scene.path)).Distinct().ToArray();
                break;
            case 2:
                sceneNames = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories).Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
                break;
        }
    }

    static void CheckAndRefreshScenes()
    {
        string[] _currentScenes;

        switch (CurrentMode)
        {
            case 0:
                _currentScenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => Path.GetFileNameWithoutExtension(scene.path)).ToArray();
                break;
            case 1:
                _currentScenes = EditorBuildSettings.scenes.Select(scene => Path.GetFileNameWithoutExtension(scene.path)).Distinct().ToArray();
                break;
            case 2:
            default:
                _currentScenes = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories).Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
                break;
        }

        if (!_currentScenes.SequenceEqual(sceneNames))
        {
            sceneNames = _currentScenes;
            SelectCurrentScene();
        }
    }

    static void SelectCurrentScene()
    {
        string _currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
        int _index = System.Array.IndexOf(sceneNames, _currentScene);

        if (_index != -1)
        {
            selectedIndex = _index;
            lastActiveScene = _currentScene;
        }
        else
        {
            // Append "(not in build index)" if the scene isn't listed
            string _notInBuildName = _currentScene + " (not in build index)";

            // Insert it at the beginning or replace first element
            sceneNames = new[] { _notInBuildName }.Concat(sceneNames).ToArray();
            selectedIndex = 0;
            lastActiveScene = _currentScene;
        }
    }

    static void UpdateSceneSelection()
    {
        string _currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);

        if (_currentScene != lastActiveScene)
        {
            lastActiveScene = _currentScene;

            // Remove any previous "(not in build index)" label to avoid duplicates
            sceneNames = sceneNames.Where(name => !name.EndsWith(" (not in build index)")).ToArray();

            SelectCurrentScene();
        }
    }


    static void LoadScene(string sceneName)
    {
        string _scenePath;

        switch (CurrentMode)
        {
            case 0:
                _scenePath = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.enabled && scene.path.Contains(sceneName))?.path;
                break;
            case 1:
                _scenePath = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.path.Contains(sceneName))?.path; // Removido o filtro por scene.enabled
                break;
            case 2:
            default:
                _scenePath = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories).FirstOrDefault(path => Path.GetFileNameWithoutExtension(path) == sceneName);
                break;
        }

        if (!string.IsNullOrEmpty(_scenePath))
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(_scenePath);
            }
        }
        else
        {
            Debug.LogError("Scene not found: " + sceneName);
        }
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
        {
            EditorApplication.delayCall += () => AddToolbarUI();
        }
    }
}
#endif
