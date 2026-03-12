using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum InputHoldingState
{
    Idle,
    WalkingLeft,
    WalkingRight,
}
public class Player : MonoBehaviour
{
    public static event Action<GameObject> PlayerHitReactableObject;
    public static event Action PlayerLeaveReactableObject;
    
    Animator animator;
    Animation rotateAnim;
    [SerializeField] private float speed = 5f;
    InputHoldingState holdingState;
    InputHoldingState curHoldingState;
    [SerializeField]  public SpriteRenderer rootSprite;
    bool moving = false;

    public Transform RootBone;
    private const int LeftIdelDogEndRotateY = 0;
    private const int RightIdelDogEndRotateY = 180;

    private int curXMovement;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        holdingState = InputHoldingState.Idle;
        rotateAnim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        TickPlayerInput();
        TickAnimStateMove();
        TickPosition();
        //TickAnimStateRotate(); 搁置 翻转没找到很好的办法 bug解决不了
        holdingState = curHoldingState;
    }

    private void TickPlayerInput()
    {
        curHoldingState = InputHoldingState.Idle;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (holdingState != InputHoldingState.WalkingRight)
            {
                curHoldingState = InputHoldingState.WalkingLeft;
            }
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (holdingState != InputHoldingState.WalkingLeft)
            {
                curHoldingState = InputHoldingState.WalkingRight;
            }
        }
        else
        {
            curHoldingState = InputHoldingState.Idle;
        }
        
    }

    private void TickAnimStateMove()
    {
        if (curHoldingState != holdingState)
        {
            if (curHoldingState != InputHoldingState.Idle)
            {
                move();
            }
            else
            {
                stopMove();
            }
        }
    }

    private void TickPosition()
    {
        if (moving)
        {
            curXMovement = 0;
            animator.SetBool("moving", true);
            if (curHoldingState == InputHoldingState.WalkingRight)
            {
                curXMovement = 1;
            }
            else if (curHoldingState == InputHoldingState.WalkingLeft)
            {
                curXMovement = -1;
            }
            if (Mathf.Abs(curXMovement) > Mathf.Epsilon && moving)
            {
                transform.Translate(Vector3.right * (curXMovement * speed * Time.deltaTime), Space.World);
            }

            if (curXMovement < 0)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    void TickAnimStateRotate()
    {
        if (curHoldingState != holdingState)
        {
            if (curHoldingState != InputHoldingState.Idle)
            {
                //如果开始按下了一侧移动，摆头
                if (curHoldingState == InputHoldingState.WalkingLeft)
                {
                    if (MathF.Abs(RootBone.localRotation.eulerAngles.y - LeftIdelDogEndRotateY) < 1)
                    {
                        //如果开始左移，但头在左侧末端位置，不改
                    }
                    else
                    {
                        animator.ResetTrigger("rev");
                        animator.SetTrigger("rev");
                    }
                }
                else
                {
                    if (MathF.Abs(RootBone.localRotation.eulerAngles.y - RightIdelDogEndRotateY) < 1)
                    {
                        //如果开始右移，但头在右侧末端位置，不改
                    }
                    else
                    {
                        animator.ResetTrigger("rotate");
                        animator.SetTrigger("rotate");
                    }
                }
            }
            else
            {
                stopMove();
            }
        }
    }

    private void move()
    {
        animator.SetBool("moving", true);
        moving = true;
    }

    private void stopMove()
    {
        animator.SetBool("moving", false);
        moving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var enteringArea = other.gameObject.GetComponent<ReactableArea>();
        if (enteringArea != null)
        {
            PlayerHitReactableObject?.Invoke(enteringArea.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var enteringArea = other.gameObject.GetComponent<ReactableArea>();
        if (enteringArea != null)
        {
            PlayerLeaveReactableObject?.Invoke();
        }
    }
    
    
}
