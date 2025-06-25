using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float horizontalVelocity = 3f;
    public float horizontalAcceleration = 30f;
    [Header("Jump")]
    public float jumpVelocity = 5f;
    [Header("Ground")]
    public float groundDetectionOffset;
    public LayerMask groundLayerMask;

    [Header("Debug")]
    [SerializeField] public bool onGround { get; private set; } = false;

    Rigidbody2D rb;
    CapsuleCollider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        CheckOnGround();
        HandleJump();
        HandleMovement();
    }

    void CheckOnGround()
    {
        onGround = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.down, groundDetectionOffset, groundLayerMask);
    }

    void HandleJump()
    {
        if (onGround && Input.GetButtonDown("Jump")) rb.linearVelocityY = jumpVelocity;
    }

    void HandleMovement()
    {
        float targetVelocity = Input.GetAxisRaw("Horizontal") * horizontalVelocity;
        float currentVelocity = rb.linearVelocityX;
        float newVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, horizontalAcceleration * Time.deltaTime);
        rb.linearVelocityX = newVelocity;
    }

    public void SpeedChange(float amount)
    {
        horizontalVelocity += amount;
    }
}
