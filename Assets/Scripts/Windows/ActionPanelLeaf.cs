using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPanelLeaf : MonoBehaviour
{
    public UIButtonScale m_ButtonScale;
    public UITexture m_actionIcon;
    public Transform m_actionIconTransform;
    public Transform m_offsetTransform;
    public Transform m_bgTransform;
    

    public void SetButtonScaleActive(bool active)
    {
        m_ButtonScale.enabled = active;
    }
    public void OnExpandingAnimEnd()
    {
        SetButtonScaleActive(true);
    }

    public void SetBehaviourIconByType(BehaveType behaviour)
    {
        if (m_actionIcon == null)
        {
            return;
        }
        Texture2D iconTexture = Models.behave.GetBehaviourIconTexture(behaviour);
        m_actionIcon.mainTexture = iconTexture;
    }
}
