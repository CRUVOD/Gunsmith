using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterTypes { Player, AI }

public class Character : MonoBehaviour, IDamageable
{    
    [Header("Basic Properties")]
    public Rigidbody2D rb;
    public GameObject SpriteGameObject;
    public Animator Animator;
    public bool isSpriteFlipped;
    public Collider2D Collider;
    /// the possible character types : player controller or AI (controlled by the computer)
    [Tooltip("Is the character player-controlled or controlled by an AI ?")]
    public CharacterTypes CharacterType = CharacterTypes.AI;
    public float moveSpeed;

    [Header("Velocity Smoothing")]
    public float accelerationTimeGrounded = 0.1f;
    public float velocityXSmoothing = 0.05f;
    public float velocityYSmoothing = 0.05f;

    [Header("Animation")]
    /// If this is true, sanity checks will be performed to make sure animator parameters exist before updating them. Turning this to false will increase performance but will throw errors if you're trying to update non existing parameters. Make sure your animator has the required parameters.
    [Tooltip("If this is true, sanity checks will be performed to make sure animator parameters exist before updating them. Turning this to false will increase performance but will throw errors if you're trying to update non existing parameters. Make sure your animator has the required parameters.")]
    public bool RunAnimatorSanityChecks = false;
    /// a list of animator parameters
    public HashSet<int> animatorParametres { get; set; }

    [HideInInspector]
    public CharacterStates.MovementStates MovementState;
    public CharacterStates.CharacterConditions ConditionState;

    protected Collider2D groundedTest;

    [Header("Falling")]
    public bool canFallDownHole = false;
    public Transform[] holeTestPoints;
    //Bunch of booleans representing character status
    //If the character is on the ground
    protected bool Grounded;
    //If the character is on top of a hole
    protected bool OverHole;

    //velocity of the character
    protected Vector3 velocity;

    [Header("Impact")]    
    //the speed at which external forces get lerped to zero
    public float ImpactFalloff = 10f;
    //impact on the character movement, used for recoil
    protected Vector3 impact;

    [Header("Health")]
    public int InitialHealth;
    [HideInInspector]
    public int CurrentHealth;

    [Header("Damage")]
    //Invulnerable is like a status effect, immune to damage is permanent
    public bool Invulnerable;
    public bool ImmuneToDamage;

    [Header("Feedbacks")]
    public FeedbackPlayer OnDamageFeedback;

    //hit delegate
    public delegate void OnHitDelegate();
    public OnHitDelegate OnHit;

    public Bounds ColliderBounds
    {
        get
        {
            return Collider.bounds;
        }
    }

