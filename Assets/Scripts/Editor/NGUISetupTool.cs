using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class NGUISetupTool
{
    [MenuItem("Tools/NGUI/Setup Essentials")]
    private static void SetupEssentials()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        Type uiRootType = FindType("UIRoot");
        Type uiCameraType = FindType("UICamera");
        Type uiPanelType = FindType("UIPanel");

        string missing = BuildMissingList(uiRootType, uiCameraType, uiPanelType);
        if (!string.IsNullOrEmpty(missing))
        {
            EditorUtility.DisplayDialog(
                "NGUI Not Ready",
                "Missing NGUI components:\n" + missing + "\n\nImport NGUI first, then run this menu again.",
                "OK");
            return;
        }

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Setup NGUI Essentials");
        int group = Undo.GetCurrentGroup();

        GameObject root = FindOrCreateRoot(uiRootType);
        FindOrCreateUICamera(root, uiCameraType);
        FindOrCreatePanel(root, uiPanelType);

        Selection.activeGameObject = root;
        EditorSceneManager.MarkSceneDirty(root.scene);
        Undo.CollapseUndoOperations(group);

        EditorUtility.DisplayDialog(
            "NGUI Setup Complete",
            "Created/verified: UI Root, UI Camera, and UI Panel.",
            "OK");
    }

    private static GameObject FindOrCreateRoot(Type uiRootType)
    {
        GameObject root = GameObject.Find("UI Root");
        if (root == null)
        {
            root = new GameObject("UI Root");
            Undo.RegisterCreatedObjectUndo(root, "Create UI Root");
        }

        AddComponentIfMissing(root, uiRootType);
        return root;
    }

    private static GameObject FindOrCreateUICamera(GameObject root, Type uiCameraType)
    {
        Transform cameraTransform = root.transform.Find("UI Camera");
        GameObject cameraObject;
        if (cameraTransform == null)
        {
            cameraObject = new GameObject("UI Camera");
            Undo.RegisterCreatedObjectUndo(cameraObject, "Create UI Camera");
            Undo.SetTransformParent(cameraObject.transform, root.transform, "Parent UI Camera");
            cameraObject.transform.localPosition = new Vector3(0f, 0f, -10f);
            cameraObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            cameraObject = cameraTransform.gameObject;
        }

        Camera camera = cameraObject.GetComponent<Camera>();
        if (camera == null)
        {
            camera = Undo.AddComponent<Camera>(cameraObject);
        }

        camera.orthographic = true;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 100f;
        camera.clearFlags = CameraClearFlags.Depth;
        camera.depth = 1f;

        AddComponentIfMissing(cameraObject, uiCameraType);
        return cameraObject;
    }

    private static GameObject FindOrCreatePanel(GameObject root, Type uiPanelType)
    {
        Transform panelTransform = root.transform.Find("UI Panel");
        GameObject panelObject;
        if (panelTransform == null)
        {
            panelObject = new GameObject("UI Panel");
            Undo.RegisterCreatedObjectUndo(panelObject, "Create UI Panel");
            Undo.SetTransformParent(panelObject.transform, root.transform, "Parent UI Panel");
            panelObject.transform.localPosition = Vector3.zero;
            panelObject.transform.localRotation = Quaternion.identity;
            panelObject.transform.localScale = Vector3.one;
        }
        else
        {
            panelObject = panelTransform.gameObject;
        }

        AddComponentIfMissing(panelObject, uiPanelType);
        return panelObject;
    }

    private static void AddComponentIfMissing(GameObject go, Type componentType)
    {
        if (go.GetComponent(componentType) == null)
        {
            Undo.AddComponent(go, componentType);
        }
    }

    private static Type FindType(string typeName)
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(assembly => assembly.GetType(typeName, false))
            .FirstOrDefault(type => type != null);
    }

    private static string BuildMissingList(params Type[] types)
    {
        string[] required = { "UIRoot", "UICamera", "UIPanel" };
        string[] missing = required.Where((_, i) => types[i] == null).ToArray();
        return string.Join("\n", missing);
    }
}
