using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachment : MonoBehaviour
{
    //links to all fields, prefabs etc for UI or anything else to use
    public WeaponAttachmentReference reference;

    /// <summary>
    /// Applies any static statistic modification from the attachment to the weapon
    /// </summary>
    /// <param name="weapon"></param>
    public virtual void ApplyStatModifiers(Weapon weapon)
    {
        return;
    }
}
