using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overdrive : WeaponAttachment
{
    [Tooltip("Percentage decrease in fireRate for this weapon")]
    public float fireRateModifier;

    public override void ApplyStatModifiers(Weapon weapon)
    {
        //For now, only apply this attachment to projectile weapons
        var projectileWeapon = weapon as ProjectileWeapon;

        if (projectileWeapon != null)
        {
            // successfully cast
            projectileWeapon.fireRate = projectileWeapon.fireRate * (1 + fireRateModifier / 100f);
        }
        else
        {
            // cast failed
        }
    }
}
