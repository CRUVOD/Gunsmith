using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour
{
    // The UIManager will contain useful references and functions to various UI elements in the game
    // Its also gonna be a singleton

    public static UIManager instance;

    [Header("Screens")]
    [Tooltip("the pause screen game object")]
    public GameObject PauseScreen;
    /// the death screen
    [Tooltip("the death screen")]
    public GameObject DeathScreen;
    [Tooltip("the victory screen")]
    public GameObject VictoryScreen;

    [Header("Crosshair")]
    //The cursor/crosshair
    public Crosshair crosshair;

    [Header("UIGroups")]
    public GameObject WeaponUI;
    public GameObject HealthUI;

    [Header("General Weapon UI")]
    public TextMeshProUGUI weaponTypeText;
    public Image weaponIcon;
    public TextMeshProUGUI weaponNameText;

    [Header("Ballistic Weapon UI")]
    public GameObject ballisticWeaponUIGroup;
    public TextMeshProUGUI weaponAmmo;

    //this is used to display magazine size i.e. '/30' at the end of the ammo display
    string ammoTextSuffix;

    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Sets the pause screen on or off.
    /// </summary>
    /// <param name="state">If set to <c>true</c>, sets the pause.</param>
    public virtual void SetPauseScreen(bool state)
    {
        if (PauseScreen != null)
        {
            PauseScreen.SetActive(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    /// <summary>
    /// Sets the death screen on or off.
    /// </summary>
    /// <param name="state">If set to <c>true</c>, sets the pause.</param>
    public virtual void SetDeathScreen(bool state)
    {
        if (DeathScreen != null)
        {
            DeathScreen.SetActive(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    /// <summary>
    /// Sets the victory screen on or off.
    /// </summary>
    /// <param name="state">If set to <c>true</c>, sets the pause.</param>
    public virtual void SetVictoryScreen(bool state)
    {
        if (VictoryScreen != null)
        {
            VictoryScreen.SetActive(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }


    /// <summary>
    /// Changes the UI to correspond to different weapon types
    /// Updating the display fields in the process
    /// </summary>
    /// <param name="weapon"></param>
    public void ChangeWeaponUI(WeaponReference weapon)
    {
        //set the icon
        weaponIcon.sprite = weapon.icon;

        //set the text fields
        weaponTypeText.text = weapon.weaponType.ToString();
        weaponNameText.text = weapon.ID;

        switch (weapon.ammoType)
        {
            case AmmoTypes.Ballistic:
                ballisticWeaponUIGroup.SetActive(true);

                if (weapon.isMagazineBased)
                {
                    ammoTextSuffix = "/" + weapon.magazineSize;
                }
                else
                {
                    //infinity character
                    weaponAmmo.text = "\u221E";
                }
                break;
            case AmmoTypes.None:
                //infinity character
                weaponAmmo.text = "\u221E";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Updates the ammo counter for Ballistic ammo weapons
    /// </summary>
    /// <param name="newAmmo"></param>
    public void UpdateBallisticAmmoUI(int newAmmo)
    {
        weaponAmmo.text = newAmmo + ammoTextSuffix;
    }
}
