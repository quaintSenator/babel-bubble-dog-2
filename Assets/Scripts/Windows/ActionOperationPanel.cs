using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionOperationPanel : BaseWindow
{
    public GameObject leafCopy;
    public List<GameObject> leaves = new List<GameObject>();
    private const int MAX_LEAF_COUNT = 6;
    private int visibleLeavesCount = 0;
    
    private void Awake()
    {
        leaves.Clear();
        for (int i = 0; i < MAX_LEAF_COUNT; i++)
        {
            GameObject leafInstance = Instantiate(leafCopy, transform);
            leafInstance.name = "Leaf " + i;
            AdjustAngle(leafInstance, i);
            leaves.Add(leafInstance);
        }
        leafCopy.SetActive(false);
    }

    private void OnEnable()
    {
        var unlockedTypes = Models.behave.GetAllUnlockedBehaveTypes();
        for(var i = 0; i < leaves.Count; i++)
        {
            var leafGo = leaves[i];
            leafGo.SetActive(false);
            var leaf = leafGo.GetComponent<ActionPanelLeaf>();
            if (leaf != null && i < unlockedTypes.Count)
            {
                leaf.SetBehaviourIconByType(unlockedTypes[i]);
            }
        }
        visibleLeavesCount = unlockedTypes.Count;
        PlayExpandingAnim();
    }
    
    public override void Open(GameObject wakerObject = null)
    {
        var player = FindObjectOfType<Player>();
        if (player != null && player.reactShowPoint != null)
        {
            transform.position = player.reactShowPoint.transform.position;
            return;
        }

        if (wakerObject == null)
        {
            return;
        }

        var area = wakerObject.GetComponent<ReactableObject>();
        if (area != null)
        {
            var reactPoint = area.reactShowingPoint;
            if (reactPoint != null)
            {
                transform.position = reactPoint.transform.position;
            }
        }
    }

    
    void AdjustAngle(GameObject leafInstance, int i)
    {
        var offset = leafInstance.transform.GetChild(1);
        var angle = 180 - i * 60;
        offset.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void PlayExpandingAnim()
    {
        StartCoroutine(PlayExpandingAnimCoroutine());
    }

    IEnumerator PlayExpandingAnimCoroutine()
    {
        for (int i = 0; i < visibleLeavesCount; i++)
        {
            GameObject leaf = leaves[i];
            if (leaf != null)
            {
                leaf.SetActive(true);
                Animation leafAnimation = leaf.GetComponent<Animation>();
                if (leafAnimation != null)
                {
                    ActionPanelLeaf actionPanelLeaf = leaf.GetComponent<ActionPanelLeaf>();
                    leafAnimation.Play();
                    actionPanelLeaf.SetButtonScaleActive(false);
                }
            }
            yield return new WaitForSeconds(0.08f);
        }
    }
}
