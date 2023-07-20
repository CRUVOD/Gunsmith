using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specific attachments for the shotgun weapons
/// </summary>
public class BasicShotgunAttachment : WeaponAttachment
{
    [Tooltip("Multiplier")]
    public float spreadModifier = 1f;
    [Tooltip("Multiplier")]
    public float rangeModifier = 1f;
    [Tooltip("Addition")]
    public int pelletModifier = 0;

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
        shotgun.spread *= spreadModifier;
        shotgun.rayTravelDistance *= rangeModifier;
        shotgun.numberOfPellets += pelletModifier;
    }
}
