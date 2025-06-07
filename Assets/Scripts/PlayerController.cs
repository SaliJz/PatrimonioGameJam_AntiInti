using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheckPoint;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private bool autoRun = true;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Gravedad")]
    [SerializeField] private float gravityMultiplier = 1.5f;

    [Header("Suelo")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded = false;
    private bool jumpPressed;
    private bool isHolding;
    private float lastGroundedTime = 0f;
    private float input;
    private Vector2 groundNormal = Vector2.up;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (rb != null) rb.gravityScale = gravityMultiplier;
    }

    private void Update()
    {
        input = (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ? -1f :
            (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) ? 1f : 0f;

        jumpPressed = Input.GetKeyDown(KeyCode.Space);
        isHolding = Input.GetKey(KeyCode.Space);
    }

    private void FixedUpdate()
    {
        HandleMovement(input);
        HandleJump(jumpPressed);
        HandleAirActions();
        CheckGroundRaycast();
    }

    private void HandleMovement(float input)
    {
        if (isGrounded)
        {
            Vector2 moveDir = Vector2.Perpendicular(groundNormal).normalized;
            if (moveDir.x < 0f) moveDir *= -1f;
            float effectiveInput = autoRun ? 1f : input;
            Vector2 targetVelocity = moveDir * moveSpeed * effectiveInput;
            Vector2 velocity = rb.velocity;

            velocity = Vector2.Lerp(velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed), velocity.y);
        }
    }

    private void HandleJump(bool jumpPressed)
    {
        if ((isGrounded || Time.time - lastGroundedTime < coyoteTime) && jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(groundNormal * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void HandleAirActions()
    {
        if (!isGrounded && isHolding)
        {
            rb.gravityScale = 0.3f;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -2f));
        }
        else
        {
            rb.gravityScale = gravityMultiplier;
        }
    }

    private void CheckGroundRaycast()
    {
        Collider2D groundHit = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        if (groundHit)
        {
            isGrounded = true;
            groundNormal = Vector2.up;
            lastGroundedTime = Time.time;
        }
        else
        {
            if (isGrounded) lastGroundedTime = Time.time;
            isGrounded = false;
        }
    }
}

