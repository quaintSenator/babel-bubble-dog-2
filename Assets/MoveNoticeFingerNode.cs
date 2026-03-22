using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNoticeFingerNode : MonoBehaviour
{
    public void SetPointAt(bool isAtRight)
    {
        transform.rotation = Quaternion.Euler(0, 0, isAtRight ? 90f : -90f);
    }
}
