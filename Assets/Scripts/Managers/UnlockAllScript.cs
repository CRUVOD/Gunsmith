using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// This script simply unlocks all weapons and attachments at start, attach to anything in scene
/// </summary>
public class UnlockAllScript : MonoBehaviour
{
    public static UnlockAllScript instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        PlayerData playerData = SaveSystem.LoadPlayer();
        playerData.weaponsUnlocked = DataManager.instance.GetWeaponDictionary().Keys.ToArray();
        playerData.attachmentsUnlocked = DataManager.instance.GetAttachmentDictionary().Keys.ToArray();
        SaveSystem.SavePlayer(playerData);
    }
}
