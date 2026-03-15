using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatReactableObject : ReactableObject
{
    public override void SetOutline(bool isOutline)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var mat = spriteRenderer.material;
        mat.SetInt("_Outline", isOutline ? 1 : 0);
    }
    
    private void OnEnable()
    {
        EventManager.Register<GameObject>(EventManager.EventKeys.PlayerHitReactableObject, OnEnterReactableArea);
        EventManager.Register<GameObject>(EventManager.EventKeys.PlayerLeaveReactableObject, OnLeaveReactableArea);
    }

    private void OnDisable()
    {
        EventManager.Unregister<GameObject>(EventManager.EventKeys.PlayerHitReactableObject, OnEnterReactableArea);
        EventManager.Unregister<GameObject>(EventManager.EventKeys.PlayerLeaveReactableObject, OnLeaveReactableArea);
    }

    public void OnEnterReactableArea(GameObject hit)
    {
        if (hit.name == gameObject.name)
        {
            SetOutline(true);
        }
    }

    public void OnLeaveReactableArea(GameObject hit)
    {
        if (hit.name == gameObject.name)
        {
            SetOutline(false);
        }
    }
}
