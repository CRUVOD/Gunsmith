using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("Player Stuff")]
    public HealthBar playerHealthBar;
    public ParticleSystem movingParticles;
    bool ignoreInput = false;
    [Header("Weapons")]
    [HideInInspector]
    public Weapon currentWeapon;
    public List<Weapon> weaponsInLoadout;
    //Where the weapons will be instantiated
    public Transform weaponEquipPoint;

    [Header("Dodge")]
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
        movingParticles.Stop();
        if (weaponsInLoadout.Count > 0)
        {
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

    }

    protected override void Update()
    {
        if (ConditionState != CharacterStates.CharacterConditions.Paused)
        {
            if (ConditionState != CharacterStates.CharacterConditions.Dead)
            {
                if (ConditionState != CharacterStates.CharacterConditions.Frozen && !ignoreInput)
                {
                    HandleFacing();
                    HandleDodge();
                    HandleMovement();
                    HandleWeapon();
                    HandleWeaponSwitch();
                }
                HandleOverHole();
                if (!ignoreInput)
                {
                    HandleInteraction();
                }
            }
            HandleParticlesAndSound();
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
        //Set facing based on weapon dirction if player is holding a weapon, else use last input
        if (currentWeapon)
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
        else
        {
            if (playerInput.x < 0)
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
            else if (playerInput.x > 0)
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

    }

    protected override void HandleMovement()
    {
        playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector2 targetVelocity = playerInput.normalized * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity.x, ref velocityXSmoothing, accelerationTimeGrounded);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocity.y, ref velocityYSmoothing, accelerationTimeGrounded);
        
        velocity = ApplyImpact(velocity);

        rb.velocity = velocity;

        if (MovementState == CharacterStates.MovementStates.Dodging)
        {
            //Don't change movement state if dodging, the coroutine should change it back automatically
            return;
        }
        else if (rb.velocity.magnitude >= 0.4f)
        {
            MovementState = CharacterStates.MovementStates.Moving;
        }
        else
        {
            MovementState = CharacterStates.MovementStates.Idle;
        }
    }

    protected override void HandleWeapon()
    {
        if (currentWeapon == null)
        {
            return;
        }

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
        if (Input.GetKeyDown(KeyCode.Space) && dodgeAvailable)
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
    /// Sets the moving particles on or off based on player condition and velocity
    /// </summary>
    private void HandleParticlesAndSound()
    {
        if (MovementState == CharacterStates.MovementStates.Moving && ConditionState != CharacterStates.CharacterConditions.Dead && rb.velocity.magnitude >= 0.4f)
        {
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

    /// <summary>
    /// Switches the current weapon equipped by player based on 1,2,3,4 etc on the keyboard
    /// </summary>
    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
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
        //also we stop the audio from playing
        if (moveSoundAudioSource != null)
        {
            AudioManagerSoundControlEvent.Trigger(AudioManagerSoundControlEventTypes.Free, 0, moveSoundAudioSource);
            moveSoundAudioSource = null;
        }

        GameEvent.Trigger(GameEvents.TogglePause, null);
    }

    /// <summary>
    /// Checks if any overlapping colliders/gameobjects has the interactable tag, and tries to interact with it
    /// </summary>
    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //maximum number of collisions
            int numColliders = 10;
            Collider2D[] colliders = new Collider2D[numColliders];
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.NoFilter();
            Physics2D.OverlapCollider(this.Collider, contactFilter, colliders);

            foreach (Collider2D collider in colliders)
            {
                if (collider != null && collider.gameObject.tag == "Interactable")
                {
                    Interactable interaction;
                    if (collider.gameObject.TryGetComponent<Interactable>(out interaction))
                    {
                        //For now, can only interact with one thing each click of 'interact'
                        interaction.Interact(this);
                        return;
                    }
                }
            }
        }

        //Debug button, use this for anything debugging for now
        if (Input.GetKeyDown(KeyCode.Period))
        {
            PlayerDebug();
        }
    }

    /// <summary>
    /// Sets the ignore player input boolean and stops the player
    /// </summary>
    /// <param name="state"></param>
    public void IgnoreInput(bool state)
    {
        ignoreInput = state;

        rb.velocity = Vector3.zero;
        playerInput = Vector2.zero;
        velocity = Vector3.zero;
    }

    /// <summary>
    /// Move the player in a certain direction at a certain speed for a certain duration
    /// This method will not break the walking animation of the player, so will be
    /// primarily used by timeline cutscenes
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    /// <param name="duration"></param>
    public void ControlPlayerMovement(Vector2 direction, float speed, float duration)
    {
        //Pause player control
        IgnoreInput(true);
        StartCoroutine(ControlMovement(direction, speed, duration));
    }

    private IEnumerator ControlMovement(Vector2 direction, float speed, float duration)
    {
        velocity = direction * speed;
        rb.velocity = velocity;
        yield return new WaitForSeconds(duration);
        //resume player control
        IgnoreInput(false);
        velocity = Vector2.zero;
        rb.velocity = velocity;
    }

    /// <summary>
    /// Freezes the player in place and freezing the rigidbody too
    /// </summary>
    /// <param name="state"></param>
    public void FreezePlayerMovement(bool state)
    {
        if (state)
        {
            //Freeze the character in place
            rb.velocity = Vector3.zero;
            playerInput = Vector2.zero;
            velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            ConditionState = CharacterStates.CharacterConditions.Frozen;
            
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ConditionState = CharacterStates.CharacterConditions.Normal;
        }
    }

    /// <summary>
    /// Initiates and executes a dodgeroll action
    /// </summary>
    /// <returns></returns>
    private IEnumerator Dodge()
    {
        Invulnerable = true;
        MovementState = CharacterStates.MovementStates.Dodging;

        //reset impact, and increase base movement speed
        impact = Vector3.zero;
        float prevMoveSpeed = moveSpeed;
        moveSpeed *= 1.8f;

        yield return new WaitForSeconds(dodgeTime - 0.01f);

        moveSpeed = prevMoveSpeed;
        Invulnerable = false;
        MovementState = CharacterStates.MovementStates.Idle;
    }

    /// <summary>
    /// Switches currently equipped weapon to the one in the weapon slot
    /// </summary>
    /// <param name="weaponSlot"></param>
    private void SwitchWeapon(int weaponSlot)
    {
        if (weaponSlot >= weaponsInLoadout.Count || weaponsInLoadout[weaponSlot] == null)
        {
            return;
        }
        else if (currentWeapon == weaponsInLoadout[weaponSlot])
        {
            return;
        }
        else
        {
            if (currentWeapon != null)
            {
                currentWeapon.OnDequip();
                currentWeapon.weaponSprite.SetActive(false);
            }
            weaponsInLoadout[weaponSlot].weaponSprite.SetActive(true);
            currentWeapon = weaponsInLoadout[weaponSlot];
            currentWeapon.OnEquip();
            //Update UI to reflect change
            currentWeapon.UpdateUI();
        }
    }

    /// <summary>
    /// Adds a weapon to the loadout and switches to it
    /// </summary>
    /// <param name="weaponPrefab"></param>
    public void AddWeaponToLoadout(Weapon weaponPrefab)
    {
        Weapon weapon = Instantiate<Weapon>(weaponPrefab, weaponEquipPoint);
        weapon.User = CharacterTypes.Player;
        weaponsInLoadout.Add(weapon);
        SwitchWeapon(weaponsInLoadout.Count-1);
    }

    public void UpdateLoadout(List<Weapon> newLoadout)
    {
        //Destroy all old weapon gameobjects
        for (int i = weaponsInLoadout.Count-1; i >= 0; i--)
        {
            Destroy(weaponsInLoadout[i].gameObject);
        }
        //Clears old loadout
        weaponsInLoadout.Clear();

        //Instantiate the weapons, set user of weapons to be player, and disable the sprite of the other weapons
        for (int i = 0; i < newLoadout.Count; i++)
        {
            Weapon weapon = Instantiate<Weapon>(newLoadout[i], weaponEquipPoint);
            weaponsInLoadout.Add(weapon);
            weaponsInLoadout[i].User = CharacterTypes.Player;
            if (i >= 1)
            {
                weaponsInLoadout[i].weaponSprite.SetActive(false);
            }
        }
        if (weaponsInLoadout.Count > 0)
        {
            currentWeapon = weaponsInLoadout[0];
            currentWeapon.UpdateUI();
        }
    }

    public void UpdateLoadout(PlayerData playerData)
    {
        //Destroy all old weapon gameobjects
        for (int i = weaponsInLoadout.Count - 1; i >= 0; i--)
        {
            Destroy(weaponsInLoadout[i].gameObject);
        }
        //Clears old loadout
        weaponsInLoadout.Clear();

        for (int i = 0; i < playerData.weaponsInLoadout.Length; i++)
        {
            //Instantiate the weapons
            WeaponReference weaponReference = DataManager.instance.TryGetWeaponReference(playerData.weaponsInLoadout[i]);
            Weapon weapon = Instantiate<Weapon>(weaponReference.weaponObject, weaponEquipPoint);
            weaponsInLoadout.Add(weapon);
            weaponsInLoadout[i].User = CharacterTypes.Player;

            //Instantiate the attachments for that weapon and equips them
            for (int j = 0; j < playerData.attachmentsInLoadout[i].Length; j++)
            {
                WeaponAttachmentReference weaponAttachmentReference = DataManager.instance.TryGetAttachmentReference(playerData.attachmentsInLoadout[i][j]);
                WeaponAttachment attachment = Instantiate<WeaponAttachment>(weaponAttachmentReference.attachmentObject);
                if (!weapon.TryEquipAttachment(attachment))
                {
                    Destroy(attachment);
                }
            }

            if (i >= 1)
            {
                weaponsInLoadout[i].weaponSprite.SetActive(false);
            }
        }

        if (weaponsInLoadout.Count > 0)
        {
            currentWeapon = weaponsInLoadout[0];
            currentWeapon.UpdateUI();
        }
    }

    #endregion

    #region WeaponInput

    void SingleFireWeapon()
    {
        //Left click to fire one projectile at a time
        if (Input.GetMouseButtonDown(0))
        {
            bool success = currentWeapon.Use(this);

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
            bool success = currentWeapon.Use(this);

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

    /// <summary>
    /// Debug action, modify to do anything
    /// </summary>
    private void PlayerDebug()
    {
        
    }

}
