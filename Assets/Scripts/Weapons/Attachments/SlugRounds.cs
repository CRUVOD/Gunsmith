using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This attachment converts the shotgun into
/// </summary>
public class SlugRounds : WeaponAttachment
{
    public float weaponRange = 20f;
    public int minDamage = 30;
    public int maxDamage = 31;

    public override void InitialiseAttachment(Weapon weapon)
    {
        var shotgun = weapon as Shotgun;
        if (shotgun != null)
        {
            InitialiseShotgun(shotgun);
            return;
        }
    }

    void InitialiseShotgun(Shotgun shotgun)
    {
        shotgun.numberOfPellets = 1;
        shotgun.spread = 0.2f;
        shotgun.rayTravelDistance = weaponRange;
        shotgun.MinDamageCaused = minDamage;
        shotgun.MaxDamageCaused = maxDamage;
    }
}