    //Bunch of variables to use for animations
    protected const string fallingAnimParametreName = "Falling";
    protected const string aliveAnimParametreName = "Alive";
    protected const string speedAnimParametreName = "Speed";
    protected int fallingAnimParametre;
    protected int aliveAnimParametre;
    protected int speedAnimParametre;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        CurrentHealth = InitialHealth;
        InitialiseAnimatorParametres();
    }

    protected void InitialiseAnimatorParametres()
    {
        if (Animator == null)
        {
            return;
        }

        animatorParametres = new HashSet<int>();

        AnimatorExtensions.AddAnimatorParameterIfExists(Animator, fallingAnimParametreName, out fallingAnimParametre, AnimatorControllerParameterType.Bool, animatorParametres);
        AnimatorExtensions.AddAnimatorParameterIfExists(Animator, aliveAnimParametreName, out aliveAnimParametre, AnimatorControllerParameterType.Bool, animatorParametres);
        AnimatorExtensions.AddAnimatorParameterIfExists(Animator, speedAnimParametreName, out speedAnimParametre, AnimatorControllerParameterType.Float, animatorParametres);
        
        //Initialise additional parametres that are in child classes
        InitialiseAnimatorParametresExtra();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        HandleFacing();
        HandleMovement();
        HandleWeapon();
        HandleOverHole();
        HandleAnimators();
    }

    /// <summary>
    /// Resets the character to start state
    /// </summary>
    public virtual void Reset()
    {
        CurrentHealth = InitialHealth;
        rb.velocity = Vector2.zero;
        Invulnerable = false;
        ImmuneToDamage = false;
        impact = Vector3.zero;
        ConditionState = CharacterStates.CharacterConditions.Normal;
        MovementState = CharacterStates.MovementStates.Idle;
        Grounded = true;
        OverHole = false;
    }

    #region EveryUpdate

    // Flips the character sprite based on cursor location
    protected virtual void HandleFacing()
    {
        return;
    }

    protected virtual void HandleMovement()
    {
        return;
    }

    protected virtual void HandleWeapon()
    {
        return;
    }

    protected virtual void HandleOverHole()
    {
        if (!canFallDownHole)
        {
            return;
        }

        if (holeTestPoints.Length == 0)
        {
            return;
        }

        //Test all the holeTestPoints if they are currently over a hole
        //If a single one isn't, break from this function and OverHole is false
        for (int i = 0; i < holeTestPoints.Length; i++)
        {
            bool testResult = Physics2D.OverlapPoint(holeTestPoints[i].position, LayerManager.HoleLayerMask);
            if (!testResult)
            {
                OverHole = false;
                return;
            }
        }

        OverHole = true;
    }

    protected virtual void HandleDodge()
    {
        return;
    }

    protected void HandleAnimators()
    {
        if (Animator != null)
        {
            AnimatorExtensions.UpdateAnimatorBool(Animator, fallingAnimParametre, OverHole, animatorParametres, RunAnimatorSanityChecks);
            AnimatorExtensions.UpdateAnimatorBool(Animator, aliveAnimParametre, ConditionState != CharacterStates.CharacterConditions.Dead, animatorParametres, RunAnimatorSanityChecks);
            AnimatorExtensions.UpdateAnimatorFloat(Animator, speedAnimParametre, velocity.magnitude, animatorParametres, RunAnimatorSanityChecks);

            //Update additional parametres that are in child classes
            UpdateAnimatorExtra();
        }
    }

    #endregion

    #region Impact

    /// <summary>
    /// Apply an impact force on this chracter
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="force"></param>
    public virtual void Impact(Vector3 direction, float force)
    {
        direction = direction.normalized;
        impact += direction.normalized * force;
    }

    protected Vector3 ApplyImpact(Vector3 velocity)
    {
        if (impact.magnitude > 0.2f)
        {
            velocity += (impact * Time.deltaTime);
        }

        impact = Vector3.Lerp(impact, Vector3.zero, ImpactFalloff * Time.deltaTime);

        return velocity;
    }

    #endregion

    #region Health
    /// <summary>
    /// Sets the current health to the specified new value, and updates the health bar
    /// </summary>
    /// <param name="newValue"></param>
    public virtual void SetHealth(int newValue)
    {
        CurrentHealth = newValue;
        //UpdateHealthBar(false);
        //HealthChangeEvent.Trigger(this, newValue);
    }

    /// <summary>
    /// Upon death, disable colliders, disable any incoming damage
    /// </summary>
    public virtual void Death()
    {
        Death(0);
    }

    public virtual void Death(float timeTillDestroy)
    {
        SetHealth(0);
        DamageDisabled();
        Collider.enabled = false;
        Destroy(gameObject, timeTillDestroy);
    }

    #endregion

    #region IncomingDamage

    /// <summary>
    /// Returns true if this Health component can be damaged this frame, and false otherwise
    /// </summary>
    /// <returns></returns>
    public virtual bool CanTakeDamageThisFrame()
    {
        // if the object is invulnerable, we do nothing and exit
        if (Invulnerable || ImmuneToDamage)
        {
            return false;
        }

        if (!this.enabled)
        {
            return false;
        }

        // if we're already below zero, we do nothing and exit
        if ((CurrentHealth <= 0) && (InitialHealth != 0))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// If no direction, we take the positions of this and the instigator as the direction
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="instigator"></param>
    /// <param name="invincibilityDuration"></param>
    /// <param name="force"></param>
    public virtual void Damage(int damage, GameObject instigator, float invincibilityDuration, float force)
    {
        Vector3 direction = this.transform.position - instigator.transform.position;
        Damage(damage, instigator, invincibilityDuration, direction, force);
    }

    /// <summary>
    /// Called when the object takes damage, also with impact
    /// </summary>
    /// <param name="damage">The amount of health points that will get lost.</param>
    /// <param name="instigator">The object that caused the damage.</param>
    /// <param name="invincibilityDuration">The duration of the short invincibility following the hit.</param>
    public virtual void Damage(int damage, GameObject instigator, float invincibilityDuration, Vector3 direciton, float force)
    {
        if (!CanTakeDamageThisFrame())
        {
            return;
        }

        // we decrease the character's health by the damage
        int previousHealth = CurrentHealth;

        SetHealth(CurrentHealth - damage);

        if (OnHit != null)
        {
            OnHit();
        }

        // we prevent the character from colliding with Projectiles, Player and Enemies
        if (invincibilityDuration > 0)
        {
            DamageDisabled();
            StartCoroutine(DamageEnabled(invincibilityDuration));
        }

        //Do knockback
        Impact(rb.velocity.normalized, force);

        // we trigger a damage taken event
        //MMDamageTakenEvent.Trigger(_character, instigator, CurrentHealth, damage, previousHealth);

        //// we update our animator
        //if (TargetAnimator != null)
        //{
        //    TargetAnimator.SetTrigger("Damage");
        //}

        //we play our feedback
        OnDamageFeedback?.PlayFeedbacks(this.transform.position);

        // we update the health bar
        //UpdateHealthBar(true);

        //// we process any condition state change
        //ComputeCharacterConditionStateChanges(typedDamages);
        //ComputeCharacterMovementMultipliers(typedDamages);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Death();
        }
    }

    #endregion

    #region DamageCalculation

    /// <summary>
    /// Prevents the character from taking any damage
    /// </summary>
    public virtual void DamageDisabled()
    {
        Invulnerable = true;
    }

    /// <summary>
    /// Allows the character to take damage
    /// </summary>
    public virtual void DamageEnabled()
    {
        Invulnerable = false;
    }

    /// <summary>
    /// makes the character able to take damage again after the specified delay
    /// </summary>
    /// <returns>The layer collision.</returns>
    public virtual IEnumerator DamageEnabled(float delay)
    {
        yield return new WaitForSeconds(delay);
        Invulnerable = false;
    }

    #endregion

    #region HelperFunctions

    /// <summary>
    /// Registers a new animator parameter to the list
    /// </summary>
    /// <param name="parametreName">Parameter name.</param>
    /// <param name="parametreType">Parameter type.</param>
    protected void RegisterAnimatorParametre(string parametreName, AnimatorControllerParameterType parametreType, out int parametre)
    {
        parametre = Animator.StringToHash(parametreName);

        if (Animator == null)
        {
            return;
        }
        if (Animator.AnimHasParameterOfType(parametreName, parametreType))
        {
            animatorParametres.Add(parametre);
        }
    }

    /// <summary>
    /// Any additional animation parametres to initialse goes here
    /// Call RegisterAnimatorParametre to do this
    /// </summary>
    protected virtual void InitialiseAnimatorParametresExtra()
    {

    }

    /// <summary>
    /// Any additional updates to animator parametres go here
    /// Call AnimatorExtensions.UpdateAnimator to do this
    /// </summary>
    protected virtual void UpdateAnimatorExtra()
    {

    }

    #endregion
}

