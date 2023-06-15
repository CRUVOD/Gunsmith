using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttachmentRewardChest : InteractableChest
{
    [Tooltip("The rarity of the attachment given")]
    public Rarity attachmentRarity;

    protected override void GiveChestContent(Player player)
    {
        //First get the weapon to unlock
        WeaponAttachmentReference reference = GetAttachmentToUnlock();

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
            if (playerData.attachmentsUnlocked != null)
            {
                List<string> attachmentsUnlocked = new List<string>();
                attachmentsUnlocked.AddRange(playerData.attachmentsUnlocked);
                attachmentsUnlocked.Add(reference.ID);
                playerData.attachmentsUnlocked = attachmentsUnlocked.ToArray();
            }
            else
            {
                playerData.attachmentsUnlocked = new string[] { reference.ID };
            }

            SaveSystem.SavePlayer(playerData);
        }
        else
        {
            PlayerData playerData = new PlayerData(new string[] { }, new string[] { reference.ID });
            SaveSystem.SavePlayer(playerData);
        }
    }

    /// <summary>
    /// Returns a weapon reference that the player unlocks
    /// </summary>
    /// <returns></returns>
    private WeaponAttachmentReference GetAttachmentToUnlock()
    {
        List<string> attachmentsInGame = new List<string>();
        attachmentsInGame.AddRange((DataManager.instance.GetAttachmentDictionary().Keys.ToArray()));

        if (SaveSystem.SaveFileExists())
        {
            string[] currentlyUnlockedAttachments = SaveSystem.LoadPlayer().attachmentsUnlocked;


            if (currentlyUnlockedAttachments != null && currentlyUnlockedAttachments.Length > 0)
            {
                for (int i = 0; i < currentlyUnlockedAttachments.Length; i++)
                {
                    //Remove any weapons that has already been unlocked
                    attachmentsInGame.Remove(currentlyUnlockedAttachments[i]);
                }
            }
        }

        List<WeaponAttachmentReference> attachmentsCanBeUnlocked = new List<WeaponAttachmentReference>();

        for (int i = 0; i < attachmentsInGame.Count; i++)
        {
            WeaponAttachmentReference reference = DataManager.instance.TryGetAttachmentReference(attachmentsInGame[i]);
            if (reference.rarity.Equals(attachmentRarity))
            {
                //If the rarity matches, the weapon can be given
                attachmentsCanBeUnlocked.Add(reference);
            }
        }

        if (attachmentsCanBeUnlocked.Count <= 0)
        {
            //Player cannot unlock any other weapon in this rarity
            return null;
        }
        else
        {
            //Randomly select a random weapon to unlock
            attachmentsCanBeUnlocked.Shuffle();
            return attachmentsCanBeUnlocked[0];
        }
    }
}
