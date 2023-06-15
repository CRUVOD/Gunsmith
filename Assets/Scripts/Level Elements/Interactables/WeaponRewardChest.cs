using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponRewardChest : InteractableChest
{
    [Tooltip("The rarity of the weapon given")]
    public Rarity weaponRarity;

    protected override void GiveChestContent(Player player)
    {
        //First get the weapon to unlock
        WeaponReference reference = GetWeaponToUnlock();

        if (reference == null)
        {
            //We cannot unlock any weapons through this chest, we give currency instead
            DebugText.instance.SetText("Implement Currency");
            return;
        }

        DebugText.instance.SetText(reference.ID);
        //Set the content of the display to the reference
        SetContent(reference.icon, reference.ID);

        //Save the new unlocked weapons
        if (SaveSystem.SaveFileExists())
        {
            PlayerData playerData = SaveSystem.LoadPlayer();
            if (playerData.weaponsUnlocked != null)
            {
                List<string> weaponsUnlocked = new List<string>();
                weaponsUnlocked.AddRange(playerData.weaponsUnlocked);
                weaponsUnlocked.Add(reference.ID);
                playerData.weaponsUnlocked = weaponsUnlocked.ToArray();
            }
            else
            {
                playerData.weaponsUnlocked = new string[] { reference.ID };
            }
            SaveSystem.SavePlayer(playerData);
        }
        else
        {
            PlayerData playerData = new PlayerData(new string[] { reference.ID }, new string[] { });
            SaveSystem.SavePlayer(playerData);
        }

    }

    /// <summary>
    /// Returns a weapon reference that the player unlocks
    /// </summary>
    /// <returns></returns>
    private WeaponReference GetWeaponToUnlock()
    {
        List<string> weaponsInGame = new List<string>();
        weaponsInGame.AddRange((DataManager.instance.GetWeaponDictionary().Keys.ToArray()));

        if (SaveSystem.SaveFileExists())
        {
            string[] currentlyUnlockedWeapons = SaveSystem.LoadPlayer().weaponsUnlocked;
            if (currentlyUnlockedWeapons != null && currentlyUnlockedWeapons.Length > 0)
            {
                for (int i = 0; i < currentlyUnlockedWeapons.Length; i++)
                {
                    //Remove any weapons that has already been unlocked
                    weaponsInGame.Remove(currentlyUnlockedWeapons[i]);
                }
            }
        }

        List<WeaponReference> weaponsCanBeUnlocked = new List<WeaponReference>();

        for (int i = 0; i < weaponsInGame.Count; i++)
        {
            WeaponReference reference = DataManager.instance.TryGetWeaponReference(weaponsInGame[i]);
            if (reference.rarity.Equals(weaponRarity))
            {
                //If the rarity matches, the weapon can be given
                weaponsCanBeUnlocked.Add(reference);
            }
        }

        if (weaponsCanBeUnlocked.Count <= 0)
        {
            //Player cannot unlock any other weapon in this rarity
            return null;
        }
        else
        {
            //Randomly select a random weapon to unlock
            weaponsCanBeUnlocked.Shuffle();
            return weaponsCanBeUnlocked[0];
        }
    }
}
