using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // 引用UI命名空间

public class PlayerController : MonoBehaviour
{
    [Header("玩家移动基本参数")]
    public float moveSpeed = 5f;          // 普通移动速度
    public float runSpeed = 10f;          // 奔跑速度
    public float climbSpeed = 3f;         // 攀爬速度
    public float ladderHorizontalSpeed = 2f; // 在梯子上的水平移动速度
    public float groundCheckDistance = 0.2f; // 地面检测的距离
    public LayerMask groundLayer;         // 地面层
    public float gravityScale = 1f;       // 重力缩放值

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 moveDirection;
    private bool isClimbing = false;
    private bool isNearLadder = false;    // 是否在梯子附近
    private Animator animator;             // 动画控制器

    [Header("交互UI提示")]
    private InteractableObject currentInteractableObject;  // 当前可交互物体

    [Header("音频")]
    [SerializeField] private RandomAudioPlayer walkAudioPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        // 处理角色的移动
        HandleMovement();

        Interact();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector2(horizontal, vertical).normalized;

        // 检查是否可以爬梯子
        if (isNearLadder && vertical > 0)
        {
            isClimbing = true;
        }
        else if (!isNearLadder || isGrounded) // 在离开梯子或接触地面时退出爬梯子状态
        {
            isClimbing = false;
        }

        // 如果在梯子上
        if (isClimbing)
        {
            // 当有垂直输入时才改变垂直速度，否则保持静止
            float verticalVelocity = vertical != 0 ? vertical * climbSpeed : 0;

            // 允许在梯子上进行水平移动，使用较慢的速度
            float horizontalVelocity = horizontal * ladderHorizontalSpeed;

            rb.velocity = new Vector2(horizontalVelocity, verticalVelocity);
            rb.gravityScale = 0f; // 禁用重力

            // 更新动画状态
            if (animator != null)
            {
                animator.SetBool("isClimbing", true);
                animator.SetFloat("climbSpeed", Mathf.Abs(vertical));
            }

            // 在梯子上移动时也更新朝向
            if (horizontal != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(horizontal), 1, 1);
            }
        }
        else
        {
            // 正常移动逻辑
            Vector2 targetVelocity = new Vector2(horizontal * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed), rb.velocity.y);
            rb.velocity = targetVelocity;
            rb.gravityScale = gravityScale; // 恢复正常重力

            // 更新动画状态
            if (animator != null)
            {
                animator.SetBool("isClimbing", false);
                animator.SetFloat("speed", Mathf.Abs(horizontal));
            }
        }

        if (moveDirection.magnitude > 0.1f && isGrounded) // 如果有明显的移动且在地面上
        {
            walkAudioPlayer.PlayRandomSound();
        }

        // 更新朝向（仅在非爬梯子状态）
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

    void Interact()
    {
        if (currentInteractableObject != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractableObject.Interact();
        }
    }

    // 检测与交互物体的碰撞
    private void OnTriggerStay2D(Collider2D other)
    {
        InteractableObject interactableObject = other.GetComponent<InteractableObject>();
        if (interactableObject != null)
        {
            currentInteractableObject = interactableObject;
            currentInteractableObject.Enter();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InteractableObject interactableObject = other.GetComponent<InteractableObject>();
        if (interactableObject != null)
        {
            currentInteractableObject.Exit();
            currentInteractableObject = null;
        }
    }
}
