using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : HitScanWeapon
{
    [Header("Shotgun")]
    //The number of smaller projectiles that would be shot from barrel
    public int numberOfPellets;
    public float spread;
    public bool isPumpAction;

    [SerializeField]
    protected TrailRenderer HitScanTrail;
    [SerializeField]
    //the impact particle system to play on hit
    protected ParticleSystem impactParticleSystem;

    public FeedbackPlayer weaponUseFeedback;
    public FeedbackPlayer weaponPumpFeedback;
    public FeedbackPlayer weaponReloadFeedback;
    public FeedbackPlayer weaponEmptyFeedback;

    private Coroutine reloadRoutine;

    protected override void Start()
    {
        base.Start();
        // Set start ammo to max
        currentAmmoReady = ammoCapacity;
        
        inReload = false;
    }

    /// <summary>
    /// Check if the weapon is ready to fire another projectile
    /// </summary>
    /// <returns></returns>
    public override bool ReadyToFire()
    {
        if (GetTimeBetweenShots() > 0)
        {
            return false;
        }

        if (currentAmmoReady <= 0)
        {
            return false;
        }

        if (inReload && isMagazineBased)
        {
            return false;
        }
        else if (inReload && !isMagazineBased && currentAmmoReady > 0)
        {
            //Reloading by each shell
            //We can cancel the reload process, and continue shooting with the current loaded shells
            ReloadCancel();
            return true;
        }

        return true;
    }


    /// <summary>
    /// We simulate the multiple projectiles by firing multiple raycasts
    /// </summary>
    /// <returns></returns>
    public override bool FireRaycast()
    {
        bool ready = ReadyToFire();

        if (ready)
        {
            //Change to RayCastAll when adding bullet penetration

            for (int i = 0; i < numberOfPellets; i++)
            {
                RaycastHit2D hit;

                float randomSpread = Random.Range(-spread, spread);
                Vector2 randomDirection = Quaternion.Euler(0f, 0f, randomSpread) * weaponDirection;

                hit = Physics2D.Raycast(firePoint.position, randomDirection, rayTravelDistance, TargetLayerMask);

                if (hit)
                {
                    Colliding(hit.collider.gameObject);
                    TrailRenderer trail = Instantiate(HitScanTrail, firePoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail, hit));
                }
                else
                {
                    TrailRenderer trail = Instantiate(HitScanTrail, firePoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail, (Vector2)firePoint.position + randomDirection * rayTravelDistance));
                }
            }

            //Use ammo in clip
            ChangeAmmoCount(currentAmmoReady - 1);

            //Reset time between shots according to fire rate if theres stil ammo in the clip
            if (currentAmmoReady > 0)
            {
                ResetTimeBetweenShots();
                //Calls to UIManager for displaying cooldown
                UIManager.instance.crosshair.StartCoolDownUI(60f / fireRate);
            }

            //Calls to play feedback
            weaponUseFeedback.PlayFeedbacks();
            if (isPumpAction)
            {
                weaponPumpFeedback.PlayFeedbacks();
            }

            return true;
        }
        else if (currentAmmoReady <= 0)
        {
            weaponEmptyFeedback.PlayFeedbacks();
        }

        return false;
    }

    /// <summary>
    /// If hit, spawn and play the impact effect, and the trail
    /// </summary>
    /// <param name="trail"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit2D hit)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;
        ParticleSystem impactEffect = Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(trail.gameObject, trail.time);
        Destroy(impactEffect.gameObject, impactEffect.main.startLifetime.constant);
    }

    /// <summary>
    /// If hit nothing, just play the trail
    /// </summary>
    /// <param name="trail"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 position)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, position, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = position;
        Destroy(trail.gameObject, trail.time);
    }

    /// <summary>
    /// Loading individual shells type shotgun
    /// </summary>
    public override void NonMagazineReload()
    {
        if (inReload)
        {
            return;
        }

        inReload = true;
        timeBetweenShots = 0;

        reloadRoutine = StartCoroutine(IndividualShellReload());
    }

    /// <summary>
    /// Magazine based shotgun reload
    /// </summary>
    public override void MagazineReload()
    {
        if (inReload)
        {
            return;
        }

        base.MagazineReload();

        //Calls to UIManager for displaying reload
        UIManager.instance.crosshair.StartCoolDownUI(reloadTime);
        
    }

    IEnumerator IndividualShellReload()
    {
        int numberOfShellsToFill = ammoCapacity - currentAmmoReady;

        for (int i = 0; i < numberOfShellsToFill; i++)
        {
            UIManager.instance.crosshair.StartCoolDownUI(reloadTime);
            currentAmmoReady += 1;
            yield return new WaitForSeconds(reloadTime);
        }

        //reload complete
        inReload = false;
    }

    public override void ReloadCancel()
    {
        if (!inReload)
        {
            return;
        }

        if (!isMagazineBased)
        {
            StopCoroutine(reloadRoutine);
        }

        inReload = false;
    }

    /// <summary>
    /// Another override, stopping individual shell reloads if they are in progress
    /// </summary>
    public override void OnDequip()
    {
        base.OnDequip();
    }
}
