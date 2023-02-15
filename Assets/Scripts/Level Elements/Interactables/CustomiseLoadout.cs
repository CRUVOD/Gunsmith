using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Click 'interact' to open up loadout customisation
/// </summary>
public class CustomiseLoadout : MonoBehaviour, Interactable
{
    public LoadoutCustomisationScreen LoadoutCustomiseScreen;

    [HideInInspector]
    //If the player is already customising their loadout or not
    public bool inCustomisation;

    public void Interact(Player player)
    {
        if (!inCustomisation)
        {
            LoadoutCustomiseScreen.gameObject.SetActive(true);
            inCustomisation = true;
            //Freeze player movement
            player.FreezePlayerMovement(true);
            //Set cursor to be visible
            UIManager.instance.crosshair.ToggleCrosshair(false);
        }
        else
        {
            LoadoutCustomiseScreen.gameObject.SetActive(false);
            inCustomisation = false;
            //Unfreeze player movement
            player.FreezePlayerMovement(false);
            UIManager.instance.crosshair.ToggleCrosshair(true);
        }
    }
}
