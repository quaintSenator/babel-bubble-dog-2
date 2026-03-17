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
    public static bool IsInZRange(float i, float c, float l, float r)
    {
        float left = Mathf.Repeat(c + l, 360f);
        float right = Mathf.Repeat(c + r, 360f);
        float angle = Mathf.Repeat(i, 360f);

        if (left <= right)
            return angle >= left && angle <= right;   // 不跨 0
        else
            return angle >= left || angle <= right;   // 跨 0
    }
}
