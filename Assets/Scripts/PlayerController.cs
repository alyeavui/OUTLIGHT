using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private float moveSpeed = 5f;
    private float jumpForce = 5f;
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    private bool isGrounded = true;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool faceRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        jumpAction.action.performed += OnJump;
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        jumpAction.action.performed -= OnJump;
    }

    void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
        HandleAnimations();
        HandleFlip();
    }

    private void HandleAnimations()
    {
        bool isWalking = rb.linearVelocity.x != 0;
        anim.SetBool("isWalking", isWalking);
    }

    private void HandleFlip()
    {
        if (rb.linearVelocity.x > 0 && faceRight == false)
            Flip();
        else if (rb.linearVelocity.x < 0 && faceRight == true)
            Flip();
    }
    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        faceRight = !faceRight;
    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            if (collision.contacts[0].normal.y > 0.5f)
                isGrounded = true;
        }
    }
}