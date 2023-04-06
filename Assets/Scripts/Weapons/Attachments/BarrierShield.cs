using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierShield : MonoBehaviour, IDamageable
{
    private int initalShieldHealth;
    private float shieldHealth;
    private float delayBeforeRecharge;
    private float rechargeRate;

    public SpriteRenderer shieldSprite;

    //The script will interpolate between these colours based on shield health
    public Color fullHealthColour;
    public Color fullDamagedColour;

    float rechargeDelayTimer;

    private void Start()
    {
        
    }

    private void Update()
    {
        CountDownRechargeTimer();
        Recharge();
    }

    public void SetShieldProperties(int health, float rechargeDelay, float rechargeRate)
    {
        initalShieldHealth = health;
        shieldHealth = health;
        delayBeforeRecharge = rechargeDelay;
        this.rechargeRate = rechargeRate;
    }

    public float GetShieldHealth()
    {
        return shieldHealth;
    }

    public bool CanTakeDamageThisFrame()
    {
        if (shieldHealth <= 0)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Just passes to the other function
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="instigator"></param>
    /// <param name="invincibilityDuration"></param>
    /// <param name="force"></param>
    public void Damage(int damage, GameObject instigator, float invincibilityDuration, float force)
    {
        Damage(damage, instigator, invincibilityDuration, Vector3.zero, force);
    }

    /// <summary>
    /// Damages the shield, and reset recharge delay
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="instigator"></param>
    /// <param name="invincibilityDuration"></param>
    /// <param name="direciton"></param>
    /// <param name="force"></param>
    public void Damage(int damage, GameObject instigator, float invincibilityDuration, Vector3 direciton, float force)
    {
        ResetRechargeDelay();
        shieldHealth = Math.Clamp(shieldHealth-damage, 0, initalShieldHealth);
        SetShieldColour();
    }

    private void ResetRechargeDelay()
    {
        rechargeDelayTimer = delayBeforeRecharge;
    }

    private void CountDownRechargeTimer()
    {
        //If shield is damaged, count down the timer to recharge
        if (shieldHealth < initalShieldHealth && rechargeDelayTimer >= 0)
        {
            rechargeDelayTimer -= Time.deltaTime;
        }
    }

    private void Recharge()
    {
        if (rechargeDelayTimer < 0 && shieldHealth < initalShieldHealth)
        {
            //Start recharging the shield and enable the sprite
            shieldHealth = Math.Clamp(shieldHealth + (rechargeRate * Time.deltaTime), 0, initalShieldHealth);
            SetShieldColour();
        }
    }

    /// <summary>
    /// Interpolates the shield colour based on current shield health
    /// </summary>
    private void SetShieldColour()
    {
        //Y = (B - A) * (X - a) / (b - a) + A <- Scaling transform equation
        float r = Math.Clamp((fullHealthColour.r - fullDamagedColour.r) * (shieldHealth) / (initalShieldHealth) + fullDamagedColour.r, 0, 1);
        float g = Math.Clamp((fullHealthColour.g - fullDamagedColour.g) * (shieldHealth) / (initalShieldHealth) + fullDamagedColour.g, 0, 1);
        float b = Math.Clamp((fullHealthColour.b - fullDamagedColour.b) * (shieldHealth) / (initalShieldHealth) + fullDamagedColour.b, 0, 1);
        float a = Math.Clamp((fullHealthColour.a - fullDamagedColour.a) * (shieldHealth) / (initalShieldHealth) + fullDamagedColour.a, 0, 1);
        shieldSprite.color = new Color(r,g,b,a);
    }
}
