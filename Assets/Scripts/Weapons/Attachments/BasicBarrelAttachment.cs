using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBarrelAttachment : WeaponAttachment
{
    [Tooltip("Multiplier")]
    public float spreadModifier = 1f;
    [Tooltip("Multiplier")]
    public float projectileSpeedModifier = 1f;
    [Tooltip("Addition")]
    public int projectileDamageModifier = 0;

    public override void InitialiseAttachment(Weapon weapon)
    {
        var projectileWeapon = weapon as ProjectileWeapon;
        if (projectileWeapon != null)
        {
            InitialiseAttachment(projectileWeapon);
            return;
        }
    }

    public override void InitialiseAttachment(ProjectileWeapon weapon)
    {
        base.InitialiseAttachment(weapon);
        weapon.baseSpread *= spreadModifier;
        weapon.projectileSpeed *= projectileSpeedModifier;
        weapon.minDamage += projectileDamageModifier;
        weapon.maxDamage += projectileDamageModifier;
    }
}
