using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int initialHealth;
    public string[] weaponsInLoadout;
    public string[][] attachmentsInLoadout;
    public string[] weaponsUnlocked;
    public string[] attachmentsUnlocked;

    /// <summary>
    /// The default constructor just attempts to load the old player data
    /// </summary>
    public PlayerData()
    {
        if (SaveSystem.SaveFileExists())
        {
            PlayerData oldData = SaveSystem.LoadPlayer();
            playerName = oldData.playerName;
            initialHealth = oldData.initialHealth;
            weaponsInLoadout = oldData.weaponsInLoadout;
            attachmentsInLoadout = oldData.attachmentsInLoadout;
            weaponsUnlocked = oldData.weaponsUnlocked;
            attachmentsUnlocked = oldData.attachmentsUnlocked;
        }
        else
        {
            playerName = GetPlayerName();
            initialHealth = GetPlayerInitialHealth();
            weaponsInLoadout = GetLoadoutWeapons();
            attachmentsInLoadout = GetLoadoutWeaponAttachments();
            weaponsUnlocked = GetUnlockedWeapons();
            attachmentsUnlocked = GetUnlockedAttachments();
        }

        UnlockSanityCheck();
    }

    public PlayerData(Player player)
    {
        playerName = GetPlayerName();
        initialHealth = (int) player.InitialHealth;
        weaponsInLoadout = GetLoadoutWeapons(player.weaponsInLoadout);
        attachmentsInLoadout = GetLoadoutWeaponAttachments(player.weaponsInLoadout);
        weaponsUnlocked = GetUnlockedWeapons();
        attachmentsUnlocked = GetUnlockedAttachments();

        UnlockSanityCheck();
    }


    public PlayerData(List<Weapon> weapons)
    {
        playerName = GetPlayerName();
        initialHealth = GetPlayerInitialHealth();
        weaponsInLoadout = GetLoadoutWeapons(weapons);
        attachmentsInLoadout = GetLoadoutWeaponAttachments(weapons);
        weaponsUnlocked = GetUnlockedWeapons();
        attachmentsUnlocked = GetUnlockedAttachments();

        UnlockSanityCheck();
    }

    public PlayerData(List<WeaponReference> weapons, List<WeaponAttachmentReference>[] weaponAttachments)
    {
        playerName = GetPlayerName();
        initialHealth = GetPlayerInitialHealth();
        List<string> weaponIDs = new List<string>();
        for (int i = 0; i < weapons.Count; i++)
        {
            weaponIDs.Add(weapons[i].ID);
        }
        weaponsInLoadout = weaponIDs.ToArray();

        List<string[]> weaponAttachmentIDs = new List<string[]>();

        for (int i = 0; i < weaponAttachments.Length; i++)
        {
            List<string> weaponAttachmentsTemp = new List<string>();

            if (weaponAttachments[i] != null)
            {
                for (int j = 0; j < weaponAttachments[i].Count; j++)
                {
                    weaponAttachmentsTemp.Add(weaponAttachments[i][j].ID);
                }
            }
            weaponAttachmentIDs.Add(weaponAttachmentsTemp.ToArray());
        }

        attachmentsInLoadout =  weaponAttachmentIDs.ToArray();
        weaponsUnlocked = GetUnlockedWeapons();
        attachmentsUnlocked = GetUnlockedAttachments();

        UnlockSanityCheck();
    }

    public PlayerData(string playerName)
    {
        this.playerName = playerName;
        initialHealth = GetPlayerInitialHealth();
        weaponsInLoadout = GetLoadoutWeapons();
        attachmentsInLoadout = GetLoadoutWeaponAttachments();
        weaponsUnlocked = GetUnlockedWeapons();
        attachmentsUnlocked = GetUnlockedAttachments();

        UnlockSanityCheck();
    }

    public PlayerData(string[] unlockedWeapons, string[] unlockedAttackments)
    {
        this.playerName = GetPlayerName();
        initialHealth = GetPlayerInitialHealth();
        weaponsInLoadout = GetLoadoutWeapons();
        attachmentsInLoadout = GetLoadoutWeaponAttachments();
        weaponsUnlocked = unlockedWeapons;
        attachmentsUnlocked = unlockedAttackments;

        UnlockSanityCheck();
    }

    /// <summary>
    /// Gets the player name from previously saved data if possible
    /// </summary>
    /// <returns></returns>
    private string GetPlayerName()
    {
        if (SaveSystem.SaveFileExists())
        {
            //Previous save file exists, use same name
            PlayerData prevSave = SaveSystem.LoadPlayer();
            return prevSave.playerName;
        }
        else
        {
            //Returns placeholder name if previous save file does not exist
            return "DefaultPlayer";
        }
    }

    /// <summary>
    /// Gets player initial health from previously saved data if possible
    /// </summary>
    /// <returns></returns>
    public int GetPlayerInitialHealth()
    {
        if (SaveSystem.SaveFileExists())
        {
            //Previous save file exists, use same initial health
            PlayerData prevSave = SaveSystem.LoadPlayer();
            return prevSave.initialHealth;
        }
        else
        {
            //Returns placeholder 100 health if previous save file does not exist
            return 100;
        }
    }

    /// <summary>
    /// Get the IDs of the weapons of the player loadout from a list of weapons
    /// Stored by [loadoutweaponposition]
    /// </summary>
    /// <param name="weapons"></param>
    /// <returns></returns>
    private string[] GetLoadoutWeapons(List<Weapon> weapons)
    {
        List<string> weaponIDs = new List<string>();

        for (int i = 0; i < weapons.Count; i++)
        {
            weaponIDs.Add(weapons[i].reference.ID);
        }

        return weaponIDs.ToArray();
    }

    /// <summary>
    /// Gets weapons in loadout from previously saved data if possible
    /// </summary>
    /// <returns></returns>
    private string[] GetLoadoutWeapons()
    {
        if (SaveSystem.SaveFileExists())
        {
            //Previous save file exists, use same loadout
            PlayerData prevSave = SaveSystem.LoadPlayer();
            return prevSave.weaponsInLoadout;
        }
        else
        {
            //Returns placeholder
            return null;
        }
    }

    /// <summary>
    /// Gets the IDs of the attachments of the player loadout
    /// Stored by [loadout weapon position][IDs]
    /// </summary>
    /// <param name="weapons"></param>
    /// <returns></returns>
    private string[][] GetLoadoutWeaponAttachments(List<Weapon> weapons)
    {
        List<string[]> weaponAttachmentIDs = new List<string[]>();

        for (int i = 0; i < weapons.Count; i++)
        {
            List<string> weaponAttachments = new List<string>();
            for (int j = 0; j < weapons[i].weaponAttachmentSlots.Count; j++)
            {
                weaponAttachments.Add(weapons[i].weaponAttachmentSlots[j].GetWeaponAttachment().reference.ID);
            }
            weaponAttachmentIDs.Add(weaponAttachments.ToArray());
        }

        return weaponAttachmentIDs.ToArray();
    }

    /// <summary>
    /// Gets weapon attachments in loadout from previously saved data if possible
    /// </summary>
    /// <returns></returns>
    private string[][] GetLoadoutWeaponAttachments()
    {
        if (SaveSystem.SaveFileExists())
        {
            //Previous save file exists, use same loadout
            PlayerData prevSave = SaveSystem.LoadPlayer();
            return prevSave.attachmentsInLoadout;
        }
        else
        {
            //Returns placeholder
            return null;
        }
    }

    /// <summary>
    /// Gets the list of weapons that has been unlocked by the player in playing
    /// </summary>
    /// <returns></returns>
    private string[] GetUnlockedWeapons()
    {
        if (SaveSystem.SaveFileExists())
        {
            //Previous save file exists, take existing unlocked weapons
            PlayerData prevSave = SaveSystem.LoadPlayer();
            return prevSave.weaponsUnlocked;
        }
        else
        {           
            //Returns placeholder
            return new string[] { };
        }
    }

    private string[] GetUnlockedAttachments()
    {
        if (SaveSystem.SaveFileExists())
        {
            //Previous save file exists, use same loadout
            PlayerData prevSave = SaveSystem.LoadPlayer();
            return prevSave.attachmentsUnlocked;
        }
        else
        {
            //Returns placeholder
            return new string[] { };
        }
    }

    /// <summary>
    /// Adds a weapon to the unlocked weapons string array
    /// </summary>
    /// <param name="weaponID"></param>
    public void AddUnlockedWeapon(string weaponID)
    {
        List<string> weaponsUnlockedList = new List<string>();
        weaponsUnlockedList.AddRange(weaponsUnlocked);
        if (!weaponsUnlockedList.Contains(weaponID))
        {
            weaponsUnlockedList.Add(weaponID);
        }
        weaponsUnlocked = weaponsUnlockedList.ToArray();
    }

    /// <summary>
    /// Adds a weapon to the unlocked weapons string array
    /// </summary>
    /// <param name="weaponID"></param>
    public void AddUnlockedAttachment(string attachmentID)
    {
        List<string> attachmentUnlockedList = new List<string>();
        attachmentUnlockedList.AddRange(attachmentsUnlocked);
        if (!attachmentUnlockedList.Contains(attachmentID))
        {
            attachmentUnlockedList.Add(attachmentID);
        }
        attachmentsUnlocked = attachmentUnlockedList.ToArray();
    }

    /// <summary>
    /// Checks through the wepaon loadouts to see if theres is a weapon/attachment not unlocked
    /// Then unlocks them in the data
    /// </summary>
    public void UnlockSanityCheck()
    {
        if (weaponsInLoadout!= null && weaponsInLoadout.Length > 0)
        {
            for (int i = 0; i < weaponsInLoadout.Length; i++)
            {
                if (Array.IndexOf(weaponsUnlocked, weaponsInLoadout[i]) < 0)
                {
                    //This weapon in the loadout is not unlocked yet, we unlock it
                    AddUnlockedWeapon(weaponsInLoadout[0]);
                }
            }
        }

        if (attachmentsInLoadout != null && attachmentsInLoadout.Length > 0)
        {
            for (int i = 0; i < attachmentsInLoadout.Length; i++)
            {
                if (attachmentsInLoadout[i].Length > 0)
                {
                    for (int j = 0; j < attachmentsInLoadout[i].Length; j++)
                    {
                        if (Array.IndexOf(attachmentsUnlocked, attachmentsInLoadout[i][j]) < 0)
                        {
                            //This attachment in the loadout is not unlocked yet, we unlock it
                            AddUnlockedAttachment(attachmentsInLoadout[i][j]);
                        }
                    }
                }
            }
        }
    }

    public override String ToString()
    {
        string ret = "Player: " + playerName;
        ret += "\n";
        ret += "Player Initial Health: " + initialHealth;
        ret += "\n";
        for (int i = 0; i < weaponsInLoadout.Length; i++)
        {
            ret += "Weapon " + i + ": " + weaponsInLoadout[i] + "- ";
            if (attachmentsInLoadout.Length != 0)
            {
                for (int j = 0; j < attachmentsInLoadout[i].Length; j++)
                {
                    ret += attachmentsInLoadout[i][j] + ", ";
                }
            }

            ret += "\n";
        }

        return ret;
    }
}
