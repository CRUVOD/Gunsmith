using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttachmentSelectScreen : MonoBehaviour
{
    //The weapon being customised right now
    public WeaponReference currentWeapon;
    public Image weaponIcon;

    [Header("Attachment Positions")]
    public AttachmentDisplayIcon[] attachmentPointIcons;

    [Header("Attachment Display")]
    public GridLayoutGroup AttachmentDisplay;
    public AttachmentDisplayIcon attachmentDisplayIconPrefab;
    public List<AttachmentDisplayIcon> attachmentDisplayIcons;

    private void Start()
    {
        GenerateEmptyDisplayIcons();
    }

    /// <summary>
    /// Generates a fixed number of empty display icons for use later
    /// Change to varying amount of empty later
    /// </summary>
    private void GenerateEmptyDisplayIcons()
    {
        attachmentDisplayIcons = new List<AttachmentDisplayIcon>();
        for (int i = 0; i < 20; i++)
        {
            attachmentDisplayIcons.Add(Instantiate(attachmentDisplayIconPrefab, AttachmentDisplay.gameObject.transform));
        }
    }

    /// <summary>
    /// Updates which weapon we are customising
    /// </summary>
    /// <param name="reference"></param>
    public void UpdateCustomisationWeapon(WeaponReference reference, List<WeaponAttachmentReference> attachmentReferences)
    {
        currentWeapon = reference;
        weaponIcon.sprite = currentWeapon.icon;
        //Disable attachment point icons for points not on the weapon, and load the attachments given 
        for (int i = 0; i < attachmentPointIcons.Length; i++)
        {
            AttachmentDisplayIcon displayIcon = attachmentPointIcons[i];
            //Fist enable all and clear whatever was displaying before
            displayIcon.gameObject.SetActive(true);
            displayIcon.RevertToEmpty();

            int pos = Array.IndexOf(reference.attachmentPoints, displayIcon.attachmentPoint);
            if (pos > -1)
            {
                //The point exists on the weapon
                if (attachmentReferences == null)
                {
                    //No attachments were given
                    continue;
                }

                for (int j = 0; j < attachmentReferences.Count; j++)
                {
                    if (attachmentReferences[j].attachmentPoint == reference.attachmentPoints[pos])
                    {
                        //There is an attachment at this point given
                        displayIcon.UpdateDisplay(attachmentReferences[j]);
                    }
                }
            }
            else
            {
                displayIcon.gameObject.SetActive(false);
            }
        }
        ClearAttachmentDisplay();
    }

    /// <summary>
    /// Clears the rightside display of attachments available
    /// </summary>
    public void ClearAttachmentDisplay()
    {
        foreach (AttachmentDisplayIcon displayIcon in attachmentDisplayIcons)
        {
            displayIcon.RevertToEmptyComplete();
        }
    }

    /// <summary>
    /// Displays the relevant attachments from the database that can be slotted into that attachment point
    /// </summary>
    public void ChosenAttachmentPoint(string attachmentPointString)
    {
        AttachmentPoint attachmentPoint = (AttachmentPoint)Enum.Parse(typeof(AttachmentPoint), attachmentPointString);

        // The foo.ToString().Contains(",") check is necessary for 
        // enumerations marked with a [Flags] attribute.
        if (!Enum.IsDefined(typeof(AttachmentPoint), attachmentPoint) && !attachmentPoint.ToString().Contains(","))
        {
            throw new InvalidOperationException(
                $"{attachmentPointString} is not an underlying value of the YourEnum enumeration."
            );
        }

        foreach (AttachmentDisplayIcon displayIcon in attachmentPointIcons)
        {
            if (displayIcon.attachmentPoint == attachmentPoint)
            {
                if (displayIcon.reference)
                {
                    //There is already something equipped, so we dequip it first
                    displayIcon.RevertToEmpty();
                }
            }
        }

        List<WeaponAttachmentReference> compatibleAttachments = GetCompatibleAttachments(attachmentPoint, currentWeapon.weaponCategory);

        ClearAttachmentDisplay();
        int index = 0;
        foreach(WeaponAttachmentReference reference in compatibleAttachments)
        {
            if (index < attachmentDisplayIcons.Count)
            {
                attachmentDisplayIcons[index].UpdateDisplay(reference);
                attachmentDisplayIcons[index].button.onClick.AddListener(delegate { SetAttachmentToPoint(reference); });
                index += 1;
            }
        }
    }

    /// <summary>
    /// Returns a list of compatible attachments at the right point, in the right category of weapons
    /// </summary>
    /// <param name="attachmentPoint"></param>
    /// <param name="weaponCategory"></param>
    /// <returns></returns>
    private List<WeaponAttachmentReference> GetCompatibleAttachments(AttachmentPoint attachmentPoint, WeaponCategories weaponCategory)
    {
        Dictionary<string, WeaponAttachmentReference> attachmentLibrary = DataManager.instance.GetAttachmentDictionary();
        List<WeaponAttachmentReference> compatibleReferences = new List<WeaponAttachmentReference>();
        foreach (WeaponAttachmentReference reference in attachmentLibrary.Values)
        {
            bool point = (attachmentPoint == reference.attachmentPoint);
            bool category = false;
            for (int i = 0; i < reference.compatibleWeaponCategories.Length; i++)
            {
                if (weaponCategory == reference.compatibleWeaponCategories[i])
                {
                    category = true;
                    break;
                }
            }

            if (point && category)
            {
                compatibleReferences.Add(reference);
            }
        }
        return compatibleReferences;
    }

    public void SetAttachmentToPoint(WeaponAttachmentReference reference)
    {
        foreach (AttachmentDisplayIcon displayIcon in attachmentPointIcons)
        {
            if (displayIcon.attachmentPoint == reference.attachmentPoint)
            {
                displayIcon.UpdateDisplay(reference);
            }
        }
    }

    public List<WeaponAttachmentReference> ExportCurrentSelectedAttachments()
    {
        List<WeaponAttachmentReference> references = new List<WeaponAttachmentReference>();

        foreach (AttachmentDisplayIcon displayIcon in attachmentPointIcons)
        {
            if (displayIcon.reference != null)
            {
                references.Add(displayIcon.reference);
            }
        }
        return references;
    }
}
