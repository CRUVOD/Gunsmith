using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : ProjectileWeapon
{
    [Header("Pistol")]
    [Tooltip("show the cooldown duration between shots in the cooldown circle")]
    //Don't set to true if the fire rate is high
    public bool useCoolDownCircleUIBetweenShots;

    protected override void Start()
    {
        base.Start();
        // Set start ammo to max
        currentAmmoInMagazine = projectileCapacity;

        inReload = false;
    }

    /// <summary>
    /// Creates a projectile and send it lololol
    /// </summary>
    /// <returns></returns>
    public override bool FireProjectile()
    {
        if (base.FireProjectile())
        {
            //Additional functionality of using the crosshair cooldown UI if succesfully fired weapon
            if (useCoolDownCircleUIBetweenShots)
            {
                UIManager.instance.crosshair.StartCoolDownUI(60f / fireRate);
            }
            return true;
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
