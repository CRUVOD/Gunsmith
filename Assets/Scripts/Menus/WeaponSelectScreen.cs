using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the logic of the loadoutcustomisation screen
/// </summary>
public class WeaponSelectScreen : MonoBehaviour
{
    //number of weapons the player can equip
    public int numWeaponSlots;

    string[] weaponReferenceIDs;
    public WeaponLoadoutIcon[] loadoutSlotIcons;

    public WeaponDisplayIcon weaponDisplayIconPrefab;
    public GridLayoutGroup weaponDisplay;
    [HideInInspector]
    public List<WeaponDisplayIcon> weaponDisplayIcons;


    private void Start()
    {
        weaponReferenceIDs = new string[DataManager.instance.GetWeaponDictionary().Count];
        DataManager.instance.GetWeaponDictionary().Keys.CopyTo(weaponReferenceIDs, 0);
        GenerateEmptyDisplayIcons();

        //We run the start function then set itself to inactive
        this.gameObject.SetActive(false);
    }

    //Generates empty display icons for use
    private void GenerateEmptyDisplayIcons()
    {
        weaponDisplayIcons = new List<WeaponDisplayIcon>();
        for (int i = 0; i < DataManager.instance.GetWeaponDictionary().Count; i++)
        {
            WeaponDisplayIcon icon = Instantiate(weaponDisplayIconPrefab, weaponDisplay.gameObject.transform);
            weaponDisplayIcons.Add(icon);
            icon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Call this function on entering/enabling weapon select screen
    /// This will fill the screen with correct information based on player data
    /// </summary>
    public void UpdateDisplayInformation()
    {
        PlayerData playerData = SaveSystem.LoadPlayer();
        if (playerData != null)
        {
            weaponReferenceIDs = playerData.weaponsUnlocked;
            UpdateLoadoutDisplay(playerData);
            UpdateWeaponDisplay(playerData);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// Updates the weapon loadout screen with saved player loadout (left side)
    /// </summary>
    /// <param name="playerData"></param>
    private void UpdateLoadoutDisplay(PlayerData playerData)
    {
        if (playerData.weaponsInLoadout != null)
        {
            for (int i = 0; i < playerData.weaponsInLoadout.Length; i++)
            {
                SetLoadoutWeapon(i, DataManager.instance.TryGetWeaponReference(playerData.weaponsInLoadout[i]));
            }
        }
        else
        {
            for (int i = 0; i < playerData.weaponsUnlocked.Length; i++)
            {
                SetLoadoutWeapon(i, DataManager.instance.TryGetWeaponReference(playerData.weaponsUnlocked[i]));
            }
        }
    }

    /// <summary>
    /// Updates the weapon selection with player unlocked weapons
    /// </summary>
    /// <param name="playerData"></param>
    private void UpdateWeaponDisplay(PlayerData playerData)
    {
        ClearWeaponDisplay();
        if (playerData.weaponsUnlocked != null)
        {
            for (int i = 0; i < playerData.weaponsUnlocked.Length; i++)
            {
                weaponDisplayIcons[i].UpdateDisplay(playerData.weaponsUnlocked[i]);
            }
        }
    }

    /// <summary>
    /// Removes all currently displayed weapon for all icons and set the game object active
    /// </summary>
    private void ClearWeaponDisplay()
    {
        for (int i = 0; i < weaponDisplayIcons.Count; i++)
        {
            weaponDisplayIcons[i].gameObject.SetActive(true);
            weaponDisplayIcons[i].SetEmptySprite();
        }
    }

    /// <summary>
    /// Set display of weapon in loadout slot
    /// </summary>
    /// <param name="slotNum"></param>
    /// <param name="reference"></param>
    public void SetLoadoutWeapon(int slotNum, WeaponReference reference)
    {
        //Debug.Log("Setting slot " + slotNum + " with" + reference.ID);
        if (slotNum >= loadoutSlotIcons.Length)
        {
            //Too many weapons in loadout/we don't have a loadout so we are just putting in the first two unlocked weapons
            return;
        }

        if (!WeaponExistsInLoadout(reference))
        {
            loadoutSlotIcons[slotNum].UpdateDisplay(reference);
        }
    }

    #region ButtonInteractions

    public void ConfirmAndSaveLoadout()
    {
        List<Weapon> currentSelectedWeapons = GetCurrentSelectedWeapons();
        PlayerData newPlayerData = new PlayerData(currentSelectedWeapons);
        SaveSystem.SavePlayer(newPlayerData);
        TryUpdatePlayerLoadout();
    }

    #endregion

    /// <summary>
    /// Returns true if the reference is already selected in the loadout
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    public bool WeaponExistsInLoadout(WeaponReference reference)
    {
        for (int i = 0; i < loadoutSlotIcons.Length; i++)
        {
            if (loadoutSlotIcons[i].reference != null && loadoutSlotIcons[i].reference.ID.Equals(reference.ID))
            {
                //Duplicate weapon in another loadout slot, return early
                return true;
            }
        }

        return false;
    }

    private List<Weapon> GetCurrentSelectedWeapons()
    {
        List<Weapon> currentSelectedWeapons = new List<Weapon>();
        for (int i = 0; i < numWeaponSlots; i++)
        {
            Weapon weapon = (loadoutSlotIcons[i].reference.weaponObject);
            currentSelectedWeapons.Add(weapon);
        }

        return currentSelectedWeapons;
    }

    /// <summary>
    /// Returns the weapon reference for the selected weapon at slotNum
    /// </summary>
    /// <param name="slotNum"></param>
    /// <returns></returns>
    public WeaponReference GetSelectedWeapon(int slotNum)
    {
        WeaponReference reference = loadoutSlotIcons[slotNum].reference;
        return reference;
    }

    public List<WeaponReference> ExportSelectedWeapons()
    {
        List<WeaponReference> references = new List<WeaponReference>();

        for (int i = 0; i < numWeaponSlots; i++)
        {
            WeaponReference reference = loadoutSlotIcons[i].reference;
            references.Add(reference);
        }

        return references;
    }

    /// <summary>
    /// Searches the scene for the player, and tries to update the player with the currently selected weapon loadout
    /// </summary>
    private void TryUpdatePlayerLoadout()
    {
        GameObject[] searchResults = GameObject.FindGameObjectsWithTag("Player");
        if (searchResults.Length == 1)
        {
            Player player;
            if (searchResults[0].TryGetComponent<Player>(out player))
            {
                player.UpdateLoadout(GetCurrentSelectedWeapons());
            }
        }
        else if (searchResults.Length > 1)
        {
            Debug.LogWarning("More than one player?");
        }
        else
        {
            Debug.LogWarning("The player is missing ;-;");
        }
    }
}
