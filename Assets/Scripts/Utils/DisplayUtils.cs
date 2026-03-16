using UnityEngine;

public static class DisplayUtils
{
    public static Vector2 WorldTransform2ScreenPos(Transform from, Camera cam)
    {
        if (from == null)
        {
            Debug.LogError("DisplayUtils.WorldTransform2ScreenPos: from is null.");
            return Vector2.zero;
        }

        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            Debug.LogError("DisplayUtils.WorldTransform2ScreenPos: camera is null.");
            return Vector2.zero;
        }

        Vector3 screenPos = cam.WorldToScreenPoint(from.position);
        return new Vector2(screenPos.x, screenPos.y);
    }
}
