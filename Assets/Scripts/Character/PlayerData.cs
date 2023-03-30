using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int initialHealth;
    public string[] weaponsInLoadout;
    public string[][] attachmentsInLoadout;

    public PlayerData(Player player)
    {
        initialHealth = player.InitialHealth;
        weaponsInLoadout = GetWeapons(player.weaponsInLoadout);
        attachmentsInLoadout = GetWeaponAttachments(player.weaponsInLoadout);
    }


    public PlayerData(List<Weapon> weapons)
    {
        initialHealth = 100;
        weaponsInLoadout = GetWeapons(weapons);
        attachmentsInLoadout = GetWeaponAttachments(weapons); 
    }

    public PlayerData(List<WeaponReference> weapons, List<WeaponAttachmentReference>[] weaponAttachments)
    {
        initialHealth = 100;
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
    }

    /// <summary>
    /// Get the IDs of the weapons of the player loadout
    /// Stored by [loadoutweaponposition]
    /// </summary>
    /// <param name="weapons"></param>
    /// <returns></returns>
    private string[] GetWeapons(List<Weapon> weapons)
    {
        List<string> weaponIDs = new List<string>();

        for (int i = 0; i < weapons.Count; i++)
        {
            weaponIDs.Add(weapons[i].reference.ID);
        }

        return weaponIDs.ToArray();
    }

    /// <summary>
    /// Gets the IDs of the attachments of the player loadout
    /// Stored by [loadout weapon position][IDs]
    /// </summary>
    /// <param name="weapons"></param>
    /// <returns></returns>
    private string[][] GetWeaponAttachments(List<Weapon> weapons)
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

    public String toString()
    {
        string ret = "Player Initial Health: " + initialHealth;
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
