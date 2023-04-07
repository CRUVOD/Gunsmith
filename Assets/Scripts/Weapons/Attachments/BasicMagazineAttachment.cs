using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMagazineAttachment : WeaponAttachment
{
    [Tooltip("Multiplier")]
    public float projectileCapacityModifier;
    [Tooltip("Multiplier")]
    public float reloadTimeModifier;

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
        weapon.magazineSize = Mathf.CeilToInt(weapon.magazineSize * projectileCapacityModifier);
        weapon.reloadTime *= reloadTimeModifier;
    }
}
