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

    public override void StartGuide()
    {
        var springObj = GuideTriggerManager.FindInLevelObjects(LayerMask.NameToLayer("LevelObject"), "Spring");
        if (springObj != null)
        {
            var reactable = springObj.GetComponent<ReactableObject>();
            if (reactable != null)
            {
                WindowManager.OpenWindow("GuideWindow", springObj);
                
            }
        }
    }
}
