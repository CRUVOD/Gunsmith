using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Mathematics;

/// <summary>
/// Child script that contains some enemy specific fields for referencing,
/// overriding character functions, and handling animation
/// </summary>
public class Enemy : Character
{
    [Header("Enemy stuff")]
    public AICore core;
    public NavMeshAgent ag;
    public HealthBar enemyHealthBar;
    public Weapon currentWeapon;
    //bool that determines if we also hide the enemy sprite if inactive
    public bool HideEnemySpriteWhenInactive;

    //player in this script should only be used for minimal things, majority of the functions should be handled by other scripts
    private Player player;

    [Header("Death")]
    //How will the body stay in the scene till destroy, if it is set to less than 0, we don't destroy the body
    public float timeTillBodyDisappear;

    [Header("Stun")]
    //Impact forces will be divided by this number when calculating stun duration
    [Tooltip("How resistance to impact is this enemy")]
    public float stunResistance;
    //Impact forces after resistance calculation above this threshould will apply a stun to the AICore
    public float stunThreshold;
    public float minStunDuration;
    public float maxStunDuration;
    [HideInInspector]
    public bool isStunned;
    protected Coroutine stunCoroutine;

    [HideInInspector]
    //Set to true if this enemy's rigibody is being controlled by another script on the update cycle
    //usually an AIAction script
    public bool externalMovementControl = false;
    //Forces the enemy to face a specific way, usually for dialogue or cutscene reasons
    private bool forceFacing = false;

    //Animation related
    protected const string onDamageAnimParametreName = "OnDamage";
    protected int onDamageAnimParametre;

    //death delegate
    public delegate void OnDeathDelegate(Enemy enemy);
    public OnDeathDelegate OnDeath;

    protected override void Start()
    {
        base.Start();
        SetHealth(InitialHealth);
        ConditionState = CharacterStates.CharacterConditions.Normal;
        if (ag != null)
        {
            ag.updateRotation = false;
            ag.updateUpAxis = false;
        }
        player = LevelManager.instance.player;
    }

    protected override void InitialiseAnimatorParametresExtra()
    {
        AnimatorExtensions.AddAnimatorParameterIfExists(Animator, onDamageAnimParametreName, out onDamageAnimParametre, AnimatorControllerParameterType.Bool, animatorParametres);
    }

    protected override void Update()
    {
        if (ConditionState != CharacterStates.CharacterConditions.Dead)
        {
            base.Update();
        }
        else
        {
            //Make the dead body slow to a stop
            SlowToStop();
            base.HandleAnimators();
        }
    }

