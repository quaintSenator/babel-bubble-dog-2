using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class DragableDog : MonoBehaviour
{
    [SerializeField] Camera targetCamera;
    [SerializeField] bool logDragEveryFrame = true;
    [SerializeField] bool applyTranslation = true;
    [SerializeField] bool applyRotation = true;
    [SerializeField] Transform RootBone;
    [SerializeField] List<Transform> UnDragableNodes = new List<Transform>();

    SpriteRenderer[] spriteRenderers = new SpriteRenderer[0];
    Collider2D[] colliders = new Collider2D[0];
    readonly System.Collections.Generic.Dictionary<SpriteRenderer, SpriteSkin> skinByRenderer =
        new System.Collections.Generic.Dictionary<SpriteRenderer, SpriteSkin>();

    SpriteRenderer activeRenderer;
    SpriteSkin activeSkin;
    Transform activeBone;
    Vector3 lastMousePosition;
    Vector3 dragStartMouseWorld;
    Vector3 dragStartBoneWorldPos;
    Quaternion dragStartBoneWorldRot;
    Vector2 dragStartBoneToMouseWorld;

    void Awake()
    {
        CacheParts();
        EnsureCamera();
    }

    void OnEnable()
    {
        CacheParts();
    }

    void Update()
    {
        EnsureCamera();
        if (targetCamera == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (TryBeginDrag())
            {
                LogBone("Down");
                lastMousePosition = Input.mousePosition;
                CacheDragStart();
            }
        }

        if (activeRenderer == null)
            return;

        if (Input.GetMouseButton(0))
        {
            UpdateDrag();
            if (logDragEveryFrame || Input.mousePosition != lastMousePosition)
                LogBone("Drag");
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            LogBone("Up");
            EndDrag();
        }
    }

    void CacheParts()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        colliders = GetComponentsInChildren<Collider2D>(true);
        skinByRenderer.Clear();

        var skins = GetComponentsInChildren<SpriteSkin>(true);
        foreach (var skin in skins)
        {
            if (skin == null)
                continue;
            var renderer = skin.GetComponent<SpriteRenderer>();
            if (renderer != null)
                skinByRenderer[renderer] = skin;
        }
    }

    void EnsureCamera()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    bool TryBeginDrag()
    {
        var worldPoint = ScreenToWorld(Input.mousePosition);
        var renderer = PickRenderer(worldPoint);
        if (renderer == null)
            return false;
        foreach (var trans in UnDragableNodes)
        {
            if (trans == renderer.transform)
            {
                return false;
            }
        }
        activeRenderer = renderer;
        activeBone = ResolveBone(renderer, out activeSkin);
        return true;
    }

    void EndDrag()
    {
        activeRenderer = null;
        activeSkin = null;
        activeBone = null;
    }

    void CacheDragStart()
    {
        dragStartMouseWorld = ScreenToWorld(Input.mousePosition);

        if (activeBone == null)
            return;

        dragStartBoneWorldPos = activeBone.position;
        dragStartBoneWorldRot = activeBone.rotation;
        dragStartBoneToMouseWorld = (Vector2)(dragStartMouseWorld - dragStartBoneWorldPos);
        if (dragStartBoneToMouseWorld.sqrMagnitude < 0.0001f)
            dragStartBoneToMouseWorld = Vector2.right;
    }

    void UpdateDrag()
    {
        if (activeBone == null)
            return;

        var currentMouseWorld = ScreenToWorld(Input.mousePosition);

        if (applyTranslation)
        {
            var delta = currentMouseWorld - dragStartMouseWorld;
            activeBone.position = dragStartBoneWorldPos + delta;
        }

        if (applyRotation)
        {
            var currentVec = (Vector2)(currentMouseWorld - dragStartBoneWorldPos);
            if (currentVec.sqrMagnitude >= 0.0001f)
            {
                var angle = Vector2.SignedAngle(dragStartBoneToMouseWorld, currentVec);
                activeBone.rotation = dragStartBoneWorldRot * Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    Vector3 ScreenToWorld(Vector3 screenPoint)
    {
        var world = targetCamera.ScreenToWorldPoint(screenPoint);
        world.z = transform.position.z;
        return world;
    }

    SpriteRenderer PickRenderer(Vector3 worldPoint)
    {
        var best = (SpriteRenderer)null;
        var bestLayer = int.MinValue;
        var bestOrder = int.MinValue;

        if (colliders != null && colliders.Length > 0)
        {
            var hits = Physics2D.OverlapPointAll(worldPoint);
            if (hits != null && hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit == null)
                        continue;
                    if (!hit.transform.IsChildOf(transform))
                        continue;

                    var renderer = hit.GetComponent<SpriteRenderer>();
                    if (renderer == null)
                        renderer = hit.GetComponentInParent<SpriteRenderer>();
                    if (renderer == null || !renderer.transform.IsChildOf(transform))
                        continue;
                    if (!IsRendererPickable(renderer, worldPoint))
                        continue;

                    if (IsHigherPriority(renderer, best, ref bestLayer, ref bestOrder))
                        best = renderer;
                }
            }
        }

        if (best != null)
            return best;

        if (spriteRenderers == null)
            return null;

        foreach (var renderer in spriteRenderers)
        {
            if (!IsRendererPickable(renderer, worldPoint))
                continue;
            if (IsHigherPriority(renderer, best, ref bestLayer, ref bestOrder))
                best = renderer;
        }

        return best;
    }

    bool IsRendererPickable(SpriteRenderer renderer, Vector3 worldPoint)
    {
        if (renderer == null || !renderer.enabled || renderer.sprite == null)
            return false;
        return renderer.bounds.Contains(worldPoint);
    }

    static bool IsHigherPriority(SpriteRenderer candidate, SpriteRenderer current, ref int currentLayer, ref int currentOrder)
    {
        if (candidate == null)
            return false;

        var layer = SortingLayer.GetLayerValueFromID(candidate.sortingLayerID);
        var order = candidate.sortingOrder;
        if (current == null || layer > currentLayer || (layer == currentLayer && order > currentOrder))
        {
            currentLayer = layer;
            currentOrder = order;
            return true;
        }

        return false;
    }

    Transform ResolveBone(SpriteRenderer renderer, out SpriteSkin skin)
    {
        skin = null;
        if (renderer == null)
            return null;

        if (!skinByRenderer.TryGetValue(renderer, out skin))
            skin = renderer.GetComponent<SpriteSkin>();
        if (skin == null) return null;
        else
        {
            var controllingBones = skin.boneTransforms;
            foreach (var bone in controllingBones)
            {
                if (bone != RootBone)
                {
                    return bone;
                }
            }
        }
        return null;
        /*if (skin.rootBone != null)
            return skin.rootBone;

        var bones = skin.boneTransforms;
        if (bones == null || bones.Length == 0)
            return null;

        for (var i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
                return bones[i];
        }

        return null;*/
    }

    void LogBone(string phase)
    {
        var partName = activeRenderer != null ? activeRenderer.name : "(none)";
        var boneName = activeBone != null ? activeBone.name : "(none)";
        Debug.Log($"DragableDog {phase}: part={partName}, bone={boneName}", this);
    }
}
