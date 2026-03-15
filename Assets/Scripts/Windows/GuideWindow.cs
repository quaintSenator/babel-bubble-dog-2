using Unity.VisualScripting;
using UnityEngine;

public class GuideWindow : BaseWindow
{
    public UITexture guideFrame;
    //GuideWindow的waker是ReactableObject
    public override void Open(GameObject wakerObject = null)
    {
        if (wakerObject != null)
        {
            var reactableObj = wakerObject.GetComponent<ReactableObject>();
            if (reactableObj != null)
            {
                var collider = reactableObj.GetComponent<BoxCollider2D>();
                if (collider != null)
                {
                    PrepareGuideFrame(collider);
                }
            }
        }
    }

    void PrepareGuideFrame(BoxCollider2D col)
    {
        var worldCamera = Camera.main;
        var uiCamera = UICamera.mainCamera;
        if (worldCamera == null || uiCamera == null)
        {
            return;
        }

        // Use world-space bounds to account for scale, offset, and parent transforms
        var bounds = col.bounds;
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
    }

    void PrepareGuideFrameClick()
    {
        
    }
}
