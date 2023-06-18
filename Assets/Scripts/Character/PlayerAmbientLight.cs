using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Unity.Mathematics;

/// <summary>
/// This script controls the circluar ambient light around the player.
/// Depending on the level's brightness, raise or lower its intensity
/// </summary>
public class PlayerAmbientLight : MonoBehaviour
{
    Light2D ambientLight;
    public float maxIntensity;


    // Start is called before the first frame update
    void Start()
    {
        float levelBrightness =  LevelManager.instance.LevelGlobalLight.intensity;
        //Any level below 0.7f brightness will have full player ambient light brightness
        levelBrightness = Mathf.Clamp(levelBrightness, 0.7f, 1f);
        ambientLight = GetComponent<Light2D>();
        ambientLight.intensity = maxIntensity - math.remap(0.7f, 1f, 0, maxIntensity, levelBrightness);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
