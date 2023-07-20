using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUnderbarrelAttachment : WeaponAttachment
{
    [Tooltip("Multiplier")]
    public float fireRateModifier = 1f;
    [Tooltip("Multiplier")]
    public float recoilModifier = 1f;

    public override void InitialiseAttachment(Weapon weapon)
    {
        var projectileWeapon = weapon as ProjectileWeapon;
        if (projectileWeapon != null)
        {
            InitialiseAttachment(projectileWeapon);
            return;
        }

        var hitScanWeapon = weapon as HitScanWeapon;
        if (hitScanWeapon != null)
        {
            InitialiseAttachment(hitScanWeapon);
            return;
        }
    }

    public override void InitialiseAttachment(ProjectileWeapon weapon)
    {
        base.InitialiseAttachment(weapon);
        weapon.fireRate = weapon.fireRate * (1 + fireRateModifier / 100f);
    }

    public override void InitialiseAttachment(HitScanWeapon weapon)
    {
        base.InitialiseAttachment(weapon);
        weapon.fireRate = weapon.fireRate * (1 + fireRateModifier / 100f);
    }
}
