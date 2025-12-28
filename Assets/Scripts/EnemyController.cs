using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float chaseRange = 8f; 
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color chaseColor = Color.red;
    private Transform player;
    private Transform currentTarget;
    private bool isChasing = false;
    private bool facingRight = true;
    private Vector3 patrolStartPosition;
    private SpriteRenderer spriteRenderer;
   
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        currentTarget = pointA;
        patrolStartPosition = transform.position;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }
    
    void Update()
    {
        if (player == null) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange && !isChasing)
        {
            StartChase();
        }
        else if (distanceToPlayer > chaseRange && isChasing)
        {
            StopChase();
        }
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }
    
    void Patrol()
    {
        if (currentTarget == null) return;
        Vector2 targetPos = new(currentTarget.position.x, transform.position.y);
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        
        transform.position = Vector2.MoveTowards(
            transform.position, 
            targetPos, 
            patrolSpeed * Time.deltaTime
        );
        if (direction.x > 0 && !facingRight)
            Flip();
        else if (direction.x < 0 && facingRight)
            Flip();
        if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.1f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }
    
    void ChasePlayer()
    {
        Vector2 targetPos = new(player.position.x, transform.position.y);
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        
        transform.position = Vector2.MoveTowards(
            transform.position, 
            targetPos, 
            chaseSpeed * Time.deltaTime
        );
        if (direction.x > 0 && !facingRight)
            Flip();
        else if (direction.x < 0 && facingRight)
            Flip();
    }
    
    void StartChase()
    {
        isChasing = true;
        if (spriteRenderer != null)
            spriteRenderer.color = chaseColor;
    }
    
    void StopChase()
    {
        isChasing = false;
        
        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;

        float distA = Vector2.Distance(transform.position, pointA.position);
        float distB = Vector2.Distance(transform.position, pointB.position);
        currentTarget = (distA < distB) ? pointA : pointB;
    }
    
    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            KillPlayer(collision.gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            KillPlayer(collision.gameObject);
        }
    }
    
    void KillPlayer(GameObject playerObj)
    {
        if (playerObj.TryGetComponent<PlayerController>(out var playerController))
        {
            playerController.enabled = false;
        }
        UI uiManager = FindFirstObjectByType<UI>();
        if (uiManager != null)
        {
            uiManager.PlayerDied();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.3f);
            Gizmos.DrawWireSphere(pointB.position, 0.3f);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}