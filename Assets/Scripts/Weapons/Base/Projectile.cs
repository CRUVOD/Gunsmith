using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject spriteGameObject;
    [HideInInspector]
    private Collider2D projectileCollider;

    [Header("Basic Properties, these can be set by the weapon instead too")]
    [Tooltip("The min amount of health to remove from the player's health on impact")]
    public int MinDamageCaused = 10;
    //The max amount of health to remove from the player's health
    [Tooltip("The max amount of health to remove from the player's health on impact")]
    public int MaxDamageCaused = 10;
    [Tooltip("The knockback force on target on impact")]
    public float impactForce;
    [Tooltip("Life time till projectile dissipates")]
    public float lifeTime;
    //Timer to count down life time
    float lifeCountDown;
    //If the projectile finish process has started
    protected bool isFinishing;

    [Header("Bullet Penetration")]
    public int canHitNumberOfCharacters = 1;

    [Header("Invincibility")]
    /// The duration of the invincibility frames after the hit (in seconds)
    [Tooltip("The duration of the invincibility frames after the hit (in seconds)")]
    public float InvincibilityDuration = 0.5f;

    [Header("Collision")]
    [Tooltip("the layers that will be damaged by this object")]
    public LayerMask TargetLayerMask;

    [Header("Feedback")]
    [Tooltip("the feedback to play when hitting a Damageable")]
    public FeedbackPlayer HitDamageableFeedback;
    /// the feedback to play when hitting a non Damageable
    [Tooltip("the feedback to play when hitting a non Damageable")]
    public FeedbackPlayer HitNonDamageableFeedback;
    /// the feedback to play when hitting anything
    [Tooltip("the feedback to play when hitting anything")]
    public FeedbackPlayer HitAnythingFeedback;

    //Cache a list of damageables hit by this projectile
    List<IDamageable> damageablesCollided;

    [Flags]
    public enum TriggerAndCollisionMask
    {
        IgnoreAll = 0,
        OnTriggerEnter = 1 << 0,
        OnTriggerStay = 1 << 1,
        OnTriggerEnter2D = 1 << 6,
        OnTriggerStay2D = 1 << 7,

        All_3D = OnTriggerEnter | OnTriggerStay,
        All_2D = OnTriggerEnter2D | OnTriggerStay2D,
        All = All_3D | All_2D
    }

    protected const TriggerAndCollisionMask AllowedTriggerCallbacks = TriggerAndCollisionMask.OnTriggerEnter
                                                                  | TriggerAndCollisionMask.OnTriggerStay
                                                                  | TriggerAndCollisionMask.OnTriggerEnter2D
                                                                  | TriggerAndCollisionMask.OnTriggerStay2D;

    /// Defines on what triggers the damage should be applied, by default on enter and stay (both 2D and 3D) but this field will let you exclude triggers if needed
    [Tooltip(
        "Defines on what triggers the damage should be applied, by default on enter and stay (both 2D and 3D) but this field will let you exclude triggers if needed")]
    public TriggerAndCollisionMask TriggerFilter = AllowedTriggerCallbacks;


    /// Events
    /// an event to trigger when hitting a Damageable
    public UnityEvent<IDamageable> HitDamageableEvent;
    /// an event to trigger when hitting a non Damageable
    public UnityEvent<GameObject> HitNonDamageableEvent;
    /// an event to trigger when hitting anything
    public UnityEvent<GameObject> HitAnythingEvent;

    void Awake()
    {
        damageablesCollided = new List<IDamageable>();
        projectileCollider = GetComponent<Collider2D>();
        lifeCountDown = lifeTime;
        isFinishing = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CountDownLife();
    }

    // Bullet lifetime decay, calls Destroy if lifetime reaches zero
    void CountDownLife()
    {
        if (lifeCountDown > 0)
        {
            lifeCountDown -= Time.deltaTime;
        }

        if (lifeCountDown < 0 && !isFinishing)
        {
            OnProjectileFinish(0);
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.velocity = velocity;
    }

    public void SetVelocity(float projectileSpeed)
    {
        rb.velocity = transform.right * projectileSpeed;
    }

    public void SetDamage(int min, int max)
    {
        MinDamageCaused = min;
        MaxDamageCaused = max;
    }

    public void SetLifeTime(float lifeTime)
    {
        this.lifeTime = lifeTime;
        lifeCountDown = lifeTime;
    }

    public void SetTargetLayer(LayerMask targetMask)
    {
        TargetLayerMask = targetMask;
    }

    #region CollisionDetection

    /// <summary>
    /// When a collision with the player is triggered, we give damage to the player and knock it back
    /// </summary>
    /// <param name="collider">what's colliding with the object.</param>
    public virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerStay2D)) return;
        Colliding(collider.gameObject);
    }

    /// <summary>
    /// On trigger enter 2D, we call our colliding endpoint
    /// </summary>
    /// <param name="collider"></param>S
    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerEnter2D)) return;
        Colliding(collider.gameObject);
    }

    /// <summary>
    /// On trigger stay, we call our colliding endpoint
    /// </summary>
    /// <param name="collider"></param>
    public virtual void OnTriggerStay(Collider collider)
    {
        if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerStay)) return;
        Colliding(collider.gameObject);
    }

    /// <summary>
    /// On trigger enter, we call our colliding endpoint
    /// </summary>
    /// <param name="collider"></param>
    public virtual void OnTriggerEnter(Collider collider)
    {
        if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerEnter)) return;
        Colliding(collider.gameObject);
    }

    #endregion

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
            // Add to list of characters collided
            if (!damageablesCollided.Contains(collidedDamageable))
            {
                damageablesCollided.Add(collidedDamageable);
            }

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
        int randomDamage = (int) UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));

        //Apply knockback/impact force on collided character
        damageable.Damage(randomDamage, gameObject, InvincibilityDuration, rb.velocity.normalized, impactForce);

        if (HitDamageableFeedback != null)
        {
            spriteGameObject.SetActive(false);
            OnProjectileFinish(HitNonDamageableFeedback.TotalDuration);
        }
        else
        {
            OnProjectileFinish(0);
        }
    }

    /// <summary>
    /// Describes what happens when colliding with a non damageable object
    /// By default, disables the sprite gameobject and let the feedbacks on impact play, then destroys game object
    /// </summary>
    protected virtual void OnCollideWithNonDamageable()
    {
        HitNonDamageableFeedback?.PlayFeedbacks(transform.position);
        if (HitNonDamageableFeedback != null)
        {
            spriteGameObject.SetActive(false);
            OnProjectileFinish(HitNonDamageableFeedback.TotalDuration);
        }
        else
        {
            OnProjectileFinish(0);
        }
    }

    /// <summary>
    /// Describes what could happens when colliding with anything
    /// By default, disables the sprite gameobject and let the feedbacks on impact play, then destroys game object
    /// </summary>
    protected virtual void OnAnyCollision(GameObject other)
    {
    }

    #endregion

    /// <summary>
    /// What heppens when the projectile reaches its end of life, and is about to be destroyed
    /// </summary>
    /// <param name="time"></param>
    public virtual void OnProjectileFinish(float time)
    {
        isFinishing = true;
        SetVelocity(new Vector2(0, 0));
        projectileCollider.enabled = false;
        Destroy(this.gameObject, time);
    }
}
