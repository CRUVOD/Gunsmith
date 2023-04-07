using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Works the same way as weapon reference
/// </summary>

//Where the attachment goes on the weapon
public enum AttachmentPoint {Core, Underbarrel, Magazine, Top, Barrel}

/// <summary>
/// ScriptableObject to establish links and references between weapon attachment prefab, name, icon, ID etc together
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "New Weapon Attachment Reference")]
public class WeaponAttachmentReference : ScriptableObject
{
    [Header("Basic Properties")]
    [Tooltip("unique name for object")]
    public string ID;
    public AttachmentPoint attachmentPoint;

    [Header("Images")]
    //square icon to display
    public Sprite icon;

    [Header("GameObjects")]
    //the weapon to give to the weapon
    public WeaponAttachment attachmentObject;

    [Header("Attachment Info")]
    //short description of item
    public string shortDescription;
    public WeaponCategories[] compatibleWeaponCategories;
}
