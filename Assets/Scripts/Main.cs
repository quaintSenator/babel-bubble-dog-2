using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Main : MonoBehaviour
{
    private void Start()
    {
        #if UNITY_EDITOR
        GameObject keyDownTestObject = new GameObject();
        keyDownTestObject.name = "KeyDownTest Object";
        keyDownTestObject.AddComponent<KeyDownTest>();
        DontDestroyOnLoad(keyDownTestObject);
        GuideManager.StartGuide(GuideID.FirstTimeEnteringSpring);
#endif
    }

    private void Update()
    {
        
    }
}
