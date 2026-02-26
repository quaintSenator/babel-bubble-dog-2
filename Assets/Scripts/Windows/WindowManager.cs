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
    private readonly Dictionary<Type, MonoBehaviour> openedType2WindowClsDict = new Dictionary<Type, MonoBehaviour>();

    // Maintains opened windows in order: oldest -> newest.
    private readonly Queue<MonoBehaviour> windowQueue = new Queue<MonoBehaviour>();
    private MonoBehaviour topWindow;

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

    public static void CloseTopWindow()
    {
        WindowManager manager = instance != null ? instance : FindInstance();
        if (manager == null)
        {
            Debug.LogError("WindowManager.CloseTopWindow failed: no WindowManager instance found in scene.");
            return;
        }

        manager.CloseTopWindowInternal();
    }

    public static void CloseWindow(MonoBehaviour windowInstance)
    {
        if (windowInstance == null)
        {
            return;
        }

        WindowManager manager = instance != null ? instance : FindInstance();
        if (manager == null)
        {
            Debug.LogError("WindowManager.CloseWindow failed: no WindowManager instance found in scene.");
            return;
        }

        manager.CloseWindowInternal(windowInstance);
    }

    public static MonoBehaviour GetTopWindow()
    {
        WindowManager manager = instance != null ? instance : FindInstance();
        if (manager == null)
        {
            return null;
        }

        manager.PruneClosedWindows();
        return manager.topWindow;
    }

    private MonoBehaviour OpenWindowInternal(string windowName, Type windowType)
    {
        PruneClosedWindows();

        if (openedType2WindowClsDict.TryGetValue(windowType, out MonoBehaviour cachedWindow) && cachedWindow != null)
        {
            cachedWindow.gameObject.SetActive(true);
            EnqueueWindow(cachedWindow);
            return cachedWindow;
        }

        GameObject prefab = ResolvePrefab(windowName);
        if (prefab == null)
        {
            Debug.LogError("WindowManager.OpenWindow failed: prefab not found for windowName = " + windowName);
            return null;
        }

        GameObject windowObject = Instantiate(prefab, transform, false);
        windowObject.name = prefab.name;
        windowObject.SetActive(true);

        MonoBehaviour windowCls = windowObject.GetComponent(windowType) as MonoBehaviour;
        if (windowCls == null)
        {
            Debug.LogError("WindowManager.OpenWindow failed: prefab '" + prefab.name + "' does not contain component " + windowType.Name);
            Destroy(windowObject);
            return null;
        }

        openedType2WindowClsDict[windowType] = windowCls;
        EnqueueWindow(windowCls);
        return windowCls;
    }

    private void CloseTopWindowInternal()
    {
        PruneClosedWindows();
        if (topWindow == null)
        {
            return;
        }

        CloseWindowInternal(topWindow);
    }

    private void CloseWindowInternal(MonoBehaviour windowInstance)
    {
        if (windowInstance == null)
        {
            return;
        }

        RemoveWindowFromQueue(windowInstance, refreshOrder: false);
        windowInstance.gameObject.SetActive(false);

        RefreshWindowSiblingOrder();
        UpdateTopWindow();
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

    private void EnqueueWindow(MonoBehaviour window)
    {
        if (window == null)
        {
            return;
        }

        RemoveWindowFromQueue(window, refreshOrder: false);
        windowQueue.Enqueue(window);

        RefreshWindowSiblingOrder();
        UpdateTopWindow();
    }

    private void RemoveWindowFromQueue(MonoBehaviour window, bool refreshOrder)
    {
        if (window == null)
        {
            return;
        }

        bool removed = RebuildQueueWithout(window);
        if (!removed)
        {
            return;
        }

        if (refreshOrder)
        {
            RefreshWindowSiblingOrder();
            UpdateTopWindow();
        }
    }

    private bool RebuildQueueWithout(MonoBehaviour targetWindow)
    {
        bool removed = false;
        Queue<MonoBehaviour> rebuiltQueue = new Queue<MonoBehaviour>(windowQueue.Count);

        while (windowQueue.Count > 0)
        {
            MonoBehaviour window = windowQueue.Dequeue();
            if (!removed && window == targetWindow)
            {
                removed = true;
                continue;
            }

            rebuiltQueue.Enqueue(window);
        }

        while (rebuiltQueue.Count > 0)
        {
            windowQueue.Enqueue(rebuiltQueue.Dequeue());
        }

        return removed;
    }

    private void PruneClosedWindows()
    {
        List<Type> invalidTypes = null;
        foreach (KeyValuePair<Type, MonoBehaviour> pair in openedType2WindowClsDict)
        {
            if (pair.Value != null)
            {
                continue;
            }

            if (invalidTypes == null)
            {
                invalidTypes = new List<Type>();
            }

            invalidTypes.Add(pair.Key);
        }

        if (invalidTypes != null)
        {
            for (int i = 0; i < invalidTypes.Count; i++)
            {
                openedType2WindowClsDict.Remove(invalidTypes[i]);
            }
        }

        bool queueChanged = false;
        Queue<MonoBehaviour> rebuiltQueue = new Queue<MonoBehaviour>(windowQueue.Count);
        while (windowQueue.Count > 0)
        {
            MonoBehaviour window = windowQueue.Dequeue();
            if (window != null && window.gameObject.activeSelf)
            {
                rebuiltQueue.Enqueue(window);
                continue;
            }

            queueChanged = true;
        }

        while (rebuiltQueue.Count > 0)
        {
            windowQueue.Enqueue(rebuiltQueue.Dequeue());
        }

        if (queueChanged)
        {
            RefreshWindowSiblingOrder();
        }

        UpdateTopWindow();
    }

    private void RefreshWindowSiblingOrder()
    {
        foreach (MonoBehaviour window in windowQueue)
        {
            if (window == null)
            {
                continue;
            }

            window.transform.SetAsLastSibling();
        }
    }

    private void UpdateTopWindow()
    {
        topWindow = null;
        foreach (MonoBehaviour window in windowQueue)
        {
            topWindow = window;
        }
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
