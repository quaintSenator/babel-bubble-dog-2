using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class BaseGuide
{
    public BaseGuide()
    {
        if (checkGuide())
        {
            StartGuide();
        }
    }
    public virtual bool checkGuide() //覆写，检查这个guide自己的各种条件能不能启动
    {
        return true;
    }

    public virtual void StartGuide()
    {
        
    }
}
