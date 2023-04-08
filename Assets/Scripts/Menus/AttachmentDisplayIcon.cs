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
    private TooltipTrigger tooltipTrigger;
    public AttachmentPoint attachmentPoint;

    private void Awake()
    {
        tooltipTrigger = GetComponent<TooltipTrigger>();
    }

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
        if (tooltipTrigger != null)
        {
            tooltipTrigger.header = reference.ID;
            tooltipTrigger.content = reference.shortDescription;
        }
    }

    /// <summary>
    /// Reverts the icon to nothing with no function
    /// </summary>
    public void RevertToEmptyComplete()
    {
        SetEmptySprite();
        if (tooltipTrigger != null)
        {
            tooltipTrigger.header = "";
            tooltipTrigger.content = "";
        }
        reference = null;
        button.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Only reverts the icon to nothing
    /// </summary>
    public void RevertToEmpty()
    {
        SetEmptySprite();
        if (tooltipTrigger != null)
        {
            tooltipTrigger.header = "";
            tooltipTrigger.content = "";
        }
        reference = null;
    }
}
