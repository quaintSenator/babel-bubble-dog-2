using Unity.VisualScripting;
using UnityEngine;

public class GuideWindow : BaseWindow
{
    public UITexture guideFrame;
    public UITexture blackMask;
    GameObject waker;

    private Material blackHoleMat;
    //GuideWindow的waker是ReactableObject
    public override void Open(GameObject wakerObject = null)
    {
        waker = wakerObject;
        if (blackMask != null)
        {
            blackHoleMat = blackMask.material;
        }
    }

    public void PrepareGuideFrame()
    {
        if (waker != null)
        {
            var reactableObj = waker.GetComponent<ReactableObject>();
            if (reactableObj != null)
            {
                var collider = reactableObj.GetComponent<BoxCollider2D>();
                if (collider != null)
                {
                    PrepareGuideFrame(collider);
                }
            }
            else if (true)
            {
                
            }
        }
    }
    public void PrepareGuideFrame(Bounds bounds)
    {
        var worldCamera = Camera.main;
        var uiCamera = UICamera.mainCamera;
        if (worldCamera == null || uiCamera == null)
        {
            return;
        }

        // Use world-space bounds to account for scale, offset, and parent transforms
        var worldMin = bounds.min;
        var worldMax = bounds.max;

        var screenMin = worldCamera.WorldToScreenPoint(worldMin);
        var screenMax = worldCamera.WorldToScreenPoint(worldMax);

        var parent = guideFrame.transform.parent;
        if (parent == null)
        {
            return;
        }

        var uiDepth = Vector3.Dot(guideFrame.transform.position - uiCamera.transform.position, uiCamera.transform.forward);
        var uiWorldMin = uiCamera.ScreenToWorldPoint(new Vector3(screenMin.x, screenMin.y, uiDepth));
        var uiWorldMax = uiCamera.ScreenToWorldPoint(new Vector3(screenMax.x, screenMax.y, uiDepth));

        var localMin = parent.InverseTransformPoint(uiWorldMin);
        var localMax = parent.InverseTransformPoint(uiWorldMax);

        var localCenter = (localMin + localMax) * 0.5f;
        var width = Mathf.Abs(localMax.x - localMin.x);
        var height = Mathf.Abs(localMax.y - localMin.y);

        guideFrame.transform.localPosition = new Vector3(localCenter.x, localCenter.y, guideFrame.transform.localPosition.z);
        guideFrame.width = Mathf.RoundToInt(width);
        guideFrame.height = Mathf.RoundToInt(height);
        guideFrame.gameObject.SetActive(true);
        
        if (blackHoleMat != null && blackMask != null)
        {
            var maskCorners = blackMask.localCorners;
            var maskSize = maskCorners[2] - maskCorners[0];
            if (maskSize.x > 0.0001f && maskSize.y > 0.0001f)
            {
                var worldMinLocal = parent.TransformPoint(localMin);
                var worldMaxLocal = parent.TransformPoint(localMax);
                var maskLocalMin = blackMask.transform.InverseTransformPoint(worldMinLocal);
                var maskLocalMax = blackMask.transform.InverseTransformPoint(worldMaxLocal);

                var maskLocalCenter = (maskLocalMin + maskLocalMax) * 0.5f;
                var maskLocalWidth = Mathf.Abs(maskLocalMax.x - maskLocalMin.x);
                var maskLocalHeight = Mathf.Abs(maskLocalMax.y - maskLocalMin.y);

                var rectCenterUV = new Vector2(
                    (maskLocalCenter.x - maskCorners[0].x) / maskSize.x,
                    (maskLocalCenter.y - maskCorners[0].y) / maskSize.y
                );
                var rectSizeUV = new Vector2(
                    maskLocalWidth / maskSize.x,
                    maskLocalHeight / maskSize.y
                );

                rectCenterUV.x = Mathf.Clamp01(rectCenterUV.x);
                rectCenterUV.y = Mathf.Clamp01(rectCenterUV.y);
                rectSizeUV.x = Mathf.Clamp01(rectSizeUV.x);
                rectSizeUV.y = Mathf.Clamp01(rectSizeUV.y);

                blackHoleMat.SetVector("_RectCenter", new Vector4(rectCenterUV.x, rectCenterUV.y, 0f, 0f));
                blackHoleMat.SetVector("_RectSize", new Vector4(rectSizeUV.x, rectSizeUV.y, 0f, 0f));
            }
        }
        
        PrepareGuideFrameClick();
    }
    public void PrepareGuideFrame(BoxCollider2D col)
    {
        PrepareGuideFrame(col.bounds);
    }
    public void PrepareGuideFrame(BoxCollider col)
    {
        PrepareGuideFrame(col.bounds);
    }

    void PrepareGuideFrameClick()
    {
        UIEventListener.Get(guideFrame.gameObject).onClick = OnClickGuideFrame;
    }

    void OnClickGuideFrame(GameObject go)
    {
        EventManager.Dispatch(EventKey.GuideWindowNextStep, go);
    }
}
