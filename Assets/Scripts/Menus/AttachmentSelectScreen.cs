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

        //First clear all attachment displays
        foreach (AttachmentDisplayIcon displayIcon in attachmentPointIcons)
        {
            displayIcon.RevertToEmpty();
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
            if (index < references.Length && references[index].attachmentPoint.Equals(attachmentPoint))
            {
                //If the attachment in the library is for the point given
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
