using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides the basic functions for the player to use and control
/// </summary>
public class Weapon : MonoBehaviour, ExtendedEventListener<GameEvent>
{
    [Header("Basic Properties")]
    //used to calculate where the weapon is pointing towards
    Camera mainCamera;
    //the gameObject to which the sprite is attached to
    public GameObject weaponSprite;
    //weapon animations if there is any
    public Animator Animator;
    //where projectiles or anything would come out of the weapon, on melee weapons, just have it be around the centre of the weapon
    public Transform firePoint;
    //links to all fields, prefabs etc for UI or anything else to use
    public WeaponReference reference;
    public bool flipWeaponSprite = true;
    //The movement scaling when equipping this weapon
    [Range(0f, 1f)]
    public float movementPenalty = 1f;
    //knockback recoil
    public float recoil;
    //which way is the weapon pointing
    [HideInInspector]
    public Vector2 weaponDirection;
    //the type of character that is using this weapon
    public CharacterTypes UserType = CharacterTypes.Player;
    //The instance character using this weapon
    public Character User;

    protected bool currentlyEquipped;

    [Header("Attachments")]
    public List<WeaponAttachmentSlot> weaponAttachmentSlots;

    [Header("Weapon Rules")]
    private IWeaponFireRule[] weaponFireRules;

    protected bool rotationFrozen;

