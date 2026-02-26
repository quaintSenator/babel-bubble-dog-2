using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WindowManager : MonoBehaviour
{
    [Serializable]
    private struct WindowPrefabEntry
    {
        public string windowName;
        public GameObject prefab;
    }

    private static readonly Dictionary<string, Type> WindowTypeByName = new Dictionary<string, Type>(StringComparer.Ordinal)
    {
        // Hardcoded windowName <-> windowClass mapping.
        { "TestWindow", typeof(TestWindow) },
        { "Test Window", typeof(TestWindow) },
    };

    private static readonly Dictionary<string, string> PrefabPathByWindowName = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        { "TestWindow", "Assets/Prefabs/UI/Windows/Test Window.prefab" },
        { "Test Window", "Assets/Prefabs/UI/Windows/Test Window.prefab" },
    };

    private static WindowManager instance;

    [SerializeField]
    private List<WindowPrefabEntry> windowPrefabs = new List<WindowPrefabEntry>();

    private readonly Dictionary<string, GameObject> prefabByWindowName = new Dictionary<string, GameObject>(StringComparer.Ordinal);
    private readonly Dictionary<Type, MonoBehaviour> openedWindows = new Dictionary<Type, MonoBehaviour>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        RebuildPrefabLookup();
    }

    private void OnValidate()
    {
        RebuildPrefabLookup();
    }

    public static MonoBehaviour OpenWindow(string windowName)
    {
        if (string.IsNullOrWhiteSpace(windowName))
        {
            Debug.LogError("WindowManager.OpenWindow failed: windowName is empty.");
            return null;
        }

        if (!WindowTypeByName.TryGetValue(windowName, out Type windowType))
        {
            Debug.LogError("WindowManager.OpenWindow failed: unknown windowName = " + windowName);
            return null;
        }

        WindowManager manager = instance != null ? instance : FindInstance();
        if (manager == null)
        {
            Debug.LogError("WindowManager.OpenWindow failed: no WindowManager instance found in scene.");
            return null;
        }

        return manager.OpenWindowInternal(windowName, windowType);
    }

    private MonoBehaviour OpenWindowInternal(string windowName, Type windowType)
    {
        if (openedWindows.TryGetValue(windowType, out MonoBehaviour cachedWindow) && cachedWindow != null)
        {
            cachedWindow.gameObject.SetActive(true);
            cachedWindow.transform.SetAsLastSibling();
            return cachedWindow;
        }

        GameObject prefab = ResolvePrefab(windowName);
        if (prefab == null)
        {
            Debug.LogError("WindowManager.OpenWindow failed: prefab not found for windowName = " + windowName);
            return null;
        }

        Transform parent = FindWindowParent();
        GameObject windowObject = parent != null ? Instantiate(prefab, parent, false) : Instantiate(prefab);
        windowObject.name = prefab.name;
        windowObject.SetActive(true);

        MonoBehaviour windowCls = windowObject.GetComponent(windowType) as MonoBehaviour;
        if (windowCls == null)
        {
            Debug.LogError("WindowManager.OpenWindow failed: prefab '" + prefab.name + "' does not contain component " + windowType.Name);
            Destroy(windowObject);
            return null;
        }

        openedWindows[windowType] = windowCls;
        return windowCls;
    }

    private static WindowManager FindInstance()
    {
        WindowManager manager = FindObjectOfType<WindowManager>();
        if (manager != null)
        {
            instance = manager;
            instance.RebuildPrefabLookup();
        }

        return manager;
    }

    private Transform FindWindowParent()
    {
        UIRoot uiRoot = FindObjectOfType<UIRoot>();
        return uiRoot != null ? uiRoot.transform : transform;
    }

    private void RebuildPrefabLookup()
    {
        prefabByWindowName.Clear();

        for (int i = 0; i < windowPrefabs.Count; i++)
        {
            WindowPrefabEntry entry = windowPrefabs[i];
            if (string.IsNullOrWhiteSpace(entry.windowName) || entry.prefab == null)
            {
                continue;
            }

            prefabByWindowName[entry.windowName] = entry.prefab;
        }
    }

    private GameObject ResolvePrefab(string windowName)
    {
        if (prefabByWindowName.TryGetValue(windowName, out GameObject prefab) && prefab != null)
        {
            return prefab;
        }

        if (TryLoadPrefabByHardcodedPath(windowName, out prefab))
        {
            prefabByWindowName[windowName] = prefab;
            return prefab;
        }

        return null;
    }

    private static bool TryLoadPrefabByHardcodedPath(string windowName, out GameObject prefab)
    {
        prefab = null;

        if (!PrefabPathByWindowName.TryGetValue(windowName, out string assetPath))
        {
            return false;
        }

#if UNITY_EDITOR
        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (prefab != null)
        {
            return true;
        }
#endif

        string resourcesPath = ToResourcesPath(assetPath);
        if (!string.IsNullOrEmpty(resourcesPath))
        {
            prefab = Resources.Load<GameObject>(resourcesPath);
            if (prefab != null)
            {
                return true;
            }
        }

        return false;
    }

    private static string ToResourcesPath(string assetPath)
    {
        const string resourcesSegment = "/Resources/";
        int resourcesIndex = assetPath.IndexOf(resourcesSegment, StringComparison.OrdinalIgnoreCase);
        if (resourcesIndex < 0)
        {
            return null;
        }

        string path = assetPath.Substring(resourcesIndex + resourcesSegment.Length);
        int extensionIndex = path.LastIndexOf('.');
        return extensionIndex >= 0 ? path.Substring(0, extensionIndex) : path;
    }
}
