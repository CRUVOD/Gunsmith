using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMagazineAttachment : WeaponAttachment
{
    [Tooltip("Multiplier")]
    public float projectileCapacityModifier = 1f;
    [Tooltip("Multiplier")]
    public float reloadTimeModifier = 1f;

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
        weapon.projectileCapacity = Mathf.CeilToInt(weapon.projectileCapacity * projectileCapacityModifier);
        weapon.reloadTime *= reloadTimeModifier;
    }

    public override void InitialiseAttachment(HitScanWeapon weapon)
    {
        base.InitialiseAttachment(weapon);
        weapon.ammoCapacity = Mathf.CeilToInt(weapon.ammoCapacity * projectileCapacityModifier);
        weapon.reloadTime *= reloadTimeModifier;
    }
}
