using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class FirstEnterSpringGuide : BaseGuide
{
    public override bool checkGuide()
    {
        return !GuideTriggerManager.everFirstEnterSpring;
    }

    protected void Step1(GameObject go)
    {
        /*var springObj = GuideTriggerManager.FindInLevelObjects(LayerMask.NameToLayer("LevelObject"), "Spring");
        if (springObj != null)
        {
            var reactable = springObj.GetComponent<ReactableObject>();
            if (reactable != null)
            {
                WindowManager.OpenWindow("GuideWindow", springObj);
            }
        }*/
        Debug.Log("FirstEnterSpringGuide Step1");
    }

    protected void Step2(GameObject go)
    {
        Debug.Log("FirstEnterSpringGuide Step2");
    }
}
