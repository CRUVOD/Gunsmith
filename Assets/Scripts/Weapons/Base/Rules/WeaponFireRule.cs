using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//These rules checks the states of the user and weapon, and see if the user can fire the weapon
public interface IWeaponFireRule
{
    bool WeaponCanFire(Character user, Weapon weapon);
}
