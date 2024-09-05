using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

/// <summary>
/// Generates particles for things getting hit, this is not a feedback but kinda is in spirit
/// Attach this script and particle system to be a children of a parent character
/// </summary>
public class OnHitParticles : MonoBehaviour
{
    [HideInInspector]
    public Character character;

    public ParticleSystem ps;

    [Tooltip("Any damage received below this threshold will not play any particles")]
    public float damageThreshold;
    [Tooltip("Any damage received above this threshold will be considered as taking 'max damage'")]
    public float damageCeiling;

    //Controls the amount of particles generated - min/max amount
    public int particlesFloor;
    public int particlesCeiling;
    //The plus and minus random variant amount of particles to generate, final value can exceed or drop below min/max
    public int particlesRandomVariance;

    private void Awake()
    {
        character = GetComponentInParent<Character>();
        
        if (character == null)
        {
            //No character script found, destroy self
            Destroy(this);
        }
    }

    private void Start()
    {
        character.OnDamage += PlayParticles;
    }

    void PlayParticles(float damage, Vector3 direction)
    {
        if (damage < damageThreshold)
        {
            return;
        }

        //Rotate against the direction of the damage
        transform.rotation = Quaternion.identity;
        transform.Rotate(new Vector3(0, 0, -Quaternion.LookRotation(direction).eulerAngles.x));

        int particleAmount = (int) math.remap(damageThreshold, damageCeiling, particlesFloor, particlesCeiling, damage);

        var em = ps.emission;

        //Main burst
        ParticleSystem.Burst burst1 = new ParticleSystem.Burst();
        burst1.minCount = (short) Mathf.Clamp(particleAmount - particlesRandomVariance, 0, particleAmount + particlesRandomVariance);
        burst1.maxCount = (short) Mathf.Clamp(particleAmount + particlesRandomVariance, 0, particleAmount + particlesRandomVariance);

        //Secondary burst
        ParticleSystem.Burst burst2 = new ParticleSystem.Burst();
        burst2.time = 0.05f;
        burst2.count = 1;

        em.SetBursts(
        new ParticleSystem.Burst[]{
                    burst1,
                    burst2
        });

        ps.Clear();
        ps.Play();

    }
}
