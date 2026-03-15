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
        EventManager.Register<GameObject>(EventKey.PlayerHitReactableObject, OnEnterReactableArea);
        EventManager.Register<GameObject>(EventKey.PlayerLeaveReactableObject, OnLeaveReactableArea);
    }

    private void OnDisable()
    {
        EventManager.Unregister<GameObject>(EventKey.PlayerHitReactableObject, OnEnterReactableArea);
        EventManager.Unregister<GameObject>(EventKey.PlayerLeaveReactableObject, OnLeaveReactableArea);
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
