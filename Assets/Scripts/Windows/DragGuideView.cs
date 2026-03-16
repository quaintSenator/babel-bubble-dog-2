using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DragGuideView : MonoBehaviour
{
    // Start is called before the first frame update
    [FormerlySerializedAs("TweenPosition")] public TweenPosition tweenPosition;

    public DragGuideView(Vector3 from, Vector3 to)
    {
        if (tweenPosition != null)
        {
            tweenPosition.from = from;
            tweenPosition.to = to;
        }
    }
    void Awake()
    {
        
    }
    void OnEnable()
    {
        tweenPosition.enabled = true;
        tweenPosition.ResetToBeginning();
        tweenPosition.PlayForward();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
