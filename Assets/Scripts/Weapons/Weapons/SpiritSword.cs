using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spirit sword script for the watchful guardian
/// </summary>
public class SpiritSword : MeleeWeapon
{
    protected override void RotateWeapon()
    {
        transform.rotation = PlayerDirectionQuaternion();
        weaponDirection = PlayerDirectionVector2();
    }
}
