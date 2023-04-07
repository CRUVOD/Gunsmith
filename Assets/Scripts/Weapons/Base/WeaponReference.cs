using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Add weapon types and ammo types here, used for defining weapon behaviour and UI to display the right
/// type of ammo counter etc.
/// </summary>

//Type of weapon, defines general traits of the weapon, more UI and cosmetic
public enum WeaponCategories { AR, DMR, Sniper, Pistol, Shotgun, Melee , Grenade}
//How the weapon is fired, single-shot, hold down the mousebutton for automatic fire, used for detecting player input
public enum FiringMechanism { Single, Auto}
//What type of ammo the weapon uses, ballistic is magazine fed, energy builds heat, none is neither
public enum AmmoTypes { Ballistic, Energy , None}

/// <summary>
/// ScriptableObject to establish links and references between weapon prefab, name, icon, ID etc together
/// This could be used to decide what type of ammo counter UI should be displayed
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "New Weapon Reference")]
public class WeaponReference : ScriptableObject
{
    [Header("Basic Properties")]
    [Tooltip("unique name for object")]
    public string ID;

    [Header("Images")]
    //square icon to display
    public Sprite icon;

    [Header("GameObjects")]
    //the weapon to give to the player
    public Weapon weaponObject;

    [Header("Weapon Info")]
    //the type of weapon this is
    public WeaponCategories weaponCategory;
    //the firing mechanism of this weapon
    public FiringMechanism firingMechanism;
    //the default ammo type this weapon uses
    public AmmoTypes ammoType;
    //short description of item
    public string shortDescription;
    //full description of item
    public string fullDescription;

    [Header("Ballistic Info")]
    //if weapon needs to reload
    public bool isMagazineBased;
    //magazine size
    public int magazineSize;

    [Header("Energy Info")]
    public float heatMax;

    [Header("Attachment Info")]
    public AttachmentPoint[] attachmentPoints;
}
