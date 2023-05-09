using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWeaponAction : AIAction
{
    public Weapon weapon;
    [Tooltip("Hides the weapon when not in state")]
    public bool AutoHideWeapon;
    [Header("Limit weapon use attempts")]
    public bool LimitUsePerStateEnter;
    public int WeaponUsesPerStateEnter;
    private int weaponUses;

    private void Start()
    {
        if (AutoHideWeapon)
        {
            weapon.weaponSprite.SetActive(false);
        }
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        weapon.weaponSprite.SetActive(true);
        weaponUses = WeaponUsesPerStateEnter;
    }

    public override void PerformAction()
    {
        if (LimitUsePerStateEnter && weaponUses >= 0)
        {
            weapon.Use();
            weaponUses -= 1;
        }
        else if (!LimitUsePerStateEnter)
        {
            weapon.Use();
        }

    }

    public override void OnExitState()
    {
        base.OnExitState();
        weapon.StopWeapon();
        if (AutoHideWeapon)
        {
            weapon.weaponSprite.SetActive(false);
        }
    }
}
