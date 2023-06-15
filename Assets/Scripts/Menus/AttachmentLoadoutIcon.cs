using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AttachmentLoadoutIcon : MonoBehaviour, IDropHandler
{
    public Image icon;
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

    #region Drag&Drop

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }

        AttachmentDisplayIcon originDisplayIcon = eventData.pointerDrag.GetComponentInParent<AttachmentDisplayIcon>();
        if (originDisplayIcon != null && originDisplayIcon.reference != null)
        {
                UpdateDisplay(originDisplayIcon.reference);
        }
    }

    #endregion
}
