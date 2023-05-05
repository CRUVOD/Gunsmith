using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutCustomisationScreen : MonoBehaviour
{
    public WeaponSelectScreen WeaponSelectScreen;
    public AttachmentSelectScreen AttachmentSelectScreen;

    public int numberOfWeapons;
    private List<WeaponReference> weaponsInSelection;
    private List<WeaponAttachmentReference>[] attachmentsInSeleciton;

    //Keeps track of which slot number is being customised
    private int attachmentSelectSlotNum;

    private void Awake()
    {
        weaponsInSelection = new List<WeaponReference>();
        attachmentsInSeleciton = new List<WeaponAttachmentReference>[numberOfWeapons];
    }

    /// <summary>
    /// Player first opening loadout customisation, enable weapon select and disable attachmentselect screen
    /// </summary>
    public void EnterLoadoutCustomisation()
    {
        WeaponSelectScreen.gameObject.SetActive(true);
        AttachmentSelectScreen.gameObject.SetActive(false);
    }

    /// <summary>
    /// Close both screens
    /// </summary>
    public void ExitLoadoutCustomisation()
    {
        WeaponSelectScreen.gameObject.SetActive(false);
        AttachmentSelectScreen.gameObject.SetActive(false);
    }

    public void EnterAttachmentSelect(int slotNum)
    {
        attachmentSelectSlotNum = slotNum;
        WeaponReference reference = WeaponSelectScreen.GetSelectedWeapon(slotNum);
        WeaponSelectScreen.gameObject.SetActive(false);
        AttachmentSelectScreen.gameObject.SetActive(true);
        AttachmentSelectScreen.UpdateCustomisationWeapon(reference, attachmentsInSeleciton[attachmentSelectSlotNum]);
    }

    /// <summary>
    /// Exits the attachment select screen, and saves the selection of attachments
    /// </summary>
    public void ExitAttachmentSelect()
    {
        attachmentsInSeleciton[attachmentSelectSlotNum] = AttachmentSelectScreen.ExportCurrentSelectedAttachments();
        WeaponSelectScreen.gameObject.SetActive(true);
        AttachmentSelectScreen.gameObject.SetActive(false);
    }

    public void ConfirmAndSaveLoadout()
    {
        List<WeaponReference> currentSelectedWeapons = WeaponSelectScreen.ExportSelectedWeapons();
        weaponsInSelection = currentSelectedWeapons;
        PlayerData newPlayerData = new PlayerData(weaponsInSelection, attachmentsInSeleciton);
        SaveSystem.SavePlayer(newPlayerData);
        Debug.Log(newPlayerData.ToString());
        TryUpdatePlayerLoadout(newPlayerData);
    }

    private void TryUpdatePlayerLoadout(PlayerData playerData)
    {
        GameObject[] searchResults = GameObject.FindGameObjectsWithTag("Player");
        if (searchResults.Length == 1)
        {
            Player player;
            if (searchResults[0].TryGetComponent<Player>(out player))
            {
                player.UpdateLoadout(playerData);
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
