using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AR : ProjectileWeapon
{
    [Header("AR")]
    [Tooltip("AR's weapon spread is dependent on continuous fire, kinda like recoil")]
    //Maximum spread amount
    public float maxSpread;
    //How fast spread increases to max spread
    public float spreadIncreaseRate;
    //How fast the spread resets to base spread
    public float spreadResetRate;
    private float spread;

    protected override void Start()
    {
        base.Start();
        // Set start ammo to max
        currentAmmoInMagazine = magazineSize;
        //Set default spread
        spread = baseSpread;
        inReload = false;
        UpdateUI();
    }

    protected override void Update()
    {
        base.Update();
        CalculateSpread(false);
    }

    public override bool FireProjectile()
    {
        //We override the base because we need to calculate spread
        bool ready = ReadyToFire();

        if (ready)
        {
            Projectile newProjectile;
            CalculateSpread(true);
            float randomSpread = Random.Range(-spread, spread);

            //Instantiate the bullet and send it flying with random spread
            newProjectile = Instantiate(projectile, firePoint.position, transform.rotation);
            newProjectile.transform.Rotate(0, 0, randomSpread);
            newProjectile.SetVelocity(projectileSpeed);
            newProjectile.SetDamage(minDamage, maxDamage);
            newProjectile.SetLifeTime(projectileLifeTime);

            //Use ammo in clip
            if (isMagazineBased)
            {
                ChangeAmmoCount(currentAmmoInMagazine - 1);
            }

            //Reset time between shots according to fire rate if theres stil ammo in the clip
            if (currentAmmoInMagazine > 0)
            {
                ResetTimeBetweenShots();
            }

            //Calls to play feedback
            weaponUseFeedback.PlayFeedbacks();

            return true;
        }
        else if (isMagazineBased && currentAmmoInMagazine <= 0)
        {
            weaponEmptyFeedback.PlayFeedbacks();
        }

        return false;
    }

    /// <summary>
    /// Calculate the spread of the projectiles depending on if the weapon has been fired this frame
    /// </summary>
    /// <returns></returns>
    private void CalculateSpread(bool weaponFired)
    {
        if (weaponFired)
        {
            spread = Mathf.Clamp(spread + spreadIncreaseRate, baseSpread, maxSpread);
        }
        else
        {
            spread = Mathf.Clamp(spread - spreadResetRate * Time.deltaTime, baseSpread, maxSpread);
        }
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
        weaponReloadFeedback.PlayFeedbacks();
    }

    /// <summary>
    /// Another override, to stop feedbacks to play
    /// </summary>
    public override void OnDequip()
    {
        base.OnDequip();
        weaponReloadFeedback.StopFeedbacks();
    }
}
