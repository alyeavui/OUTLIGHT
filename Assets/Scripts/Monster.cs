using UnityEngine;

public class Monster : MonoBehaviour
{
    public Transform player;
    public PlayerController playerController;
    public float followDistance = 15f;
    public float moveSpeed = 2f;
    public SpriteRenderer monsterSprite;
    public float appearAtLightLevel = 0.5f;
    private bool hasAttacked = false;
    
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerController = playerObj.GetComponent<PlayerController>();
            }
        }
        if (monsterSprite != null)
        {
            Color color = monsterSprite.color;
            color.a = 0f;
            monsterSprite.color = color;
        }
    }
    
    void Update()
    {
        if (player == null || playerController == null) return;
        Vector3 targetPos = player.position - new Vector3(followDistance, 0, 0);
        targetPos.y = player.position.y;
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        float lightLevel = 0f;
        if (playerController.playerLight != null)
            lightLevel = playerController.playerLight.intensity;
        if (monsterSprite != null)
        {
            Color color = monsterSprite.color;
            
            if (lightLevel < appearAtLightLevel)
            {
                color.a = Mathf.Lerp(color.a, Mathf.InverseLerp(appearAtLightLevel, 0.1f, lightLevel), Time.deltaTime * 2f);
            }
            else
            {
                color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 3f);
            }
            
            monsterSprite.color = color;
        }
        if (lightLevel <= 0f && !hasAttacked)
        {
            AttackPlayer();
        }
    }
    
    void AttackPlayer()
    {
        hasAttacked = true;
        UI uiManager = FindFirstObjectByType<UI>();
        if (uiManager != null)
        {
            Debug.Log("Monster caught you");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Vector3 monsterPos = player.position - new Vector3(followDistance, 0, 0);
            Gizmos.DrawWireSphere(monsterPos, 1f);
            Gizmos.DrawLine(player.position, monsterPos);
        }
    }
}