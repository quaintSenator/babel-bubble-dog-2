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
    public static event Action<GameObject> PlayerHitReactableObject
    {
        add => EventManager.Register(EventKey.PlayerHitReactableObject, value);
        remove => EventManager.Unregister(EventKey.PlayerHitReactableObject, value);
    }

    public static event Action PlayerLeaveReactableObject
    {
        add => EventManager.Register(EventKey.PlayerLeaveReactableObject, value);
        remove => EventManager.Unregister(EventKey.PlayerLeaveReactableObject, value);
    }
    
    Animator animator;
    Animation rotateAnim;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpMaxHeight = 2f;
    InputHoldingState holdingState;
    InputHoldingState curHoldingState;
    [SerializeField]  public SpriteRenderer rootSprite;
    bool moving = false;
    private bool holdingJump = false;
    [SerializeField] public bool isAtGround = true;
    private float verticalVelocity = 0f;
    private const float JumpGravity = 20f;
    private bool isGrounded = false;
    private bool lockMovement = false;
    private bool lockYMovement = false;
    
    public Transform RootBone;
    private const int LeftIdelDogEndRotateY = 0;
    private const int RightIdelDogEndRotateY = 180;
    Collider2D myCollider;
    private int curXMovement;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        holdingState = InputHoldingState.Idle;
        rotateAnim = GetComponent<Animation>();
        myCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        TickPlayerInput();
        TickAnimStateMove();
        TickPositionX();
        TickPositionY();
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

        if (Input.GetKey(KeyCode.Space))
        {
            holdingJump = true;
        }
        else
        {
            holdingJump = false;  
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

    private void TickPositionX()
    {
        if (moving && !lockMovement)
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

    void TickPositionY()
    {
        if (lockYMovement)
        {
            return;
        }
        var msk = LayerMask.GetMask("Ground");

        Vector2 center = myCollider.bounds.center;
        Vector2 size = myCollider.bounds.size;
        float angle = transform.eulerAngles.z;

        Collider2D hit = Physics2D.OverlapBox(center, size, angle, msk);
        isAtGround = hit != null && hit != myCollider;
        
        if (holdingJump && isAtGround)
        {
            verticalVelocity = Mathf.Sqrt(2f * JumpGravity * Mathf.Max(0f, jumpMaxHeight));
            isAtGround = false;
        }

        if (!isAtGround || verticalVelocity > 0f)
        {
            verticalVelocity -= JumpGravity * Time.deltaTime;
            transform.position += Vector3.up * (verticalVelocity * Time.deltaTime);
        }
        else
        {
            verticalVelocity = 0f;
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

    #region 碰撞处理

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enteringArea = other.gameObject.GetComponent<ReactableObject>();
        if (enteringArea != null) //如果是可交互对象
        {
            EventManager.Dispatch(EventKey.PlayerHitReactableObject, enteringArea.gameObject);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        var enteringArea = other.gameObject.GetComponent<ReactableObject>();
        if (enteringArea != null)
        {
            EventManager.Dispatch(EventKey.PlayerLeaveReactableObject, enteringArea.gameObject);
        }
    }
    #endregion

    #region 移动处理

    public void SetLockMove(bool lockMove)
    {
        lockMovement = lockMove;
    }

    public bool GetLockMove()
    {
        return lockMovement;
    }

    public void SetLockYMove(bool lockYMove)
    {
        lockYMovement = lockYMove;
    }

    public void FreezePlayer(bool isFreeze)
    {
        SetLockMove(!isFreeze);
        SetLockYMove(!isFreeze);
    }

    #endregion
}
