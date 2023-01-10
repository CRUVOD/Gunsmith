using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class Grenade : Projectile
{
    [Header("Explosion")]
    [Tooltip("an existing damage area to activate/handle as the grenade explodes")]
    public DamageArea ExplosionArea;
    [Tooltip("Damage caused by the explosion")]
    public int explosionDamage;
    [Tooltip("Force knockback of explosion")]
    public float explosionForce;

    [Tooltip("the feedback to play when exploding")]
    public FeedbackPlayer ExplosionFeedback;
    public GameObject explosionSprite;

    [Header("Damage Area Timing")]

    /// the initial delay to apply before triggering the damage area
    [Tooltip("the initial delay to apply before triggering the damage area")]
    public float InitialDelay = 0f;
    /// the duration during which the damage area is active
    [Tooltip("the duration during which the damage area is active")]
    public float ActiveDuration = .05f;
    /// the duration after the melee weapon cannot be used
    [Tooltip("the duration after which the weapon is in cooldown")]
    public float CoolDownDuration = 0f;

    /// <summary>
    /// Disables damage area at start, and set damage area detection
    /// </summary>
    private void Start()
    {
        DisableExplosionArea();
        ExplosionArea.HitDamageableEvent.AddListener(OnExplosionCollision);
    }

    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Set the explosion damage
    /// </summary>
    /// <param name="explosionDamage"></param>
    public void SetExplosionDamage(int explosionDamage)
    {
        this.explosionDamage = explosionDamage;
    }

    /// <summary>
    /// Describes what happens when colliding with a damageable object
    /// Stops grenade travel, apply impact damage, wait till detonation
    /// </summary>
    /// <param name="health">Health.</param>
    protected override void OnCollideWithDamageable(Character character)
    {
        SetVelocity(new Vector2(0, 0));

        if (!character.CanTakeDamageThisFrame())
        {
            return;
        }

        HitDamageableFeedback?.PlayFeedbacks(this.transform.position);

        //we apply the damage to the thing we've collided with
        int randomDamage = (int)UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));

        //Apply knockback/impact force on collided character
        character.Impact(rb.velocity.normalized, impactForce);

        character.Damage(randomDamage, gameObject, 0f);
    }

    /// <summary>
    /// Describes what happens when colliding with a non damageable object
    /// Stops grenade travel, wait till detonation
    /// </summary>
    protected override void OnCollideWithNonDamageable()
    {
        SetVelocity(new Vector2(0, 0));

        HitNonDamageableFeedback?.PlayFeedbacks(transform.position);
    }

    /// <summary>
    /// We trigger the explosion then destroy the object
    /// </summary>
    /// <param name="time"></param>
    public override void OnProjectileFinish(float time)
    {
        isFinishing = true;
        SetVelocity(new Vector2(0, 0));

        StartCoroutine(Explode());
    }

    #region Explosion

    private IEnumerator Explode()
    {
        explosionSprite?.SetActive(true);
        ExplosionFeedback?.PlayFeedbacks(transform.position);
        EnableExplosionArea();
        yield return new WaitForSeconds(ActiveDuration);
        DisableExplosionArea();
        spriteGameObject.SetActive(false);

        //Let the explosion animation play out for specified amount of time
        //TODO
        Destroy(this.gameObject, 2f);
    }

    /// <summary>
    /// Enables the damage area.
    /// </summary>
    protected virtual void EnableExplosionArea()
    {
        if (ExplosionArea != null)
        {
            ExplosionArea.damageCollider.enabled = true;
        }
    }

    /// <summary>
    /// Disables the damage area.
    /// </summary>
    protected virtual void DisableExplosionArea()
    {
        if (ExplosionArea != null)
        {
            ExplosionArea.damageCollider.enabled = false;
        }
    }

    /// <summary>
    /// Describes what happens when colliding with a damageable object
    /// </summary>
    /// <param name="health">Health.</param>
    protected virtual void OnExplosionCollision(Character character)
    {
        if (!character.CanTakeDamageThisFrame())
        {
            return;
        }

        Vector3 distance = character.transform.position - this.transform.position;
        //we apply the damage to the thing we've collided with

        Vector3 direction = (character.transform.position - this.transform.position).normalized;
        //Apply knockback/impact force on collided character
        character.Impact(direction, explosionForce);

        character.Damage(explosionDamage, gameObject, InvincibilityDuration);
    }

    #endregion
}
