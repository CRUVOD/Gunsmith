using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitScanWeapon : Weapon
{
    //how far the raycast check travels for
    public float rayTravelDistance;

    //bullets per minute
    public float fireRate;
    //time between shots, calculated from fireRate
    protected float timeBetweenShots;
    //counting down reload time
    protected float timeToReload;

    //If the weapon reloads by a magazine
    public bool isMagazineBased;
    //The total number of bullets/shots in the chamber
    public int ammoCapacity;
    public float reloadTime;
    [HideInInspector]
    public int currentAmmoReady;
    [HideInInspector]
    public bool inReload;

    [Header("Basic Properties")]
    [Tooltip("The min amount of health to remove from the player's health")]
    public int MinDamageCaused = 10;
    //The max amount of health to remove from the player's health
    [Tooltip("The max amount of health to remove from the player's health")]
    public int MaxDamageCaused = 10;
    [Tooltip("The knockback force on target")]
    public float impactForce;

    [Header("Collision")]
    [Tooltip("the layers that will be damaged by this weapon")]
    public LayerMask TargetLayerMask;

    [Header("Invincibility")]
    /// The duration of the invincibility frames after the hit (in seconds)
    [Tooltip("The duration of the invincibility frames after the hit (in seconds)")]
    public float InvincibilityDuration = 0.3f;

    [Header("Feedback")]
    [Tooltip("the feedback to play when hitting a Damageable")]
    public FeedbackPlayer HitDamageableFeedback;
    /// the feedback to play when hitting a non Damageable
    [Tooltip("the feedback to play when hitting a non Damageable")]
    public FeedbackPlayer HitNonDamageableFeedback;
    /// the feedback to play when hitting anything
    [Tooltip("the feedback to play when hitting anything")]
    public FeedbackPlayer HitAnythingFeedback;

    [Header("Events")]
    /// Events
    /// an event to trigger when hitting a Damageable
    public UnityEvent<IDamageable> HitDamageableEvent;
    /// an event to trigger when hitting a non Damageable
    public UnityEvent<GameObject> HitNonDamageableEvent;
    /// an event to trigger when hitting anything
    public UnityEvent<GameObject> HitAnythingEvent;

    protected virtual void Update()
    {
        if (currentlyEquipped)
        {
            countDownTimeBetweenShots();
            countDownMagazineReloadTime();
            RotateWeapon();
            UpdateUI();
        }
    }

    public void ResetTimeBetweenShots()
    {
        timeBetweenShots = 60f / fireRate;
    }

    public float GetTimeBetweenShots()
    {
        return timeBetweenShots;
    }

    /// <summary>
    /// Changes the current ammo in the magazine, updating the UI
    /// </summary>
    /// <param name="newAmmo"></param>
    public void ChangeAmmoCount(int newAmmo)
    {
        currentAmmoReady = newAmmo;
        UIManager.instance.UpdateBallisticAmmoUI(newAmmo, ammoCapacity);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
        UIManager.instance.UpdateBallisticAmmoUI(currentAmmoReady, ammoCapacity);
    }

    public override void Reload()
    {
        if (isMagazineBased)
        {
            MagazineReload();
        }
        else
        {
            NonMagazineReload();
        }
    }

    public override void ReloadCancel()
    {
        //if not in reload, get out of this function
        if (!inReload)
        {
            return;
        }

        inReload = false;
    }

    /// <summary>
    /// Non-standard magazine reload method, can be overridden to do anything
    /// </summary>
    public virtual void NonMagazineReload()
    {
        Debug.LogWarning("Non-magazine reload not implemented");
    }

    /// <summary>
    /// Reloads the magazine to max by default, can be overriden
    /// </summary>
    public virtual void MagazineReload()
    {
        //Check if weapon is already reloading, then start reload, set timebetweenshots to zero
        if (!inReload)
        {
            timeToReload = reloadTime;
            inReload = true;
            timeBetweenShots = 0;
        }
    }

    // Return true if weapon is ready to fire projectile again
    public virtual bool ReadyToFire()
    {
        if (timeBetweenShots > 0)
        {
            return false;
        }

        return true;
    }

    // counts down the timer between shots, call every update
    public virtual void countDownTimeBetweenShots()
    {
        if (timeBetweenShots > 0 && !inReload)
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }

    // counts down the timer between reloads, call every update
    public virtual void countDownMagazineReloadTime()
    {
        if (inReload && isMagazineBased)
        {
            if (timeToReload > 0)
            {
                timeToReload -= Time.deltaTime;
            }
            else if (timeToReload <= 0)
            {
                //reload complete, give full ammo to player
                ChangeAmmoCount(ammoCapacity);
                inReload = false;
            }
        }
    }

    /// <summary>
    /// Use the projectile weapon, if a projectile is fired, return true
    /// </summary>
    /// <returns></returns>
    public override bool Use()
    {
        return FireRaycast();
    }

    // Returns true if projectile is succesfully fired
    public virtual bool FireRaycast()
    {
        return false;
    }

    /// <summary>
    /// On equipping weapon, overwrite the current cooldown on display
    /// </summary>
    public override void OnEquip()
    {
        base.OnEquip();

        if (inReload)
        {
            UIManager.instance.crosshair.SetCoolDownUI(timeToReload, reloadTime);
        }
        else if ((fireRate < 300f) && (timeBetweenShots > 0))
        {
            //Only show a cooldown on slower fire rate weapons really
            UIManager.instance.crosshair.SetCoolDownUI(timeBetweenShots, 60f / fireRate);
        }
        else
        {
            UIManager.instance.crosshair.ResetCoolDownUI();
        }
    }

    /// <summary>
    /// On dequiping the weapon, cancels the reload if it can be reloaded
    /// </summary>
    public override void OnDequip()
    {
        base.OnDequip();
        ReloadCancel();
    }

    #region OnCollision

    /// <summary>
    /// When colliding, we apply the appropriate damage
    /// </summary>
    /// <param name="collider"></param>
    protected virtual void Colliding(GameObject collider)
    {
        if (!EvaluateAvailability(collider))
        {
            return;
        }

        IDamageable collidedDamageable = collider.gameObject.GetComponent<IDamageable>();

        // if what we're colliding with is damageable
        if (collidedDamageable != null)
        {
            if (collidedDamageable.CanTakeDamageThisFrame())
            {
                OnCollideWithDamageable(collidedDamageable);
            }
            HitDamageableEvent?.Invoke(collidedDamageable);
        }
        else // if what we're colliding with can't be damaged
        {
            OnCollideWithNonDamageable();
            HitNonDamageableEvent?.Invoke(collider);
        }

        OnAnyCollision(collider);
        HitAnythingEvent?.Invoke(collider);
        HitAnythingFeedback?.PlayFeedbacks(transform.position);
    }

    /// <summary>
    /// Checks whether or not damage should be applied this frame
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    protected virtual bool EvaluateAvailability(GameObject collider)
    {
        // if we're inactive, we do nothing
        if (!isActiveAndEnabled) { return false; }

        // if what we're colliding with isn't part of the target layers, we do nothing and exit
        if (!ExtraLayers.LayerInLayerMask(collider.layer, TargetLayerMask)) { return false; }

        // if we're on our first frame, we don't apply damage
        if (Time.time == 0f) { return false; }

        return true;
    }

    /// <summary>
    /// Describes what happens when colliding with a damageable object
    /// By default, disables the sprite gameobject and let the feedbacks on impact play, then destroys game object
    /// </summary>
    /// <param name="health">Health.</param>
    protected virtual void OnCollideWithDamageable(IDamageable damageable)
    {
        HitDamageableFeedback?.PlayFeedbacks(this.transform.position);

        //we apply the damage to the thing we've collided with
        int randomDamage = (int)UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));

        //Apply knockback/impact force on collided character
        damageable.Damage(randomDamage, gameObject, InvincibilityDuration, weaponDirection, impactForce);
    }

    /// <summary>
    /// Describes what happens when colliding with a non damageable object
    /// By default, disables the sprite gameobject and let the feedbacks on impact play, then destroys game object
    /// </summary>
    protected virtual void OnCollideWithNonDamageable()
    {
        HitNonDamageableFeedback?.PlayFeedbacks(transform.position);
    }

    /// <summary>
    /// Describes what could happens when colliding with anything
    /// By default, disables the sprite gameobject and let the feedbacks on impact play, then destroys game object
    /// </summary>
    protected virtual void OnAnyCollision(GameObject other)
    {
    }

    #endregion
}
