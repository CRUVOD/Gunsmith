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

    //The instant a cast starts, this timer starts, this is analagous to fire rate
    public float recastTime;
    private float recastTimer;

    bool castInProgress;

    protected override void Start()
    {
        base.Start();
        castInProgress = false;
        recastTimer = 0;
    }

    protected virtual void Update()
    {
        RotateWeapon();
        //Count down recast
        if (recastTimer >= 0)
        {
            recastTimer -= Time.deltaTime;
        }
    }

    protected override void RotateWeapon()
    {
        if (rotationFrozen)
        {
            return;
        }

        if (UserType == CharacterTypes.Player)
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
        if (!castInProgress && recastTimer < 0)
        {
            //We currently are not casting the AOE yet/we don't have an AOE to cast
            if (UserType == CharacterTypes.Player)
            {
                //TODO
                return false;
            }
            else
            {
                CreateAOEPlayerTarget();
                castInProgress = true;
                recastTimer = recastTime;
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
        if (currentSpellCast == null)
        {
            return;
        }

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
        currentSpellCast = null;
        castInProgress = false;
    }

    public void CancelCast()
    {

    }
}
