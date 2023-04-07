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

        Dictionary<string, WeaponAttachmentReference> attachmentLibrary = DataManager.instance.GetAttachmentDictionary();
        int index = 0;
        WeaponAttachmentReference[] references = new WeaponAttachmentReference[attachmentLibrary.Count];
        attachmentLibrary.Values.CopyTo(references, 0);

        foreach (AttachmentDisplayIcon displayIcon in attachmentDisplayIcons)
        {
            if (index < references.Length && references[index].attachmentPoint.Equals(attachmentPoint) && AttachmentIsCompatible(references[index]))
            {
                //If the attachment in the library is for the point given, and is for the right weapon category
                displayIcon.UpdateDisplay(references[index]);
                displayIcon.button.onClick.AddListener(delegate { SetAttachmentToPoint(displayIcon.reference); });
                //Debug.Log(displayIcon.button.onClick.GetPersistentEventCount());
                index += 1;
            }
            else
            {
                displayIcon.RevertToEmptyComplete();
            }
        }
    }

    private bool AttachmentIsCompatible(WeaponAttachmentReference reference)
    {
        foreach (WeaponCategories weaponCategory in reference.compatibleWeaponCategories)
        {
            if (weaponCategory == currentWeapon.weaponCategory)
            {
                return true;
            }
        }
        return false;
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
