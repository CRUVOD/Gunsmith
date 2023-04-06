using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachment : MonoBehaviour
{
    //links to all fields, prefabs etc for UI or anything else to use
    public WeaponAttachmentReference reference;
    //links to the weapon it is attached to
    private Weapon weapon;

    /// <summary>
    /// Applies any static statistic modification from the attachment to the weapon
    /// </summary>
    /// <param name="weapon"></param>
    public virtual void InitialiseAttachment(Weapon weapon)
    {
        this.weapon = weapon;
    }
}
