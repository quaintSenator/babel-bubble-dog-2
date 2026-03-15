using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GuideTriggerManager : BaseManager
{
    private static GuideTriggerManager instance;
    public static bool everFirstEnterSpring = false; //todo suifeng 接存档系统 记录guide情况
    [SerializeField]public GameObject levelObjects;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    private void OnEnable()
    {
        EventManager.Register<GameObject>(EventKey.PlayerHitReactableObject, OnEnterReactableArea);
    }

    private void OnDisable()
    {
        EventManager.Unregister<GameObject>(EventKey.PlayerHitReactableObject, OnEnterReactableArea);
    }

    public void OnEnterReactableArea(GameObject enteringArea)
    {
        GuideManager.StartGuide(GuideID.FirstTimeEnteringSpring);
    }
    public static GameObject FindInLevelObjects(LayerMask mask, string name)
    {
        return instance.FindInLevelObjectsInternal(mask, name);
    }

    private GameObject FindInLevelObjectsInternal(LayerMask mask, string name)
    {
        var child = levelObjects.transform.Find(name);
        if (child != null)
        {
            if (child.gameObject.layer == mask)
            {
                return child.gameObject;
            }
        }
        return null;
    }
}
