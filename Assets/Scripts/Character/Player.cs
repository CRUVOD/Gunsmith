using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("Player Stuff")]
    public HealthBar playerHealthBar;
    public ParticleSystem movingParticles;
    [Header("Weapons")]
    [HideInInspector]
    public Weapon currentWeapon;
    public List<Weapon> weaponsInLoadout;

    [Header("Dodge")]
    //Can the player dodge
    public bool dodgeAbility;
    //If dodge is currently available
    protected bool dodgeAvailable;
    //How long each dodge/invuln duration last
    public float dodgeTime;
    //Time between dodge
    public float dodgeCoolDown;
    float dodgeCoolDownTimer;

    [Header("Audio")]
    public AudioClip moveSound;

    [HideInInspector]
    //If the chracter is firing a weapon
    public bool isFiring;
    //How lomg until isFiring reverts back to false after firing
    public float isFiringThreshold;
    //Timer to count down if player is firing
    float isFiringTimer;

    [HideInInspector]
    public Vector2 playerInput;

    protected AudioSource moveSoundAudioSource;

    //Additional animation parametres
    //If a weapons is being fired
    protected const string firingAnimParametreName = "Firing";
    //If player input is greater than 0
    protected const string playerInputAnimParametreName = "PlayerInput";
    protected int firingAnimParametre;
    protected int playerInputAnimParametre;
    //If player is using dodge roll
    protected const string dodgeAnimParametreName = "Dodge";
    protected int dodgeAnimParametre;

    protected override void Start()
    {
        base.Start();
        //Initialise dodge to be true
        dodgeAvailable = true;
        //Default selected weapon is the weapon at index 0
        currentWeapon = weaponsInLoadout[0];
        //Set user of weapons to be player, and disable the sprite of the other weapons
        for (int i = 0; i < weaponsInLoadout.Count; i++)
        {
            weaponsInLoadout[i].User = CharacterTypes.Player;
            if (i >= 1)
            {
                weaponsInLoadout[i].weaponSprite.SetActive(false);
            }
        }
        currentWeapon.UpdateUI();
    }

    protected override void Update()
    {
        if (ConditionState != CharacterStates.CharacterConditions.Paused)
        {
            if (ConditionState != CharacterStates.CharacterConditions.Dead)
            {
                HandleFacing();
                HandleMovement();
                HandleWeapon();
                HandleWeaponSwitch();
                HandleOverHole();
                HandleDodge();
            }

            HandleAnimators();
        }

        HandlePause();
    }

   
    #region OverridngBase

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
        if (currentWeapon.weaponDirection.x < 0)
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
        if (MovementState == CharacterStates.MovementStates.Dodging)
        {
            //If we are dodging, ignore basic movement controls
            return;
        }

        playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector2 targetVelocity = playerInput.normalized * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity.x, ref velocityXSmoothing, accelerationTimeGrounded);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocity.y, ref velocityYSmoothing, accelerationTimeGrounded);

        velocity = ApplyImpact(velocity);

        rb.velocity = velocity;

        //Do things if player is moving or not
        if (playerInput.magnitude >= 0.2f)
        {
            //If velocity is greater than 0.2f, we count the character as moving
            MovementState = CharacterStates.MovementStates.Moving;

            if (!movingParticles.isEmitting)
            {
                movingParticles.Play();
            }

            // Start playing the moving sound if it hasn't already
            if (moveSoundAudioSource == null)
            {
                moveSoundAudioSource = AudioManagerPlaySoundEvent.Trigger(moveSound, AudioManager.AudioManagerTracks.Sfx, this.transform.position, true, 0.6f);
            }
        }
        else
        {
            MovementState = CharacterStates.MovementStates.Idle;

            if (movingParticles.isEmitting)
            {
                movingParticles.Stop();
            }

            //Stop playing the moving sound if it hasn't already
            if (moveSoundAudioSource != null)
            {
                AudioManagerSoundControlEvent.Trigger(AudioManagerSoundControlEventTypes.Free, 0, moveSoundAudioSource);
                moveSoundAudioSource = null;
            }
        }
    }

    protected override void HandleWeapon()
    {
        // Animation stuff
        if (isFiring)
        {
            // Countdown time since last fired
            isFiringTimer -= Time.deltaTime;

            if (isFiringTimer <= 0)
            {
                isFiring = false;
            }
        }
        
        //Go to the appropriate function for weapon handling based on firing mechanism
        switch (currentWeapon.reference.firingMechanism)
        {
            case FiringMechanism.Single:
                SingleFireWeapon();
                break;
            case FiringMechanism.Auto:
                AutomaticFireWeapon();
                break;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeapon.Reload();
        }
    }

    protected override void HandleDodge()
    {
        if (Input.GetKeyDown(KeyCode.Space) && dodgeAbility && dodgeAvailable)
        {
            //Performing dodge
            StartCoroutine(Dodge());
            dodgeAvailable = false;
            dodgeCoolDownTimer = dodgeCoolDown;
        }

        //Count down dodge cooldown
        if (dodgeCoolDownTimer > 0)
        {
            dodgeCoolDownTimer -= Time.deltaTime;
        }
        else
        {
            dodgeAvailable = true;
        }
    }

    // Adds to base function to update healthbar
    public override void SetHealth(int newValue)
    {
        base.SetHealth(newValue);
        playerHealthBar.UpdateBar(newValue, 0f, InitialHealth, true);
    }

    protected override void InitialiseAnimatorParametresExtra()
    {
        RegisterAnimatorParametre(firingAnimParametreName, AnimatorControllerParameterType.Bool, out firingAnimParametre);
        RegisterAnimatorParametre(playerInputAnimParametreName, AnimatorControllerParameterType.Float, out playerInputAnimParametre);
        RegisterAnimatorParametre(dodgeAnimParametreName, AnimatorControllerParameterType.Bool, out dodgeAnimParametre);
    }

    protected override void UpdateAnimatorExtra()
    {
        AnimatorExtensions.UpdateAnimatorBool(Animator, firingAnimParametreName, isFiring);
        AnimatorExtensions.UpdateAnimatorFloat(Animator, playerInputAnimParametreName, playerInput.magnitude);
        AnimatorExtensions.UpdateAnimatorBool(Animator, dodgeAnimParametreName, MovementState == CharacterStates.MovementStates.Dodging);
        
    }

    public override void Death()
    {        
        //Trigger player death event
        GameEvent.Trigger(GameEvents.PlayerDeath, this);

        //Change condition state
        ConditionState = CharacterStates.CharacterConditions.Dead;

        //erase player input
        playerInput = Vector2.zero;

        //Hide weapons
        for (int i = 0; i < weaponsInLoadout.Count; i++)
        {
            weaponsInLoadout[i].weaponSprite.SetActive(false);
        }

        //Stop the effects that play when moving
        if (movingParticles.isEmitting)
        {
            movingParticles.Stop();
        }
        //Stop playing the moving sound if it hasn't already
        if (moveSoundAudioSource != null)
        {
            AudioManagerSoundControlEvent.Trigger(AudioManagerSoundControlEventTypes.Free, 0, moveSoundAudioSource);
            moveSoundAudioSource = null;
        }


        //Freeze the character in place in case weird shit happens
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        SetHealth(0);
        DamageDisabled();
        Collider.enabled = false;
    }

    #endregion

    #region PlayerSpecific

    /// <summary>
    /// Switches the current weapon equipped by player based on 1,2,3,4 etc on the keyboard
    /// </summary>
    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (weaponsInLoadout[0] == null)
            {
                return;
            }

            if (currentWeapon == weaponsInLoadout[0])
            {
                return;
            }
            else
            {
                currentWeapon.OnDequip();
                currentWeapon.weaponSprite.SetActive(false);
                weaponsInLoadout[0].weaponSprite.SetActive(true);
                currentWeapon = weaponsInLoadout[0];
                currentWeapon.OnEquip();
                //Update UI to reflect change
                currentWeapon.UpdateUI();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (weaponsInLoadout[1] == null)
            {
                return;
            }

            if (currentWeapon == weaponsInLoadout[1])
            {
                return;
            }
            else
            {
                currentWeapon.OnDequip();
                currentWeapon.weaponSprite.SetActive(false);
                weaponsInLoadout[1].weaponSprite.SetActive(true);
                currentWeapon = weaponsInLoadout[1];
                currentWeapon.OnEquip();
                //Update UI to reflect change
                currentWeapon.UpdateUI();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (weaponsInLoadout.Count < 3)
            {
                return;
            }

            if (currentWeapon == weaponsInLoadout[2])
            {
                return;
            }
            else
            {
                currentWeapon.OnDequip();
                currentWeapon.weaponSprite.SetActive(false);
                weaponsInLoadout[2].weaponSprite.SetActive(true);
                currentWeapon = weaponsInLoadout[2];
                currentWeapon.OnEquip();
                //Update UI to reflect change
                currentWeapon.UpdateUI();
            }
        }
    }

    /// <summary>
    /// Handles the player input of pausing
    /// </summary>
    private void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TriggerPause();
        }
    }

    protected void FallDeath()
    {
        MovementState = CharacterStates.MovementStates.Falling;
        Death();
    }

    protected void TriggerPause()
    {
        if (ConditionState == CharacterStates.CharacterConditions.Dead)
        {
            return;
        }
        // we trigger a Pause event for the GameManager and other classes that could be listening to it too
        GameEvent.Trigger(GameEvents.TogglePause, null);
    }

    /// <summary>
    /// Initiates and executes a dodgeroll action
    /// </summary>
    /// <returns></returns>
    private IEnumerator Dodge()
    {
        Invulnerable = true;
        MovementState = CharacterStates.MovementStates.Dodging;
        Vector2 targetDirection = playerInput.normalized * moveSpeed;
        Vector2 dodgeDirection = new Vector2();
        dodgeDirection.x = Mathf.SmoothDamp(velocity.x, targetDirection.x, ref velocityXSmoothing, accelerationTimeGrounded);
        dodgeDirection.y = Mathf.SmoothDamp(velocity.y, targetDirection.y, ref velocityYSmoothing, accelerationTimeGrounded);
        rb.velocity = dodgeDirection.normalized * moveSpeed * 1.5f;
        yield return new WaitForSeconds(dodgeTime);

        Invulnerable = false;
        MovementState = CharacterStates.MovementStates.Idle;
    }

    #endregion

    #region WeaponInput

    void SingleFireWeapon()
    {
        //Left click to fire one projectile at a time
        if (Input.GetMouseButtonDown(0))
        {
            bool success = currentWeapon.Use();

            // Apply recoil if weapon is successfully fired, and add to timer to say weapon has been fired recently
            if (success)
            {
                if (currentWeapon.recoil > 0)
                {
                    Impact(-currentWeapon.weaponDirection, currentWeapon.recoil);
                }
                //Extend time since last fired to threshold
                isFiringTimer = isFiringThreshold;
                isFiring = true;
            }
        }
    }

    void AutomaticFireWeapon()
    {
        //Left click to fire one projectile at a time
        if (Input.GetMouseButton(0))
        {
            bool success = currentWeapon.Use();

            // Apply recoil if weapon is successfully fired, and add to timer to say weapon has been fired recently
            if (success)
            {
                Impact(-currentWeapon.weaponDirection, currentWeapon.recoil);
                //Extend time since last fired to threshold
                isFiringTimer = isFiringThreshold;
                isFiring = true;
            }
        }
    }

    #endregion
}
