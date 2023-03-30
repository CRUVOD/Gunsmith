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

    //The index of the current wepaon displayed in each slot
    private int[] weaponSlotIndices;

    string[] weaponReferenceIDs;
    public Image[] weaponSlotIcons;

    private void Start()
    {
        weaponSlotIndices = new int[numWeaponSlots];
        weaponReferenceIDs = new string[DataManager.instance.GetWeaponDictionary().Count];
        DataManager.instance.GetWeaponDictionary().Keys.CopyTo(weaponReferenceIDs, 0);

        PlayerData playerData = SaveSystem.LoadPlayer();
        if (playerData != null)
        {
            InitialiseWithPlayerData(playerData);
        }
        else
        {
            //Default to 0 on all slots which weapon to display if player data is not loaded
            for (int i = 0; i < weaponSlotIndices.Length; i++)
            {
                weaponSlotIndices[i] = 0;
                SetWeapon(i, DataManager.instance.TryGetWeaponReference(weaponReferenceIDs[0]));
            }
        }
    }

    /// <summary>
    /// Initialises the weapon loadout screen with saved player loadout
    /// </summary>
    /// <param name="playerData"></param>
    private void InitialiseWithPlayerData(PlayerData playerData)
    {
        for (int i = 0; i < playerData.weaponsInLoadout.Length; i++)
        {
            SetWeapon(i, DataManager.instance.TryGetWeaponReference(playerData.weaponsInLoadout[i]));
            for (int j = 0; j < weaponReferenceIDs.Length; j++)
            {
                //Set index of the two weapon slots for previous/next navigation
                if (weaponReferenceIDs[j] == playerData.weaponsInLoadout[i])
                {
                    weaponSlotIndices[i] = j;
                }
            }
        }
    }

    /// <summary>
    /// Set display of weapon in loadout slot, this does not change the indices
    /// </summary>
    /// <param name="slotNum"></param>
    /// <param name="reference"></param>
    public void SetWeapon(int slotNum, WeaponReference reference)
    {
        //Debug.Log("Setting slot " + slotNum + " with" + reference.ID);

        weaponSlotIcons[slotNum].sprite = reference.icon;
    }

    #region ButtonInteractions

    public void PreviousWeapon(int slotNum)
    {
        //First check if there exist a previous weapon
        if (weaponSlotIndices[slotNum] > 0)
        {
            //Roll back one index on the weapon references
            weaponSlotIndices[slotNum] -= 1;
            SetWeapon(slotNum, DataManager.instance.TryGetWeaponReference(weaponReferenceIDs[weaponSlotIndices[slotNum]]));
        }
    }

    public void NextWeapon(int slotNum)
    {
        //First check if there exist a next weapon
        if (weaponSlotIndices[slotNum] < weaponReferenceIDs.Length - 1)
        {
            //Advance one index on the weapon references
            weaponSlotIndices[slotNum] += 1;
            SetWeapon(slotNum, DataManager.instance.TryGetWeaponReference(weaponReferenceIDs[weaponSlotIndices[slotNum]]));
        }
    }

    public void ConfirmAndSaveLoadout()
    {
        List<Weapon> currentSelectedWeapons = GetCurrentSelectedWeapons();
        PlayerData newPlayerData = new PlayerData(currentSelectedWeapons);
        SaveSystem.SavePlayer(newPlayerData);
        TryUpdatePlayerLoadout();
    }

    #endregion

    private List<Weapon> GetCurrentSelectedWeapons()
    {
        List<Weapon> currentSelectedWeapons = new List<Weapon>();
        for (int i = 0; i < numWeaponSlots; i++)
        {
            Weapon weapon = DataManager.instance.TryGetWeaponReference(weaponReferenceIDs[weaponSlotIndices[i]]).weaponObject;
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
        WeaponReference reference = DataManager.instance.TryGetWeaponReference(weaponReferenceIDs[weaponSlotIndices[slotNum]]);
        return reference;
    }

    public List<WeaponReference> ExportSelectedWeapons()
    {
        List<WeaponReference> references = new List<WeaponReference>();

        for (int i = 0; i < numWeaponSlots; i++)
        {
            WeaponReference reference = DataManager.instance.TryGetWeaponReference(weaponReferenceIDs[weaponSlotIndices[i]]);
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
