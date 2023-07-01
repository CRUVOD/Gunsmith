using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cast weapons cast AOESpell instances, they do not rotate based on player direction
/// </summary>
public class CastWeapon : Weapon
{
    public AOESpellInstance AOEPrefab;

    private AOESpellInstance currentSpellCast;

    bool castInProgress;

    protected override void Start()
    {
        base.Start();
        castInProgress = false;
    }

    protected virtual void Update()
    {
        RotateWeapon();
    }

    protected override void RotateWeapon()
    {
        if (rotationFrozen)
        {
            return;
        }

        if (User == CharacterTypes.Player)
        {
            weaponDirection = MouseDirectionVector2();

            if (flipWeaponSprite)
            {
                //Flip weapon sprite if pointing leftwards
                if (weaponDirection.x < 0)
                {
                    weaponSprite.transform.localScale = new Vector3(Mathf.Abs(weaponSprite.transform.localScale.x) * -1f, weaponSprite.transform.localScale.y);
                }
                else
                {
                    weaponSprite.transform.localScale = new Vector3(weaponSprite.transform.localScale.x, Mathf.Abs(weaponSprite.transform.localScale.y));
                }
            }
        }
        else
        {
            weaponDirection = PlayerDirectionVector2();

            if (flipWeaponSprite)
            {
                //Flip weapon sprite if pointing leftwards
                if (weaponDirection.x < 0)
                {
                    weaponSprite.transform.localScale = new Vector3(Mathf.Abs(weaponSprite.transform.localScale.x) * -1f, weaponSprite.transform.localScale.y);
                }
                else
                {
                    weaponSprite.transform.localScale = new Vector3(weaponSprite.transform.localScale.x, Mathf.Abs(weaponSprite.transform.localScale.y));
                }
            }
        }
    }

    public override bool Use()
    {
        if (!castInProgress)
        {
            //We currently are not casting the AOE yet/we don't have an AOE to cast
            if (User == CharacterTypes.Player)
            {
                //TODO
                return false;
            }
            else
            {
                CreateAOEPlayerTarget();
                castInProgress = true;
            }
        }
        else
        {
            Cast();
        }
        
        return true;
    }

    /// <summary>
    /// Create the AOEs in the game
    /// </summary>
    private void CreateAOEPlayerTarget()
    {
        Vector2 targetPosition = LevelManager.instance.player.transform.position;
        currentSpellCast = Instantiate(AOEPrefab, targetPosition, Quaternion.identity);
    }


    /// <summary>
    /// Make progress on casting the spell
    /// </summary>
    private void Cast()
    {
        currentSpellCast.Cast();

        if (currentSpellCast.successfullyCast)
        {
            castInProgress = false;
        }
    }

    public override void StopWeapon()
    {
        base.StopWeapon();
        currentSpellCast?.CancelCast();
        castInProgress = false;
    }

    public void CancelCast()
    {

    }
}