    protected virtual void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        InitaliseAttachments();
        InitialiseRules();
    }

    private void InitialiseRules()
    {
        weaponFireRules = GetComponents<IWeaponFireRule>();
    }

    #region PublicAPIs

    /// <summary>
    /// Applies the stat modifiers from the attachments to the weapon, and initialises them
    /// </summary>
    public virtual void InitaliseAttachments()
    {
        foreach (WeaponAttachmentSlot attachmentSlot in weaponAttachmentSlots)
        {
            if (attachmentSlot.GetWeaponAttachment()!= null)
            {
                attachmentSlot.GetWeaponAttachment().InitialiseAttachment(this);
            }
        }
    }

    /// <summary>
    /// Applies the stat modifiers of the given weapon attachment slot
    /// </summary>
    /// <param name="attachment"></param>
    public virtual void ApplyAttachmentModifier(WeaponAttachmentSlot attachmentSlot)
    {
        attachmentSlot.GetWeaponAttachment().InitialiseAttachment(this);
    }

    /// <summary>
    /// Tries to add the attachment to the weapon at the right attachment slot, and apply the modifiers if succuessful
    /// Returns true if successful, false if not
    /// </summary>
    /// <param name="attachment"></param>
    public virtual bool TryEquipAttachment(WeaponAttachment attachment)
    {
        for (int i = 0; i < weaponAttachmentSlots.Count; i++)
        {
            if (weaponAttachmentSlots[i].TrySetAttachment(attachment))
            {
                attachment.transform.position = weaponAttachmentSlots[i].transform.position;
                attachment.transform.parent = weaponAttachmentSlots[i].transform;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Use the weapon, return true if successfully used
    /// </summary>
    /// <returns></returns>
    public virtual bool Use()
    {
        return false;
    }

    /// <summary>
    /// Use the weapon, return true if successfully used, this method can check user state and decide if weapon should be
    /// succuessfully used
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public virtual bool Use(Character user)
    {
        return Use();
    }

    /// <summary>
    /// Try to reload weapon
    /// </summary>
    public virtual void Reload()
    {
        return;
    }

    /// <summary>
    /// Cancels a reload if one is in progress
    /// </summary>
    public virtual void ReloadCancel()
    {
        return;
    }

    /// <summary>
    /// Call to UIManager to update the UI
    /// </summary>
    public virtual void UpdateUI()
    {
        if (UserType == CharacterTypes.Player)
        {
            UIManager.instance.ChangeWeaponUI(reference);
        }
    }

    /// <summary>
    /// What happens when the weapon is equipped
    /// </summary>
    public virtual void OnEquip()
    {
        currentlyEquipped = true;
        if (UserType == CharacterTypes.AI)
        {
            return;
        }
        else
        {
            if (User != null)
            {
                //Apply movement penalty of weapon on player
                User.ApplyMovementModifer(movementPenalty);
            }
        }

        return;
    }

    /// <summary>
    /// What happens when the weapon is dequipped
    /// </summary>
    public virtual void OnDequip()
    {
        currentlyEquipped = false;
        if (UserType == CharacterTypes.AI)
        {
            return;
        }
        else
        {
            if (User != null)
            {
                //Revert movement penalty on player
                User.ApplyMovementModifer(1/movementPenalty);
            }
        }

        return;
    }

    /// <summary>
    /// Stops the weapon, whatever that might be, stops an animation perhaps etc
    /// </summary>
    public virtual void StopWeapon()
    {
        return;
    }

    /// <summary>
    /// Freezes the rotation of the weapon
    /// </summary>
    public virtual void ToggleWeaponRotation(bool state)
    {
        rotationFrozen = !state;
    }

    #endregion

    protected virtual void RotateWeapon()
    {
        if (rotationFrozen)
        {
            return;
        }

        if (UserType == CharacterTypes.Player)
        {
            transform.rotation = MouseDirectionQuaternion();
            weaponDirection = MouseDirectionVector2();

            if (flipWeaponSprite)
            {
                //Flip weapon sprite if pointing leftwards
                if (weaponDirection.x < 0)
                {
                    weaponSprite.transform.localScale = new Vector3(weaponSprite.transform.localScale.x, Mathf.Abs(weaponSprite.transform.localScale.y) * -1f);
                }
                else
                {
                    weaponSprite.transform.localScale = new Vector3(weaponSprite.transform.localScale.x, Mathf.Abs(weaponSprite.transform.localScale.y));
                }
            }
        }
        else
        {
            transform.rotation = PlayerDirectionQuaternion();
            weaponDirection = PlayerDirectionVector2();

            if (flipWeaponSprite)
            {
                //Flip weapon sprite if pointing leftwards
                if (weaponDirection.x < 0)
                {
                    weaponSprite.transform.localScale = new Vector3(weaponSprite.transform.localScale.x, Mathf.Abs(weaponSprite.transform.localScale.y) * -1f);
                }
                else
                {
                    weaponSprite.transform.localScale = new Vector3(weaponSprite.transform.localScale.x, Mathf.Abs(weaponSprite.transform.localScale.y));
                }
            }
        }
    }

    #region WeaponRules

    /// <summary>
    /// Checks through weapon fire rules, and returns true if all are good with firing the weapon
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    protected bool CheckWeaponFireRules(Character user)
    {
        for (int i = 0; i < weaponFireRules.Length; i++)
        {
            if (!weaponFireRules[i].WeaponCanFire(user, this))
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    #region MouseDirection

    // Returns the direction of the mouse with respect to the firepoint
    // these should not be used by the AI ever like wtf would that even do
    public Quaternion MouseDirectionQuaternion()
    {
        Vector3 rotation;
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if ((mousePos - (Vector2)firePoint.position).magnitude < 3)
        {
            rotation = (mousePos - (Vector2)transform.position).normalized;
        }
        else
        {
            rotation = (mousePos - (Vector2)firePoint.position).normalized;
        }
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, rotZ);
    }

    public Vector2 MouseDirectionVector2()
    {
        Vector3 direction;
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if ((mousePos - (Vector2)firePoint.position).magnitude < 3)
        {
            direction = (mousePos - (Vector2)transform.position).normalized;
        }
        else
        {
            direction = (mousePos - (Vector2)firePoint.position).normalized;
        }
        return new Vector2(direction.x, direction.y).normalized;
    }

    public float MouseDirectionFloat()
    {
        Vector3 rotation;
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if ((mousePos - (Vector2)firePoint.position).magnitude < 3)
        {
            rotation = (mousePos - (Vector2)transform.position).normalized;
        }
        else
        {
            rotation = (mousePos - (Vector2)firePoint.position).normalized;
        }
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        return rotZ;
    }

    #endregion

    #region PlayerDireciton

    // Returns the direction of the player with respect to the firepoint
    // these should not be used by the player
    public Quaternion PlayerDirectionQuaternion()
    {
        Vector3 rotation;
        Vector2 playerPos = LevelManager.instance.player.transform.position;
        if ((playerPos - (Vector2)firePoint.position).magnitude < 3)
        {
            rotation = (playerPos - (Vector2)transform.position).normalized;
        }
        else
        {
            rotation = (playerPos - (Vector2)firePoint.position).normalized;
        }
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, rotZ);
    }

    public Vector2 PlayerDirectionVector2()
    {
        Vector2 direction;
        Vector3 playerPos = LevelManager.instance.player.transform.position;
        if ((playerPos - firePoint.position).magnitude < 3)
        {
            direction = (playerPos - transform.position);
        }
        else
        {
            direction = (playerPos - firePoint.position);
        }
        return direction.normalized;
    }

    public float PlayerDirectionFloat()
    {
        Vector3 rotation;
        Vector2 playerPos = LevelManager.instance.player.transform.position;
        if ((playerPos - (Vector2)firePoint.position).magnitude < 3)
        {
            rotation = (playerPos - (Vector2)transform.position).normalized;
        }
        else
        {
            rotation = (playerPos - (Vector2)firePoint.position).normalized;
        }
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        return rotZ;
    }

    #endregion

    #region Events

    public void OnExtendedEvent(GameEvent GameEvent)
    {
        switch (GameEvent.EventType)
        {
            case GameEvents.Pause:
                ToggleWeaponRotation(false);
                break;

            case GameEvents.UnPause:
                ToggleWeaponRotation(true);
                break;
        }
    }

    /// <summary>
    /// OnDisable, we start listening to events.
    /// </summary>
    protected virtual void OnEnable()
    {
        this.ExtendedEventStartListening<GameEvent>();
    }

    /// <summary>
    /// OnDisable, we stop listening to events.
    /// </summary>
    protected virtual void OnDisable()
    {
        this.ExtendedEventStopListening<GameEvent>();
    }

    #endregion
}
