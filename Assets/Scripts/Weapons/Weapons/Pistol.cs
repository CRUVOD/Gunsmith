using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : ProjectileWeapon
{
    [Header("Pistol")]
    [Tooltip("show the cooldown duration between shots in the cooldown circle")]
    //Don't set to true if the fire rate is high
    public bool useCoolDownCircleUIBetweenShots;

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
            Projectile newProjectile;

            //Instantiate the bullet and send it flying
            if (User == CharacterTypes.Player)
            {
                newProjectile = Instantiate(projectile, firePoint.position, MouseDirectionQuaternion());
            }
            else
            {
                newProjectile = Instantiate(projectile, firePoint.position, PlayerDirectionQuaternion());
            }

            newProjectile.SetVelocity(weaponDirection.normalized * projectileSpeed);
            newProjectile.SetDamage(minDamage, maxDamage);
            newProjectile.SetLifeTime(projectileLifeTime);

            //Use ammo in clip
            ChangeAmmoCount(currentAmmoInMagazine - 1);

            //Reset time between shots according to fire rate if theres stil ammo in the clip
            if (currentAmmoInMagazine > 0)
            {
                ResetTimeBetweenShots();
                if (useCoolDownCircleUIBetweenShots)
                {
                    UIManager.instance.crosshair.StartCoolDownUI(60f / fireRate);
                }
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
