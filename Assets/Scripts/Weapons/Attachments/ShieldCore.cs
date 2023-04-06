using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCore : WeaponAttachment
{
    public int shieldHealth;
    public float delayBeforeRecharge;
    public float rechargeRate;
    //How much in front of the weapon the shield will be
    public float shieldDistanceFromWeapon;

    [Header("Shield Prefab")]
    public BarrierShield shieldPrefab;

    public override void InitialiseAttachment(Weapon weapon)
    {
        base.InitialiseAttachment(weapon);
        BarrierShield newShield = Instantiate(shieldPrefab, weapon.firePoint.position + new Vector3(shieldDistanceFromWeapon, 0, 0), Quaternion.identity, weapon.weaponSprite.transform);
        newShield.SetShieldProperties(shieldHealth, delayBeforeRecharge, rechargeRate);
    }
}
