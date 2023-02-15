using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AR : ProjectileWeapon
{
    [Header("AR")]

    public FeedbackPlayer weaponUseFeedback;
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
    /// Creates a projectile and send it lololol
    /// </summary>
    /// <returns></returns>
    public override bool FireProjectile()
    {
        bool ready = ReadyToFire();

        if (ready)
        {
            //Instantiate the bullet and send it flying
            Projectile newProjectile = Instantiate(projectile, firePoint.position, MouseDirectionQuaternion());
            newProjectile.SetVelocity(weaponDirection.normalized * projectileSpeed);
            newProjectile.SetDamage(minDamage, maxDamage);
            newProjectile.SetLifeTime(projectileLifeTime);

            //Use ammo in clip
            ChangeAmmoCount(currentAmmoInMagazine - 1);

            //Reset time between shots according to fire rate if theres stil ammo in the clip
            if (currentAmmoInMagazine > 0)
            {
                ResetTimeBetweenShots();
            }

            //Calls to play feedback
            weaponUseFeedback.PlayFeedbacks();

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
