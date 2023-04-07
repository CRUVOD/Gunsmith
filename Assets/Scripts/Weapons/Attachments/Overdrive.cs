using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overdrive : WeaponAttachment
{
    [Tooltip("Percentage increase in fireRate for this weapon")]
    public float fireRateModifier;

    public override void InitialiseAttachment(Weapon weapon)
    {
        base.InitialiseAttachment(weapon);
        //For now, only apply this attachment to projectile weapons
        var projectileWeapon = weapon as ProjectileWeapon;

        if (projectileWeapon != null)
        {
            // successfully cast
            projectileWeapon.fireRate = projectileWeapon.fireRate * (1 + fireRateModifier / 100f);
        }
        else
        {
            var hitScanWeapon = weapon as HitScanWeapon;
            if (hitScanWeapon != null)
            {
                hitScanWeapon.fireRate = hitScanWeapon.fireRate * (1 + fireRateModifier / 100f);
            }
        }
    }
}
