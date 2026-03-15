using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GuideID
{
    FirstTimeEnteringSpring,
}

[Serializable]
public class GuideTableItem
{
    [SerializeField]
    public GuideID GuideId;
    [SerializeField]
    public UnityEngine.Object GuideAsset;
    
    [SerializeField, HideInInspector]
    private string guideTypeName;

    public Type ResolveGuideType()
    {
        if (string.IsNullOrWhiteSpace(guideTypeName))
        {
            return null;
        }

        return Type.GetType(guideTypeName);
    }

#if UNITY_EDITOR
    public void EditorSyncGuideType()
    {
        guideTypeName = null;
        if (GuideAsset == null)
        {
            return;
        }

        Type guideType = null;
        if (GuideAsset is MonoScript monoScript)
        {
            guideType = monoScript.GetClass();
        }
        else if (GuideAsset is ScriptableObject scriptableObject)
        {
            guideType = scriptableObject.GetType();
        }

        if (guideType != null)
        {
            guideTypeName = guideType.AssemblyQualifiedName;
        }
    }
#endif
}

public class GuideManager : BaseManager
{
    [SerializeField] List<GuideTableItem> guideTableItems = new List<GuideTableItem>();
    private  static GuideManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public static BaseGuide StartGuide(GuideID guideId)
    {
        if (instance == null)
        {
            Debug.LogError("GuideManager instance not found in scene.");
            return null;
        }

        instance = (GuideManager)instance;
        return instance.CreateGuideInstance(guideId);
    }

    private BaseGuide CreateGuideInstance(GuideID guideId)
    {
        GuideTableItem item = null;
        for (int i = 0; i < guideTableItems.Count; i++)
        {
            if (guideTableItems[i] != null && guideTableItems[i].GuideId == guideId)
            {
                item = guideTableItems[i];
                break;
            }
        }

        if (item == null)
        {
            Debug.LogError($"GuideId not found: {guideId}");
            return null;
        }

        Type guideType = item.ResolveGuideType();
        if (guideType == null)
        {
            Debug.LogError($"Guide type not resolved for GuideId: {guideId}");
            return null;
        }

        if (!typeof(BaseGuide).IsAssignableFrom(guideType))
        {
            Debug.LogError($"Guide type {guideType.FullName} does not derive from BaseGuide.");
            return null;
        }

        try
        {
            return (BaseGuide)Activator.CreateInstance(guideType);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create guide instance for {guideType.FullName}: {ex}");
            return null;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (guideTableItems == null)
        {
            return;
        }

        for (int i = 0; i < guideTableItems.Count; i++)
        {
            if (guideTableItems[i] != null)
            {
                guideTableItems[i].EditorSyncGuideType();
            }
        }
    }
#endif
    
}
