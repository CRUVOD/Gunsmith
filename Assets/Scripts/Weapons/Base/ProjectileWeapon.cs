using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    [Header("Projectile based weapon")]

    public Projectile projectile;
    
    [Header("Projectile settings")]
    //bullet damage
    public int minDamage;
    public int maxDamage;
    //bullet speed
    public float projectileSpeed;
    //how long the bullet lasts for
    public float projectileLifeTime;
    [Header("Weapon properties")]
    //Base random bullet spread
    public float baseSpread;
    //bullets per minute
    public float fireRate;
    //time between shots, calculated from fireRate
    float timeBetweenShots;
    //counting down reload time
    float timeToReload;

    [Header("Magazine")]
    //Attributes if the weapon has a limited magazine size
    public bool isMagazineBased;
    public int magazineSize;
    public float reloadTime;
    [HideInInspector]
    public int currentAmmoInMagazine;
    [HideInInspector]
    public bool inReload;


    protected virtual void Update()
    {
        countDownTimeBetweenShots();
        countDownReloadTime();
        RotateWeapon();
    }

    public void ResetTimeBetweenShots()
    {
        timeBetweenShots = 60f / fireRate;
    }

    public float GetTimeBetweenShots()
    {
        return timeBetweenShots;
    }

    /// <summary>
    /// Changes the current ammo in the magazine, updating the UI
    /// </summary>
    /// <param name="newAmmo"></param>
    public void ChangeAmmoCount(int newAmmo)
    {
        if (!isMagazineBased)
        {
            return;
        }
        currentAmmoInMagazine = newAmmo;
        UIManager.instance.UpdateBallisticAmmoUI(newAmmo, magazineSize);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        UIManager.instance.UpdateBallisticAmmoUI(currentAmmoInMagazine, magazineSize);
    }

    public override void Reload()
    {
        MagazineReload();
    }

    public override void ReloadCancel()
    {
        //if not in reload, get out of this function
        if (!inReload)
        {
            return;
        }

        inReload = false;
    }

    /// <summary>
    /// Reloads the magazine to max by default, can be overriden
    /// </summary>
    public virtual void MagazineReload()
    {
        //Check if weapon is already reloading, then start reload, set timebetweenshots to zero
        if (isMagazineBased && !inReload)
        {
            timeToReload = reloadTime;
            inReload = true;
            timeBetweenShots = 0;
        }
    }

    // Return true if weapon is ready to fire projectile again
    public virtual bool ReadyToFire()
    {
        if (timeBetweenShots > 0)
        {
            return false;
        }

        return true;
    }

    // counts down the timer between shots, call every update
    public virtual void countDownTimeBetweenShots()
    {
        if (timeBetweenShots > 0 && !inReload)
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }

    // counts down the timer between reloads, call every update
    public virtual void countDownReloadTime()
    {
        if (inReload)
        {
            if (timeToReload > 0)
            {
                timeToReload -= Time.deltaTime;
            }
            else if (timeToReload <= 0)
            {
                //reload complete, give full ammo to player
                ChangeAmmoCount(magazineSize);
                inReload = false;
            }
        }
    }

    /// <summary>
    /// Use the projectile weapon, if a projectile is fired, return true
    /// </summary>
    /// <returns></returns>
    public override bool Use()
    {
        return FireProjectile();
    }

    public override bool Use(Character user)
    {
        if (CheckWeaponFireRules(user))
        {
            return Use();
        }
        return false;
    }

    // Returns true if projectile is succesfully fired
    public virtual bool FireProjectile()
    {
        return false;
    }

    /// <summary>
    /// On equipping weapon, overwrite the current cooldown on display
    /// </summary>
    public override void OnEquip()
    {
        if (inReload)
        {
            UIManager.instance.crosshair.SetCoolDownUI(timeToReload, reloadTime);
        }
        else if ((fireRate < 300f) && (timeBetweenShots > 0))
        {
            //Only show a cooldown on slower fire rate weapons really
            UIManager.instance.crosshair.SetCoolDownUI(timeBetweenShots, 60f / fireRate);
        }
        else
        {
            UIManager.instance.crosshair.ResetCoolDownUI();
        }
    }

    /// <summary>
    /// On dequiping the weapon, cancels the reload if it can be reloaded
    /// </summary>
    public override void OnDequip()
    {
        if (isMagazineBased)
        {
            ReloadCancel();
        }
    }

}
