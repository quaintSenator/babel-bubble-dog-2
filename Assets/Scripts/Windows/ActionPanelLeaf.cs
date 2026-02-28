using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPanelLeaf : MonoBehaviour
{
    public UIButtonScale m_ButtonScale;

    public void ActiveButtonScale(bool active)
    {
        m_ButtonScale.enabled = active;
    }
}
