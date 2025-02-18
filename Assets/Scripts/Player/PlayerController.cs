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
    public float groundCheckDistance = 0.2f; // 地面检测的距离
    public LayerMask groundLayer;         // 地面层

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 moveDirection;
    private bool isClimbing = false;

    [Header("交互UI提示")]
    public GameObject interactionUI;      // 交互提示UI
    private InteractableObject currentInteractableObject;  // 当前可交互物体

    [Header("音频")]
    [SerializeField] private RandomAudioPlayer walkAudioPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);  // 初始时隐藏UI
        }
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

        // 横向移动逻辑
        Vector2 targetVelocity = moveDirection * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed);
        rb.velocity = new Vector2(targetVelocity.x, rb.velocity.y);

        if (moveDirection.magnitude > 0.1f) // 如果有明显的移动
        {
            walkAudioPlayer.PlayRandomSound();
        }

        // 攀爬
        if (isClimbing)
        {
            float verticalClimb = Input.GetAxis("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, verticalClimb * climbSpeed); // 只修改垂直方向的速度
        }
    }

    public void ToggleClimbing(bool isclimbing)
    {
        isClimbing = isclimbing;
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
            ShowInteractionUI(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        InteractableObject interactableObject = other.GetComponent<InteractableObject>();
        if (interactableObject != null)
        {
            currentInteractableObject.Exit();
            currentInteractableObject = null;
            ShowInteractionUI(false);
        }
    }

    // 显示或隐藏交互提示UI
    void ShowInteractionUI(bool show)
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(show);
        }
    }
}
