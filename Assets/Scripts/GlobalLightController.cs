using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightController : MonoBehaviour
{
    private Light2D globalLight;
    private PlayerController player;
    private float victoryLightIntensity = 3f;
    private float lightTransitionSpeed = 1.5f;
    private int totalFragments = 5;
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