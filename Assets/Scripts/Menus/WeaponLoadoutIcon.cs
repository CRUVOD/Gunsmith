using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Display icons for player loadout
/// </summary>
public class WeaponLoadoutIcon : MonoBehaviour, IDropHandler
{
    [Header("Parent select screen")]
    public WeaponSelectScreen weaponSelectScreen;

    [Header("Elements")]
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

    #region Drag&Drop

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }

        WeaponDisplayIcon originDisplayIcon = eventData.pointerDrag.GetComponentInParent<WeaponDisplayIcon>();
        if (originDisplayIcon != null && originDisplayIcon.reference != null)
        {
            //Check if this weapon is already in the loadout
            if (!weaponSelectScreen.WeaponExistsInLoadout(originDisplayIcon.reference))
            {
                UpdateDisplay(originDisplayIcon.reference);
            }
        }
    }

    #endregion
}
