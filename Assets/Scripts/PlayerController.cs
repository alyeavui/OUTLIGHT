using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float jumpForce = 6f;
    private int maxJumps = 2;
    private float climbSpeed = 3f;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    public Light2D playerLight;
    public float maxLightIntensity = 2f;
    private float lightDecayRate = 0.1f;
    private float intensityToBlink = 0.3f;
    private float blinkSpeed = 5f;
    private float fragmentLightBonus = 0.5f; 
    private Color normalLightColor = Color.white;
    private Color dangerLightColor = Color.red;
    private Animator anim;
    private Rigidbody2D rb;
    private UI uiManager;
    private Vector2 moveInput;
    private bool faceRight = true;
    private bool isGrounded = true;
    private int jumpCount = 0;
    private bool isOnStairs = false;
    private bool isClimbing = false;
    private bool isDead = false;
    public int fragments { get; private set; }
    private float currentLightIntensity;
    private HashSet<int> collectedFragmentIds = new HashSet<int>();
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        uiManager = FindFirstObjectByType<UI>();
        fragments = 0;
        
        if (playerLight != null)
        {
            currentLightIntensity = maxLightIntensity;
            playerLight.intensity = currentLightIntensity;
            playerLight.color = normalLightColor;
        }
    }
    
    void Start()
    {
        if (uiManager != null)
            uiManager.UpdateFragmentUI(fragments);
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
        if (isDead) return;
        
        moveInput = moveAction.action.ReadValue<Vector2>();
        
        HandleClimbing();
        HandleAnimations();
        HandleFlip();
        HandleLight();
    }
    
    void FixedUpdate()
    {
        if (isDead) return;
        
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(moveInput.x * climbSpeed * 0.5f, moveInput.y * climbSpeed);
        }
        else
        {
            rb.gravityScale = 1f;
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }
    }
    
    private void HandleClimbing()
    {
        if (isOnStairs)
        {
            float verticalInput = moveInput.y;
            if (Mathf.Abs(verticalInput) > 0.1f)
            {
                isClimbing = true;
            }
            else if (isClimbing && Mathf.Abs(verticalInput) < 0.1f)
            {
                isClimbing = false;
            }
        }
    }
    
    private void HandleAnimations()
    {
        bool isWalking = !isClimbing && Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded;
        anim.SetBool("isWalking", isWalking);
        bool isJumping = !isGrounded && !isClimbing;
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isClimbing", isClimbing);
    }
    
    private void HandleFlip()
    {
        if (isClimbing) return;

        if (moveInput.x > 0.1f && !faceRight)
            Flip();
        else if (moveInput.x < -0.1f && faceRight)
            Flip();
    }
    
    private void Flip()
    {
        faceRight = !faceRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    private void HandleLight()
    {
        if (playerLight == null) return;
        currentLightIntensity -= lightDecayRate * Time.deltaTime;
        currentLightIntensity = Mathf.Max(currentLightIntensity, 0f);
        if (currentLightIntensity <= 0f && !isDead)
        {
            TriggerDeath();
            return;
        }
        if (currentLightIntensity <= intensityToBlink && currentLightIntensity > 0f)
        {
            float blink = Mathf.PingPong(Time.time * blinkSpeed, 1);
            playerLight.intensity = Mathf.Lerp(0f, currentLightIntensity, blink);
        }
        else
        {
            playerLight.intensity = currentLightIntensity;
        }
    }
    
    public void CollectFragment(int fragmentId)
    {
        if (collectedFragmentIds.Contains(fragmentId)) return;
        collectedFragmentIds.Add(fragmentId);
        fragments++;
        currentLightIntensity = Mathf.Min(currentLightIntensity + fragmentLightBonus, maxLightIntensity);
        if (uiManager != null)
            uiManager.UpdateFragmentUI(fragments);
    }
    
    private void TriggerDeath()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        if (playerLight != null)
        {
            playerLight.color = dangerLightColor;
            playerLight.intensity = 0.5f;
        }
        if (uiManager != null)
        {
            Invoke(nameof(NotifyGameOver), 2f);
        }
    }
    
    private void NotifyGameOver()
    {
        if (uiManager != null)
            uiManager.TriggerGameOver("The darkness consumed you!");
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        if (isClimbing || isDead) return;
        
        if (isGrounded || jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            if (collision.contacts[0].normal.y > 0.5f)
            {
                isGrounded = true;
                jumpCount = 0;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Stairs"))
        {
            isOnStairs = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Stairs"))
        {
            isOnStairs = false;
            isClimbing = false;
            rb.gravityScale = 1f;
        }
    }
}