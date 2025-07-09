using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float airControl = 0.8f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float coyoteTimeForPlatform = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float fallingGravityFactor = 2f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private int maxDashesInAir = 1;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private bool canDashInAir = true;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    [SerializeField] private Transform groundCheck;

    [Header("Motion Blur Effect")]
    [SerializeField] private bool isMotionBlurActive = false;
    [SerializeField] private float blurSpawnRate = 0.05f;
    [SerializeField] private float blurLifetime = 0.3f;
    [SerializeField] private float blurStartAlpha = 0.5f;
    [SerializeField] private float blurEndAlpha = 0f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;
    private float moveInput;
    private bool isFacingRight = false;
    private bool isCrossingPlatform;
    private bool isGrounded;
    private int jumpsLeft;
    private int dashesLeft;
    private float coyoteTimer;
    private float coyoteTimerForPlatform;
    private float jumpBufferTimer;
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector2 lastVelocity;
    private float baseGravity;
    private float blurTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        jumpsLeft = maxJumps;
        dashesLeft = maxDashesInAir;
        baseGravity = rb.gravityScale;
        blurTimer = blurSpawnRate;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ (ï¿½ï¿½ï¿½ OnTriggerEnter2D)
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        isGrounded = isGrounded && (rb.linearVelocity.y <= 0f);
        // ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½, ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
            jumpsLeft = maxJumps;
            dashesLeft = maxDashesInAir;
        }
        else if (!isGrounded && jumpsLeft == maxJumps)
        {
            jumpsLeft--;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½, ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½, ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½, ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ Jump ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Down"))
        {
            coyoteTimerForPlatform = coyoteTimeForPlatform;
        }
        else
        {
            coyoteTimerForPlatform -= Time.deltaTime;
        }

        if (jumpBufferTimer > 0f && (coyoteTimer > 0f || jumpsLeft > 0))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && dashesLeft > 0 && (!isDashing && (canDashInAir || isGrounded)))
        {
            StartDash();
        }
        if (isMotionBlurActive)
        {
            blurTimer -= Time.deltaTime;
            if (blurTimer <= 0f)
            {
                CreateBlurCopy();
                blurTimer = blurSpawnRate;
            }
        }
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            

            if (dashTimer <= 0f)
            {
                isDashing = false;
                isMotionBlurActive = false;
            }
        }

        dashCooldownTimer -= Time.deltaTime; 
        if (coyoteTimerForPlatform > 0f)
        {
            StartCoroutine(DisablePlatformCollision());
        }

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        // ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½
        if (!isDashing)
        {
            Move();
        }
    }

    private void Move()
    {
        float targetSpeed = moveInput * moveSpeed;

        // ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

        // ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        accelRate *= (isGrounded ? 1f : airControl);
        float movement = speedDif * accelRate;
        rb.AddForce(movement * Vector2.right);

        // ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½, ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½, ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½, ï¿½ï¿½ ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½
        if (moveInput != 0)
        {
            isFacingRight = moveInput > 0;
            spriteRenderer.flipX = isFacingRight;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpsLeft--;
        coyoteTimer = 0f;
        jumpBufferTimer = 0f;
    }

    private void StartDash()
    {
        dashesLeft--;
        isDashing = true;
        dashTimer = dashTime;
        dashCooldownTimer = dashCooldown;
        lastVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector2((isFacingRight ? 1 : -1) * dashSpeed, 0f);
        rb.gravityScale = 0f;
        isMotionBlurActive = true;
    }

    private void CreateBlurCopy()
    {
        GameObject blurCopy = new GameObject("BlurCopy");
        blurCopy.transform.position = transform.position;
        blurCopy.transform.rotation = transform.rotation;
        blurCopy.transform.localScale = transform.localScale;

        SpriteRenderer blurRenderer = blurCopy.AddComponent<SpriteRenderer>();
        blurRenderer.sprite = spriteRenderer.sprite;
        blurRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        blurRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
        blurRenderer.color = new Color(1f, 1f, 1f, blurStartAlpha); // ����� ��������������
        blurRenderer.flipX = spriteRenderer.flipX; // �������������� ����� �� ��� X

        StartCoroutine(FadeOutBlur(blurCopy, blurRenderer));
    }

    private IEnumerator FadeOutBlur(GameObject blurCopy, SpriteRenderer blurRenderer)
    {
        float elapsedTime = 0f;
        Color startColor = blurRenderer.color;
        Color endColor = new Color(1f, 1f, 1f, blurEndAlpha);

        while (elapsedTime < blurLifetime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / blurLifetime;
            blurRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        Destroy(blurCopy);
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", !isGrounded && rb.linearVelocity.y > 0.05f);
        animator.SetBool("IsFalling", !isGrounded && rb.linearVelocity.y < 0f);
    }

    private void LateUpdate()
    {
        // ï¿½ï¿½ï¿½ï¿½ ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½, ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½
        if (!isDashing)
        {
            if (rb.linearVelocityY < 0f)
            {
                rb.gravityScale = baseGravity * fallingGravityFactor;
            } 
            else
            {
                rb.gravityScale = baseGravity;
            }
        }
    }

    // ��� �������� �������������� ��������� ����� IsGrounded
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }

    public void SpeedChange(float amount)
    {
        moveSpeed += amount;
    }
    private IEnumerator DisablePlatformCollision()
    {
        Collider2D[] overlappingPlatforms = Physics2D.OverlapBoxAll(groundCheck.position, groundCheckSize, 0f, platformLayer);
        foreach (Collider2D col in overlappingPlatforms)
        {
            PlatformEffector2D effector = col.GetComponent<PlatformEffector2D>();
            if (effector != null)
            {
                Collider2D platformCollider = col.GetComponent<Collider2D>();
                if (platformCollider != null)
                {
                    platformCollider.enabled = false;
                    yield return new WaitForSeconds(0.5f);
                    platformCollider.enabled = true;
                }
            }
        }
    }

}