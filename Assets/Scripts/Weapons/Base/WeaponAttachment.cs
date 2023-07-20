using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachment : MonoBehaviour
{
    //links to all fields, prefabs etc for UI or anything else to use
    public WeaponAttachmentReference reference;
    //links to the weapon it is attached to
    private Weapon weapon;
    //The sprite of the attachment
    public SpriteRenderer sprite;

    /// <summary>
    /// Generic initialise
    /// </summary>
    /// <param name="weapon"></param>
    public virtual void InitialiseAttachment(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public virtual void InitialiseAttachment(ProjectileWeapon weapon)
    {
        this.weapon = weapon;
    }

    public virtual void InitialiseAttachment(HitScanWeapon weapon)
    {
        this.weapon = weapon;
    }

    /// <summary>
    /// Simply toggles the sprite to be visible or not
    /// </summary>
    public void ToggleSprite(bool state)
    {
        sprite?.gameObject.SetActive(state);
    }
}
