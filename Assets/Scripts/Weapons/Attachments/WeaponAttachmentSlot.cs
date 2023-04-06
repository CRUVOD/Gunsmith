using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachmentSlot : MonoBehaviour
{
    public WeaponAttachment attachment;
    public AttachmentPoint attachmentPoint;

    /// <summary>
    /// Tries to set attachment of this slot, return false if type mismatch
    /// </summary>
    /// <param name="attachment"></param>
    /// <returns></returns>
    public bool TrySetAttachment(WeaponAttachment attachment)
    {
        if (attachmentPoint == attachment.reference.attachmentPoint)
        {
            this.attachment = attachment;
            return true;
        }
        return false;
    }

    public WeaponAttachment GetWeaponAttachment()
    {
        return attachment;
    }
}
