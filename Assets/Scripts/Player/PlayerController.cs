using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("玩家移动基本参数")]
    public float moveSpeed = 5f;          // 普通移动速度
    public float runSpeed = 10f;          // 奔跑速度
    public float climbSpeed = 3f;         // 攀爬速度
    public float ladderHorizontalSpeed = 5f; // 在梯子上的水平移动速度
    public float groundCheckDistance = 0.2f; // 地面检测的距离
    public LayerMask groundLayer;         // 地面层
    public float gravityScale = 2f;       // 重力缩放值

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 moveDirection;
    private bool isClimbing = false;
    private bool isNearLadder = false;    // 是否在梯子附近
    private Animator animator;             // 动画控制器

    [Header("交互UI提示")]
    private Interactable _currentBaseInteractableObject;  // 当前可交互物体

    [Header("音频")]
    [SerializeField] private RandomAudioPlayer walkAudioPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameInput.Instance.OnInteractAction += InstanceOnInteract;
        GameInput.Instance.OnOpenBag += InstanceOnOnOpenBag;
    }

    private void InstanceOnOnOpenBag(object sender, EventArgs e)
    {
        Debug.Log("open bag");
    }
    
    void InstanceOnInteract(object sender, EventArgs e)
    {
        if (_currentBaseInteractableObject != null)
        {
            _currentBaseInteractableObject.Interact();
        }
    }

    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        // 处理角色的移动
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector2 moveVector = GameInput.Instance.GetMovementVectorNormalized();
        float horizontal = moveVector.x;
        float vertical = moveVector.y;

        moveDirection = new Vector2(horizontal, vertical).normalized;
        
        if (isNearLadder) // 玩家在梯子附近并按下向上时
        {
            isClimbing = true;
        }
        else if (!isNearLadder || isGrounded) // 离开梯子或站在地面时，退出爬梯子状态
        {
            isClimbing = false;
        }
        
        // 如果在梯子上，执行爬梯子逻辑
        if (isClimbing)
        {
            // 设置垂直方向的速度，垂直输入控制上下移动
            float verticalVelocity = vertical != 0 ? vertical * climbSpeed : 0;

            // 设置水平方向的速度，在梯子上水平移动时使用较慢的速度
            float horizontalVelocity = horizontal * ladderHorizontalSpeed;

            // 禁止重力
            rb.velocity = new Vector2(horizontalVelocity, verticalVelocity);
            rb.gravityScale = 0f;

            // 更新动画状态
            if (animator != null)
            {
                animator.SetBool("isClimbing", true);
                animator.SetFloat("climbSpeed", Mathf.Abs(vertical)); // 设置动画中的爬梯速度
            }
            
            if (horizontal != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(horizontal), 1, 1);
            }
        }
        else // 非梯子状态下的正常移动逻辑
        {
            float currentMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;
            Vector2 targetVelocity = new Vector2(horizontal * currentMoveSpeed, rb.velocity.y);
            rb.velocity = targetVelocity;

            rb.gravityScale = gravityScale; // 恢复正常重力

            // 更新动画状态：如果玩家正在走
            if (animator != null)
            {
                animator.SetBool("isClimbing", false); // 停止爬梯子动画
                animator.SetFloat("speed", Mathf.Abs(horizontal)); // 设置走路动画速度
            }
        }
        
        if (moveDirection.magnitude > 0.1f && isGrounded)
        {
            walkAudioPlayer.PlayRandomSound();
        }
        
        if (!isClimbing && horizontal != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontal), 1, 1);
        }
    }

    public void SetNearLadder(bool isNear)
    {
        isNearLadder = isNear;
        if (!isNear)
        {
            isClimbing = false;
        }
    }


    // 检测与交互物体的碰撞
    private void OnTriggerStay2D(Collider2D other)
    {
        Interactable baseInteractableObject = other.GetComponent<Interactable>();
        if (baseInteractableObject != null)
        {
            _currentBaseInteractableObject = baseInteractableObject;
            _currentBaseInteractableObject.Enter();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Interactable baseInteractableObject = other.GetComponent<Interactable>();
        if (baseInteractableObject != null)
        {
            _currentBaseInteractableObject.Exit();
            _currentBaseInteractableObject = null;
        }
    }
}
