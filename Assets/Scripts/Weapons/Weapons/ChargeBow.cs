using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///The charge bow requires a hold/charge time, similar to cast weapons
/// During the charge, fires a ray indicator
/// The speed of the weapon rotation is also limited
/// At the end of the charge, fires a raycast
/// </summary>
public class ChargeBow : Weapon
{
    [Header("Charge Bow Properties")]
    //The time it takes to charge the weapon
    public float chargeTime;
    //Time between successful firing of weapon and time till next use/charge of weapon
    public float rechargeTime;

    //Weapon damage
    public int minDamage;
    public int maxDamage;

    //If movement is further slowed when charging
    public bool applyChargingMovementPenalty;
    //Movement penalty when charging.
    [Range(0f, 1f)]
    public float chargeMovementPenalty;

    [Header("AI only")]
    //Limits the speed at which the weapon rotates
    public float EnemyWeaponRotateSpeed;

    bool weaponCharging;
    bool weaponRecharging;
    float chargeTimer;
    float rechargeTimer;

    protected override void Start()
    {
        base.Start();
        chargeTimer = chargeTime;
    }

    private void Update()
    {
        RotateWeapon();

        if (weaponCharging)
        {
            if (UserType == CharacterTypes.Player && !Input.GetMouseButtonDown(0))
            {
                //We have stopped charging the weapon midway, so no recharge delay
                ResetCharge(false);
            }
        }

        if (weaponRecharging)
        {
            rechargeTimer -= Time.deltaTime;
            if (rechargeTimer < 0)
            {
                //Recharge complete
                weaponRecharging = false;
            }
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
            transform.rotation = MouseDirectionQuaternion();
            weaponDirection = MouseDirectionVector2();

            if (flipWeaponSprite)
            {
                //Flip weapon sprite if pointing leftwards
                if (weaponDirection.x < 0)
                {
                    weaponSprite.transform.localScale = new Vector3(weaponSprite.transform.localScale.x, Mathf.Abs(weaponSprite.transform.localScale.y) * -1f);
                }
                else
                {
                    weaponSprite.transform.localScale = new Vector3(weaponSprite.transform.localScale.x, Mathf.Abs(weaponSprite.transform.localScale.y));
                }
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, PlayerDirectionQuaternion(), EnemyWeaponRotateSpeed * Time.deltaTime);
            weaponDirection = transform.rotation.eulerAngles;

            if (flipWeaponSprite)
            {
                //Flip weapon sprite if pointing leftwards
                if (weaponDirection.x < 0)
                {
                    weaponSprite.transform.localScale = new Vector3(weaponSprite.transform.localScale.x, Mathf.Abs(weaponSprite.transform.localScale.y) * -1f);
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
        if (weaponRecharging)
        {
            //weapon cannot be used in recharge
            return false;
        }

        if (!weaponCharging)
        {
            StartChargingWeapon();
            return false;
        }
        else
        {
            return Charge();
        }
    }

    /// <summary>
    /// The instant we charge the weapon
    /// </summary>
    private void StartChargingWeapon()
    {
        weaponCharging = true;
        if (applyChargingMovementPenalty)
        {
            User.ApplyMovementModifer(chargeMovementPenalty);
        }
    }

    /// <summary>
    /// Charging the weapon, will fire the weapon if timer reaches 0
    /// </summary>
    /// <returns></returns>
    private bool Charge()
    {
        chargeTimer -= Time.deltaTime;

        if (chargeTimer > 0)
        {
            //Still charging
            return false;
        }

        //Sucessfully charged

        FireRayCast();
        ResetCharge(true);

        return true;
    }

    private void FireRayCast()
    {
        Debug.Log("here");
    }

    private void ResetCharge(bool applyRechargeTime)
    {
        weaponCharging = false;
        chargeTimer = chargeTime;

        if (applyRechargeTime)
        {
            //Weapon was successfully fired, we apply recharge timer 
            weaponRecharging = true;
            rechargeTimer = rechargeTime;
        }
    }

    public override void StopWeapon()
    {
        base.StopWeapon();
        ResetCharge(false);
    }
}
