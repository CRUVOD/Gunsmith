using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple class for the attachment display icons, holding some references
/// </summary>
public class AttachmentDisplayIcon : MonoBehaviour
{
    public Image icon;
    public Button button;
    public WeaponAttachmentReference reference;
    public Sprite emptySprite;
    public AttachmentPoint attachmentPoint;

    public void SetIconSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    public void SetEmptySprite()
    {
        SetIconSprite(emptySprite);
    }

    /// <summary>
    /// Updates the display based on the reference
    /// </summary>
    /// <param name="reference"></param>
    public void UpdateDisplay(WeaponAttachmentReference reference)
    {
        this.reference = reference;
        this.attachmentPoint = reference.attachmentPoint;
        SetIconSprite(reference.icon);
    }

    /// <summary>
    /// Reverts the icon to nothing with no function
    /// </summary>
    public void RevertToEmptyComplete()
    {
        SetEmptySprite();
        reference = null;
        button.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Only reverts the icon to nothing
    /// </summary>
    public void RevertToEmpty()
    {
        SetEmptySprite();
        reference = null;
    }
}
