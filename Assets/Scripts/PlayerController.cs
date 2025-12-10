using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float jumpForce = 6f;
    public int maxJumps = 2;
    public float climbSpeed = 3f;
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public Light2D playerLight;
    public float maxLightIntensity = 2f;
    public float lightDecayRate = 0.1f;
    public float intensityToBlink = 0.3f;
    public float blinkSpeed = 5f;
    public float fragmentLightBonus = 1f;
    private Animator anim;
    private Rigidbody2D rb;
    private UI uiManager;
    private Vector2 moveInput;
    private bool faceRight = true;
    private bool isGrounded = true;
    private int jumpCount = 0;
    private bool isOnStairs = false;
    private bool isClimbing = false;
    public int fragments { get; private set; }
    private float currentLightIntensity;
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
        moveInput = moveAction.action.ReadValue<Vector2>();
        
        HandleClimbing();
        HandleAnimations();
        HandleFlip();
        HandleLight();
    }
    
    void FixedUpdate()
    {
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
        if (!isClimbing)
        {
            if (rb.linearVelocity.x > 0 && !faceRight)
                Flip();
            else if (rb.linearVelocity.x < 0 && faceRight)
                Flip();
        }
    }
    
    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        faceRight = !faceRight;
    }
    
    private void HandleLight()
    {
        if (playerLight == null) return;
        currentLightIntensity -= lightDecayRate * Time.deltaTime;
        currentLightIntensity = Mathf.Max(currentLightIntensity, 0f);
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
    
    public void CollectFragment()
    {
        fragments++;
        currentLightIntensity = Mathf.Min(currentLightIntensity + fragmentLightBonus, maxLightIntensity);
        
        if (uiManager != null)
            uiManager.UpdateFragmentUI(fragments);
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        if (isClimbing) return;
        
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