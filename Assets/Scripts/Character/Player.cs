using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("Player Stuff")]
    public HealthBar playerHealthBar;
    public StaminaBar playerStaminaBar;
    public ParticleSystem movingParticles;
    bool ignoreInput = false;
    [Header("Weapons")]
    [HideInInspector]
    public Weapon currentWeapon;
    public List<Weapon> weaponsInLoadout;
    //Where the weapons will be instantiated
    public Transform weaponEquipPoint;

    [Header("Dodge")]
    public float maxStamina;
    [HideInInspector]
    public float currentStamina;
    public float staminaRecoveryDelay;
    public float staminaRecoveryRate;
    private float staminaRecoveryTimer;
    //If dodge is currently available
    protected bool dodgeAvailable;
    //How long each dodge/invuln duration last
    public float dodgeTime;
    //Time between dodge
    public float dodgeCoolDown;
    //Stamina cost each dodge
    public float dodgeStaminaCost;
    float dodgeCoolDownTimer;

    [HideInInspector]
    //If the chracter is firing a weapon
    public bool isFiring;
    //How lomg until isFiring reverts back to false after firing
    public float isFiringThreshold;
    //Timer to count down if player is firing
    float isFiringTimer;

    [HideInInspector]
    public Vector2 playerInput;

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
        staminaRecoveryTimer = staminaRecoveryDelay;
        currentStamina = maxStamina;
        if (weaponsInLoadout.Count > 0)
        {
            //Default selected weapon is the weapon at index 0
            currentWeapon = weaponsInLoadout[0];
            //Set user of weapons to be player, and disable the sprite of the other weapons
            for (int i = 0; i < weaponsInLoadout.Count; i++)
            {
                weaponsInLoadout[i].UserType = CharacterTypes.Player;
                weaponsInLoadout[i].User = this;
                if (i >= 1)
                {
                    weaponsInLoadout[i].weaponSprite.SetActive(false);
                }
            }
            SwitchWeapon(0);
        }
        playerHealthBar = UIManager.instance.PlayerStatUI.healthbar;
        playerStaminaBar = UIManager.instance.PlayerStatUI.staminaBar;
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
                HandleStaminaRecharge();
            }
            HandleParticles();
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

        Vector2 targetVelocity = playerInput.normalized * moveSpeed * movementModifier;
        DebugText.instance.SetText((movementModifier).ToString());

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity.x, ref velocityXSmoothing, accelerationTimeGrounded);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocity.y, ref velocityYSmoothing, accelerationTimeGrounded);
        
        if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
        {
            velocity = Vector3.zero;
            Debug.Log("Velocity has NaN");
        }

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

        if (currentWeapon == null)
        {
            return;
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
        if (Input.GetKeyDown(KeyCode.Space) && dodgeAvailable && currentStamina > 0)
        {
            //Performing dodge
            StartCoroutine(Dodge());
            dodgeAvailable = false;
            dodgeCoolDownTimer = dodgeCoolDown;
            UseStamina(dodgeStaminaCost);
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
    public override void SetHealth(float newValue)
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
    private void HandleParticles()
    {
        if (MovementState == CharacterStates.MovementStates.Moving && ConditionState != CharacterStates.CharacterConditions.Dead && rb.velocity.magnitude >= 0.4f)
        {
            if (!movingParticles.isEmitting)
            {
                movingParticles.Play();
            }
        }
        else
        {
            if (movingParticles.isEmitting)
            {
                movingParticles.Stop();
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
    /// Handles stamina recharge
    /// </summary>
    private void HandleStaminaRecharge()
    {
        playerStaminaBar.SetStaminaBar(currentStamina / maxStamina);
        if (staminaRecoveryTimer > 0)
        {
            //Count down stamina recovery timer
            staminaRecoveryTimer -= Time.deltaTime;
        }
        else
        {
            currentStamina = Math.Clamp(currentStamina + staminaRecoveryRate * Time.deltaTime, 0, maxStamina);
        }
    }

    /// <summary>
    /// Use a certain amount of stamina if it is above 0, and resets the timer to recharge
    /// </summary>
    /// <param name="amount"></param>
    private void UseStamina(float amount)
    {
        if (currentStamina > 0)
        {
            currentStamina = Math.Clamp(currentStamina - amount, 0, maxStamina);
        }
        staminaRecoveryTimer = staminaRecoveryDelay;
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
    /// Wipes the current loadout
    /// </summary>
    public void ClearLoadout()
    {
        //Destroy all old weapon gameobjects
        for (int i = weaponsInLoadout.Count - 1; i >= 0; i--)
        {
            weaponsInLoadout[i].StopWeapon();
            Destroy(weaponsInLoadout[i].gameObject);
        }
        //Clears old loadout
        weaponsInLoadout.Clear();
        UIManager.instance.ClearWeaponUI();
        //Reset movement modifier
        movementModifier = 1f;
    }

    /// <summary>
    /// Adds a weapon to the loadout and switches to it
    /// </summary>
    /// <param name="weaponPrefab"></param>
    public void AddWeaponToLoadout(Weapon weaponPrefab)
    {
        Weapon weapon = Instantiate<Weapon>(weaponPrefab, weaponEquipPoint);
        weapon.UserType = CharacterTypes.Player;
        weapon.User = this;
        weaponsInLoadout.Add(weapon);
        SwitchWeapon(weaponsInLoadout.Count-1);
    }

    public void UpdateLoadout(List<Weapon> newLoadout)
    {
        ClearLoadout();

        //Instantiate the weapons, set user of weapons to be player, and disable the sprite of the other weapons
        for (int i = 0; i < newLoadout.Count; i++)
        {
            Weapon weapon = Instantiate<Weapon>(newLoadout[i], weaponEquipPoint);
            weaponsInLoadout.Add(weapon);
            weapon.User = this;
            weaponsInLoadout[i].UserType = CharacterTypes.Player;
            if (i >= 1)
            {
                weaponsInLoadout[i].weaponSprite.SetActive(false);
            }
        }
        if (weaponsInLoadout.Count > 0)
        {
            SwitchWeapon(0);
        }
    }

    /// <summary>
    /// Updates current loadout to the ones saved in playerdata, does not change anything else about the current player
    /// </summary>
    /// <param name="playerData"></param>
    public void UpdateLoadout(PlayerData playerData)
    {
        ClearLoadout();

        for (int i = 0; i < playerData.weaponsInLoadout.Length; i++)
        {
            //Instantiate the weapon
            WeaponReference weaponReference = DataManager.instance.TryGetWeaponReference(playerData.weaponsInLoadout[i]);
            Weapon weapon = Instantiate<Weapon>(weaponReference.weaponObject, weaponEquipPoint);
            weaponsInLoadout.Add(weapon);
            weaponsInLoadout[i].UserType = CharacterTypes.Player;
            weapon.User = this;

            if (playerData.attachmentsInLoadout.Length > 0 && playerData.attachmentsInLoadout[i] != null)
            {
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
            }

            if (i >= 1)
            {
                weaponsInLoadout[i].weaponSprite.SetActive(false);
            }
        }

        if (weaponsInLoadout.Count > 0)
        {
            SwitchWeapon(0);
        }
    }

    public void LoadPlayer(PlayerData playerData)
    {
        //Health
        InitialHealth = playerData.initialHealth;
        CurrentHealth = InitialHealth;

        //Weapons
        ClearLoadout();

        for (int i = 0; i < playerData.weaponsInLoadout.Length; i++)
        {
            //Instantiate the weapon
            WeaponReference weaponReference = DataManager.instance.TryGetWeaponReference(playerData.weaponsInLoadout[i]);
            Weapon weapon = Instantiate<Weapon>(weaponReference.weaponObject, weaponEquipPoint);
            weaponsInLoadout.Add(weapon);
            weaponsInLoadout[i].UserType = CharacterTypes.Player;
            weapon.User = this;

            if (playerData.attachmentsInLoadout.Length > 0 && playerData.attachmentsInLoadout[i] != null)
            {
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
            }

            if (i >= 1)
            {
                weaponsInLoadout[i].weaponSprite.SetActive(false);
            }
        }

        if (weaponsInLoadout.Count > 0)
        {
            SwitchWeapon(0);
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
