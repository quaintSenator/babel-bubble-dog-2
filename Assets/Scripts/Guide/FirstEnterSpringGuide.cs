using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class FirstEnterSpringGuide : BaseGuide
{
    private GuideWindow _window;
    public override bool checkGuide()
    {
        return !GuideTriggerManager.everFirstEnterSpring;
    }

    public override void RegisterAllEvents()
    {
        EventManager.Register<GameObject>(EventKey.GuideWindowNextStep, OnNextStep);
    }

    public void OnNextStep(GameObject go)
    {
        RunNextGuideStep();
    }

    protected void Step1(GameObject go) //框选Spring
    {
        Debug.Log("FirstEnterSpringGuide Step1");
        var springObj = GuideTriggerManager.FindInLevelObjects(LayerMask.NameToLayer("LevelObject"), "Spring");
        if (springObj != null)
        {
            var reactable = springObj.GetComponent<ReactableObject>();
            if (reactable != null)
            {
                _window = WindowManager.OpenWindow("GuideWindow", springObj) as GuideWindow;
                _window.PrepareGuideFrame();
            }
        }
    }

    protected void Step2(GameObject go)
    {
        if (_window != null)
        {
            var behLearnWindow = WindowManager.OpenWindow("BehaviourLearnWindow", go);
        }
        Debug.Log("FirstEnterSpringGuide Step2");
    }
}
