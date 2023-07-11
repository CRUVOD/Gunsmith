using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class MeleeWeapon : Weapon
{
    [Tooltip("an existing damage area to activate/handle as the weapon is used")]
    public DamageArea DamageArea;

    [Header("Feedbacks")]
    [Tooltip("the feedback to play when using the weapon")]
    public FeedbackPlayer WeaponUsedFeedback;
    /// the feedback to play when hitting a Damageable
    [Tooltip("the feedback to play when hitting a Damageable")]
    public FeedbackPlayer HitDamageableFeedback;
    /// the feedback to play when hitting a non Damageable
    [Tooltip("the feedback to play when hitting a non Damageable")]
    public FeedbackPlayer HitNonDamageableFeedback;
    /// an existing damage area to activate/handle as the weapon is used

    [Header("Damage Area Timing")]

    /// the initial delay to apply before triggering the damage area
    [Tooltip("the initial delay to apply before triggering the damage area")]
    public float InitialDelay = 0f;
    /// the duration during which the damage area is active
    [Tooltip("the duration during which the damage area is active")]
    public float ActiveDuration = 1f;
    /// the duration after the melee weapon cannot be used
    [Tooltip("the duration after which the weapon is in cooldown")]
    public float CoolDownDuration = 0f;

    [Header("Damage Caused")]

    /// the layers that will be damaged by this object
    [Tooltip("the layers that will be damaged by this object")]
    public LayerMask TargetLayerMask;
    /// The min amount of health to remove from the player's health
    [Tooltip("The min amount of health to remove from the player's health")]
    public int MinDamageCaused = 10;
    /// The max amount of health to remove from the player's health
    [Tooltip("The max amount of health to remove from the player's health")]
    public int MaxDamageCaused = 10;
    /// The force to apply to the object that gets damaged
    [Tooltip("The force to apply to the object that gets damaged")]
    public float impactForce = 10f;
    /// The duration of the invincibility frames after the hit (in seconds)
    [Tooltip("The duration of the invincibility frames after the hit (in seconds)")]
    public float InvincibilityDuration = 0.5f;

    protected bool attackInProgress = false;
    protected GameObject damageAreaGameObject;


    protected override void Start()
    {
        base.Start();
        CreateDamageArea();
        DisableDamageArea();

        DamageArea.HitDamageableEvent.AddListener(OnCollideWithDamageable);
    }

    protected virtual void Update()
    {
        RotateWeapon();
    }

    /// <summary>
    /// Creates the damage area.
    /// </summary>
    protected virtual void CreateDamageArea()
    {
        if (DamageArea != null)
        {
            damageAreaGameObject = DamageArea.gameObject;
            return;
        }
    }

    #region APIs

    /// <summary>
    /// When the weapon is used, we trigger our attack routine
    /// </summary>
    public override bool Use()
    {
        if (!attackInProgress)
        {
            StartCoroutine(MeleeWeaponAttack());
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnEquip()
    {
        base.OnEquip();
        DisableDamageArea();
    }

    public override void OnDequip()
    {
        base.OnDequip();
        DisableDamageArea();
    }

    public override void StopWeapon()
    {
        StopAllCoroutines();
        DisableDamageArea();
        Animator.SetBool("Attacking", false);
        attackInProgress = false;
        WeaponUsedFeedback?.StopFeedbacks();
    }

    #endregion

    /// <summary>
    /// Triggers an attack, turning the damage area on and then off, also playing the animation for attacking
    /// </summary>
    /// <returns>The weapon attack.</returns>
    protected virtual IEnumerator MeleeWeaponAttack()
    {
        if (attackInProgress) { yield break; }

        attackInProgress = true;
        WeaponUsedFeedback?.PlayFeedbacks();
        Animator.SetBool("Attacking", true);
        yield return new WaitForSeconds(InitialDelay);
        EnableDamageArea();
        yield return new WaitForSeconds(ActiveDuration);
        DisableDamageArea();
        Animator.SetBool("Attacking", false);
        yield return new WaitForSeconds(CoolDownDuration);
        attackInProgress = false;
    }

    /// <summary>
    /// Enables the damage area.
    /// </summary>
    protected virtual void EnableDamageArea()
    {
        if (DamageArea != null)
        {
            DamageArea.damageCollider.enabled = true;
        }
    }


    /// <summary>
    /// Disables the damage area.
    /// </summary>
    protected virtual void DisableDamageArea()
    {
        if (DamageArea != null)
        {
            DamageArea.damageCollider.enabled = false;
        }
    }

    /// <summary>
    /// On disable we set our flag to false
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        attackInProgress = false;
    }

    #region OnCollision

    /// <summary>
    /// Describes what happens when colliding with a damageable object
    /// </summary>
    /// <param name="health">Health.</param>
    protected virtual void OnCollideWithDamageable(IDamageable damageable)
    {
        if (!damageable.CanTakeDamageThisFrame())
        {
            return;
        }

        HitDamageableFeedback?.PlayFeedbacks(this.transform.position);

        //we apply the damage to the thing we've collided with
        int randomDamage = (int)UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));
        
        //Apply knockback/impact force on collided character and damage
        damageable.Damage(randomDamage, gameObject, InvincibilityDuration, weaponDirection, impactForce);
    }

    /// <summary>
    /// Describes what happens when colliding with a non damageable object
    /// </summary>
    protected virtual void OnCollideWithNonDamageable()
    {
        HitNonDamageableFeedback?.PlayFeedbacks(transform.position);
    }

    /// <summary>
    /// Describes what could happens when colliding with anything
    /// </summary>
    protected virtual void OnAnyCollision(GameObject other)
    {
    }

    #endregion


}
