using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionOperationPanel : MonoBehaviour
{
    public GameObject leafCopy;
    public List<GameObject> leaves = new List<GameObject>();
    private void Awake()
    {
        leaves.Clear();
        for (int i = 0; i < 6; i++)
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
        foreach (var leaf in leaves)
        {
            leaf.SetActive(false);
        }
        PlayExpandingAnim();
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
        for (int i = 0; i < leaves.Count; i++)
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
                    actionPanelLeaf.ActiveButtonScale(false);
                }
            }
            yield return new WaitForSeconds(0.08f);
        }
    }
}
