using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class BaseGuide
{
    private readonly List<Action<GameObject>> guideSteps = new List<Action<GameObject>>();
    private bool stepsInitialized;
    private int currentStepIndex = -1;

    protected List<Action<GameObject>> GuideSteps => guideSteps;
    
    public virtual bool checkGuide() //覆写，检查这个guide自己的各种条件能不能启动
    {
        return true;
    }

    public virtual void StartGuide()
    {
        StartGuideSteps();
    }

    protected void StartGuideSteps(GameObject context = null)
    {
        EnsureGuideStepsInitialized();
        currentStepIndex = -1;
        RunNextGuideStep(context);
    }

    protected void RunNextGuideStep(GameObject context = null)
    {
        EnsureGuideStepsInitialized();
        var nextIndex = currentStepIndex + 1;
        if (nextIndex < 0 || nextIndex >= guideSteps.Count)
        {
            return;
        }

        currentStepIndex = nextIndex;
        var step = guideSteps[currentStepIndex];
        if (step != null)
        {
            step(context);
        }
    }

    private void EnsureGuideStepsInitialized()
    {
        if (stepsInitialized)
        {
            return;
        }

        stepsInitialized = true;
        guideSteps.Clear();

        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var stepMethods = new List<(int index, MethodInfo method)>();
        foreach (var method in methods)
        {
            if (!method.Name.StartsWith("Step", StringComparison.Ordinal))
            {
                continue;
            }

            var suffix = method.Name.Substring("Step".Length);
            if (string.IsNullOrWhiteSpace(suffix))
            {
                continue;
            }

            if (!int.TryParse(suffix, out var index) || index <= 0)
            {
                continue;
            }

            if (method.ReturnType != typeof(void))
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(GameObject))
            {
                continue;
            }

            stepMethods.Add((index, method));
        }

        stepMethods.Sort((left, right) => left.index.CompareTo(right.index));

        for (var i = 0; i < stepMethods.Count; i++)
        {
            var handler = (Action<GameObject>)Delegate.CreateDelegate(
                typeof(Action<GameObject>),
                this,
                stepMethods[i].method,
                false);

            if (handler == null)
            {
                continue;
            }

            guideSteps.Add(context =>
            {
                handler(context);
                //RunNextGuideStep(context); //不要自动下一步
            });
        }
    }
}
