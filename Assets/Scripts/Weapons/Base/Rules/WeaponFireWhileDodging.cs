using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFireWhileDodging : MonoBehaviour, IWeaponFireRule
{
    public bool weaponCanFireWhileDodging;
    
    /// <summary>
    /// If character is dodging, and rule is false, the weapon cannot fire
    /// </summary>
    /// <param name="user"></param>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public bool WeaponCanFire(Character user, Weapon weapon)
    {
        if (user.MovementState != CharacterStates.MovementStates.Dodging)
        {
            return true;
        }
        else
        {
            return weaponCanFireWhileDodging;
        }
    }

}
