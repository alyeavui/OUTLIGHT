using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightController : MonoBehaviour
{
    public Light2D globalLight;
    public PlayerController player;
    public float victoryLightIntensity = 3f;
    public float lightTransitionSpeed = 1.5f;
    public int totalFragments = 5;
    
    private bool victoryTriggered = false;
    private float targetIntensity = 0f;
    
    void Start()
    {
        if (globalLight == null)
            globalLight = GetComponent<Light2D>();
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.GetComponent<PlayerController>();
        }

        if (globalLight != null)
            globalLight.intensity = 0f;
    }
    
    void Update()
    {
        if (player == null || globalLight == null) return;

        if (player.fragments >= totalFragments && !victoryTriggered)
        {
            victoryTriggered = true;
            targetIntensity = victoryLightIntensity;
        }

        if (victoryTriggered)
        {
            globalLight.intensity = Mathf.Lerp(
                globalLight.intensity, 
                targetIntensity, 
                Time.deltaTime * lightTransitionSpeed
            );
        }
    }
}