    /// <summary>
    /// Slows the character to a stop
    /// </summary>
    private void SlowToStop()
    {
        if (MovementState == CharacterStates.MovementStates.Falling)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector2.zero, 1.5f * Time.deltaTime);
        }
        else
        {
            // Still apply the impact from the external force to the velocity of the dead body
            velocity.x = Mathf.SmoothDamp(velocity.x, 0f, ref velocityXSmoothing, accelerationTimeGrounded);
            velocity.y = Mathf.SmoothDamp(velocity.y, 0f, ref velocityYSmoothing, accelerationTimeGrounded);

            velocity = ApplyImpact(velocity);

            rb.velocity = velocity;
        }
    }

    protected override void HandleOverHole()
    {
        base.HandleOverHole();

        if (OverHole)
        {
            FallDeath();
        }
    }

    protected override void HandleFacing()
    {
        if (forceFacing )
        {
            return;
        }

        //If we have a player in the level, we face the player
        if (player != null)
        {
            float xDirection = (player.transform.position - this.transform.position).x;

            if (xDirection < 0)
            {
                if (isSpriteFlipped)
                {
                    SpriteGameObject.transform.localScale = new Vector3(1f, 1f);
                }
                else
                {
                    SpriteGameObject.transform.localScale = new Vector3(-1f, 1f);
                }
            }
            else
            {
                if (isSpriteFlipped)
                {
                    SpriteGameObject.transform.localScale = new Vector3(-1f, 1f);
                }
                else
                {
                    SpriteGameObject.transform.localScale = new Vector3(1f, 1f);
                }
            }
            return;
        }

        //Default sprite facing behaviour
        if (rb.velocity.x < 0)
        {
            if (isSpriteFlipped)
            {
                SpriteGameObject.transform.localScale = new Vector3(1f, 1f);
            }
            else
            {
                SpriteGameObject.transform.localScale = new Vector3(-1f, 1f);
            }
        }
        else
        {
            if (isSpriteFlipped)
            {
                SpriteGameObject.transform.localScale = new Vector3(-1f, 1f);
            }
            else
            {
                SpriteGameObject.transform.localScale = new Vector3(1f, 1f);
            }
        }
    }

    protected override void HandleMovement()
    {
        //We only apply impact if enemy is stunned and can be knocked back
        if (impact.magnitude > 0.2f && canBeKnockedBack && isStunned)
        {
            rb.velocity = ApplyImpact(rb.velocity);
            DebugText.instance.SetText(impact.magnitude.ToString());
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
        velocity = ag.velocity;
    }

    public override void Death()
    {
        //On death event delegate
        OnDeath?.Invoke(this);

        //Disable navmesh agent
        ag.enabled = false;

        //Disable weapons
        currentWeapon.StopWeapon();
        currentWeapon.weaponSprite.SetActive(false);

        // Max velocity that a dead body can travel is currently defined here, might be something to look into later
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 5f);
        //rb.constraints = RigidbodyConstraints2D.FreezeAll;

        //Change condition state
        ConditionState = CharacterStates.CharacterConditions.Dead;

        //Hide weapons
        //not implemented yet

        SetHealth(0);
        DamageDisabled();

        if (timeTillBodyDisappear >= 0)
        {
            Destroy(this.gameObject, timeTillBodyDisappear);
        }
    }

    /// <summary>
    /// Moves the enemy in a direction
    /// </summary>
    /// <param name="direction"></param>
    public void Move(Vector3 direction)
    {
        Vector2 targetVelocity = direction.normalized * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity.x, ref velocityXSmoothing, accelerationTimeGrounded);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocity.y, ref velocityYSmoothing, accelerationTimeGrounded);

        if (canBeKnockedBack)
        {
            velocity = ApplyImpact(velocity);
        }

        rb.velocity = velocity;
    }

    // Adds to base function to update healthbar
    public override void SetHealth(float newValue)
    {
        base.SetHealth(newValue);
        enemyHealthBar.UpdateBar(newValue, 0f, InitialHealth, true);
    }

    protected void FallDeath()
    {
        MovementState = CharacterStates.MovementStates.Falling;
        Death();
    }

    protected override void UpdateAnimatorExtra()
    {
        AnimatorExtensions.UpdateAnimatorBool(Animator, onDamageAnimParametre, isStunned, animatorParametres, RunAnimatorSanityChecks);
    }

    #region ImpactAndStun

    // Adds to base function to calculate stun effect
    public override void Impact(Vector3 direction, float force)
    {
        base.Impact(direction, force);
        HandleStun(force);
    }

    /// <summary>
    /// Applies a stun effect if the impacting force is above the threshold
    /// </summary>
    /// <param name="impactForce"></param>
    protected virtual void HandleStun(float impactForce)
    {
        float stunAmount = Mathf.Clamp(impactForce - stunResistance, -1000, 1000);

        if (stunAmount > 0)
        {
            float stunDuration = math.remap(0, 1000, minStunDuration, maxStunDuration, stunAmount);

            if (stunCoroutine != null)
            {
                //Starts a new stun coroutine everytime
                StopCoroutine(stunCoroutine);
            }

            stunCoroutine = StartCoroutine(Stun(stunDuration));
        }
    }

    /// <summary>
    /// Pauses the AICore for a duration
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    protected virtual IEnumerator Stun(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    #endregion

    /// <summary>
    /// Pauses/hides/shows the enemy logic when the player is not here yet
    /// </summary>
    public virtual void ActivateEnemy(bool state)
    {
        if (!state)
        {
            //Enemy is inactive
            Invulnerable = true;
            core.CoreActive = false;
            if (HideEnemySpriteWhenInactive)
            {
                SpriteGameObject.SetActive(false);
            }

        }
        else
        {
            //Enemy is active
            Invulnerable = false;
            core.ResetAICore();
            SpriteGameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Forces the enemy to face a specific direciton, direction true is right, false is left
    /// </summary>
    /// <param name="state"></param>
    /// <param name="direction"></param>
    public void ForceFacing(bool state, bool direction = true)
    {
        if (!state)
        {
            forceFacing = false;
            return;
        }

        forceFacing = true;
        if (direction)
        {
            //face right
            if (isSpriteFlipped)
            {
                SpriteGameObject.transform.localScale = new Vector3(-1f, 1f);
            }
            else
            {
                SpriteGameObject.transform.localScale = new Vector3(1f, 1f);
            }
        }
        else
        {
            //face left
            if (isSpriteFlipped)
            {
                SpriteGameObject.transform.localScale = new Vector3(1f, 1f);
            }
            else
            {
                SpriteGameObject.transform.localScale = new Vector3(-1f, 1f);
            }
        }
    }
}
