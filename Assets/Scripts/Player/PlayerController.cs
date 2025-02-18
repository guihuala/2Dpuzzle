using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;          // 普通移动速度
    public float runSpeed = 10f;          // 奔跑速度
    public float climbSpeed = 3f;         // 攀爬速度
    public float groundCheckDistance = 0.2f; // 地面检测的距离
    public LayerMask groundLayer;         // 地面层

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 moveDirection;
    private bool isClimbing = false;
    
    [Header("音频")]
    [SerializeField] private RandomAudioPlayer walkAudioPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        
        // 处理角色的移动
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector2(horizontal, vertical).normalized;

        if (!isClimbing)
        {
            Vector2 targetVelocity = moveDirection * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed);
            rb.velocity = new Vector2(targetVelocity.x, rb.velocity.y);
            
            if (moveDirection.magnitude > 0.1f) // 如果有明显的移动
            {
                walkAudioPlayer.PlayRandomSound();
            }
        }
        else if (isClimbing)
        {
            float verticalClimb = Input.GetAxis("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, verticalClimb * climbSpeed); // 只修改垂直方向的速度
        }
    }

    // 检测与交互物体的碰撞
    void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 3f);
            if (hit.collider != null && hit.collider.CompareTag("Interactable"))
            {
                Debug.Log("与 " + hit.collider.name + " 进行交互");
                // 调用交互的逻辑
            }
        }
    }

    // 攀爬检测
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Climbable"))
        {
            isClimbing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Climbable"))
        {
            isClimbing = false;
        }
    }
}
