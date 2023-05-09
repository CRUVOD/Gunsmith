using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The AI will fire the projectile weapon in a specific pattern every set amount of time
/// </summary>
public class AIProjectileWeaponPatternAction : AIAction
{
    public ProjectileWeapon projectileWeapon;
    //We only apply the recoil once, despite firing multiple times for simplicity sake
    public bool applyRecoil;

    //If the AI is currently using the projectile weapon
    bool isFiring = false;

    //Time between trying to fire the pattern
    public float timeBetweenFiring;
    float timeBetweenFiringTimer;
    //The pattern the in which the AI will fire the projectile
    public ProjectilePattern projectilePattern;

    private void Start()
    {
        timeBetweenFiringTimer = -1;
    }

    private void Update()
    {
        if (timeBetweenFiringTimer > 0)
        {
            timeBetweenFiringTimer -= Time.deltaTime;
        }
    }

    public override void PerformAction()
    {
        TryUseWeapon();
    }

    private void TryUseWeapon()
    {
        if (!isFiring)
        {
            if (timeBetweenFiringTimer < 0 && projectilePattern.IsValid())
            {
                StartCoroutine(FireProjectilePattern(projectilePattern.projectileInstances));
            }
        }
    }

    private IEnumerator FireProjectilePattern(ProjectileInstance[] projectileInstances)
    {
        isFiring = true;
        //wait for the first projectile
        yield return new WaitForSeconds(projectileInstances[0].timeOffset);

        for (int i = 0; i < projectileInstances.Length - 1; i++)
        {
            //Fires all projectiles up to the second last one
            float currentTime = projectileInstances[i].timeOffset;
            projectileWeapon.FireProjectile(projectileInstances[i].angleOffset);
            yield return new WaitForSeconds(projectileInstances[i + 1].timeOffset - currentTime);
        }

        //Fires the last projectile
        projectileWeapon.FireProjectile(projectileInstances[projectileInstances.Length - 1].angleOffset);
        isFiring = false;
        timeBetweenFiringTimer = timeBetweenFiring;
    }

    public override void OnExitState()
    {
        base.OnExitState();
        projectileWeapon.StopWeapon();
        StopAllCoroutines();
        isFiring = false;
    }
}
