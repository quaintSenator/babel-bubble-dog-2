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

    public override void RegisterAllEvents()
    {
        EventManager.Register<GameObject>(EventKey.GuideWindowNextStep, OnNextStep);
    }

    public void OnNextStep(GameObject go)
    {
        RunNextGuideStep();
    }

    protected void Step1(GameObject go)//显示左右移动icon
    {
        CommonStepWalkToThing(go, "Bush");
    }
    
    protected void Step2(GameObject go)
    {
       Debug.Log("sfsfsf");
    }

    protected void Step3(GameObject go)
    {
        if (_guideWindow != null)
        {
            var behLearnWindow = WindowManager.OpenWindow("BehaviourLearnWindow", go);
        }
        Debug.Log("FirstEnterSpringGuide Step2");
    }
}
