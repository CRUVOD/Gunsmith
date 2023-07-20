using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script that gives the player a weapon on enter
/// </summary>
public class PlayerEnterWeaponGiver : MonoBehaviour
{
    private bool hasBeenTriggered;
    public Weapon weaponToGive;
    private Player player;

    private void Awake()
    {
        hasBeenTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && !hasBeenTriggered)
        {
            player = collision.gameObject.GetComponent<Player>();
            hasBeenTriggered = true;
            player.AddWeaponToLoadout(weaponToGive);

            //Unlocks the weapon in data as well
            PlayerData playerData = new PlayerData();
            playerData.AddUnlockedWeapon(weaponToGive.reference.ID);
            SaveSystem.SavePlayer(playerData);
        }
    }
}
