using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Simple class for the attachment display icons, holding some references
/// </summary>
public class AttachmentDisplayIcon : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
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
    /// Reverts the icon to nothing
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (reference == null)
        {
            return;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (reference == null)
        {
            return;
        }

        //Set icon to be slightly transparent and makes sure IDropHandler can be raycasted
        CanvasGroup canvasGroup = icon.GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (reference == null)
        {
            return;
        }

        icon.rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (reference == null)
        {
            return;
        }

        //Return the icon to original position
        icon.rectTransform.anchoredPosition = Vector2.zero;

        //Return transparency and raycast blocking
        CanvasGroup canvasGroup = icon.GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    #endregion
}
