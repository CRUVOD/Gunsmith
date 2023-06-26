using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Display icons for player unlcoked weapons for the weapon select screen
/// </summary>
public class WeaponDisplayIcon : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Image icon;
    public Sprite emptySprite;
    public WeaponReference reference;
    private TooltipTrigger tooltipTrigger;

    private void Awake()
    {
        tooltipTrigger = GetComponent<TooltipTrigger>();
    }

    private void SetIconSprite(Sprite sprite)
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
    public void UpdateDisplay(WeaponReference reference)
    {
        this.reference = reference;
        SetIconSprite(reference.icon);
        if (tooltipTrigger != null)
        {
            tooltipTrigger.header = reference.ID;
            tooltipTrigger.content = reference.shortDescription;
        }
    }

    /// <summary>
    /// Updates the display based on the reference
    /// </summary>
    /// <param name="reference"></param>
    public void UpdateDisplay(string weaponID)
    {
        this.reference = DataManager.instance.TryGetWeaponReference(weaponID);
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

    #region Dragon&Drop

    //For dragging and dropping when equiping for loadout

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
