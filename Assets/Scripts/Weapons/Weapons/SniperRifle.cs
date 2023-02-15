using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperRifle : HitScanWeapon
{
    [Header("Sniper Rifle")]

    public FeedbackPlayer weaponUseFeedback;
    public FeedbackPlayer weaponBoltPullFeedback;
    public FeedbackPlayer weaponReloadFeedback;
    public FeedbackPlayer weaponEmptyFeedback;

    protected override void Start()
    {
        base.Start();
        // Set start ammo to max
        currentAmmoInMagazine = magazineSize;

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

        if (currentAmmoInMagazine <= 0)
        {
            return false;
        }

        if (inReload)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Creates a ray to scan for hits
    /// </summary>
    /// <returns></returns>
    public override bool FireRaycast()
    {
        bool ready = ReadyToFire();

        if (ready)
        {
            //Change to RayCastAll when adding bullet penetration

            RaycastHit2D hit;

            hit = Physics2D.Raycast(firePoint.position, weaponDirection, rayTravelDistance, TargetLayerMask);

            if (hit)
            {
                Colliding(hit.collider.gameObject);
                TrailRenderer trail = Instantiate(HitScanTrail, firePoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit));
            }
            else
            {
                TrailRenderer trail = Instantiate(HitScanTrail, firePoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, (Vector2) firePoint.position + weaponDirection * rayTravelDistance));
            }

            //Use ammo in clip
            ChangeAmmoCount(currentAmmoInMagazine - 1);

            //Reset time between shots according to fire rate if theres stil ammo in the clip
            if (currentAmmoInMagazine > 0)
            {
                ResetTimeBetweenShots();
                //Calls to UIManager for displaying cooldown
                UIManager.instance.crosshair.StartCoolDownUI(60f / fireRate);
            }

            //Calls to play feedback
            weaponUseFeedback.PlayFeedbacks();
            weaponBoltPullFeedback.PlayFeedbacks();
            return true;
        }
        else if (currentAmmoInMagazine <= 0)
        {
            weaponEmptyFeedback.PlayFeedbacks();
        }

        return false;
    }

    public override void MagazineReload()
    {
        if (inReload)
        {
            return;
        }

        base.MagazineReload();

        //Calls to UIManager for displaying reload
        UIManager.instance.crosshair.StartCoolDownUI(reloadTime);
        weaponBoltPullFeedback.StopFeedbacks();
        weaponReloadFeedback.PlayFeedbacks();
    }


    /// <summary>
    /// Another override, to stop feedbacks to play
    /// </summary>
    public override void OnDequip()
    {
        base.OnDequip();
        weaponBoltPullFeedback.StopFeedbacks();
        weaponReloadFeedback.StopFeedbacks();
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
}
