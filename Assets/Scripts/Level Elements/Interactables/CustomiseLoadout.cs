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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitLoadoutCustomisation();
        }
    }

    public void Interact(Player player)
    {
        if (!inCustomisation)
        {
            EnterLoadoutCustomisation(player);
        }
        else
        {
            ExitLoadoutCustomisation(player);
        }
    }

    private void EnterLoadoutCustomisation(Player player)
    {
        LoadoutCustomiseScreen.gameObject.SetActive(true);
        LoadoutCustomiseScreen.EnterLoadoutCustomisation();
        inCustomisation = true;
        //Freeze player movement
        player.FreezePlayerMovement(true);
        //Toggle UI
        UIManager.instance.MenuMode(true);
    }

    private void ExitLoadoutCustomisation()
    {
        LoadoutCustomiseScreen.gameObject.SetActive(false);
        LoadoutCustomiseScreen.ExitLoadoutCustomisation();
        inCustomisation = false;
        LevelManager.instance.player.FreezePlayerMovement(false);
        UIManager.instance.MenuMode(false);
    }

    private void ExitLoadoutCustomisation(Player player)
    {
        LoadoutCustomiseScreen.gameObject.SetActive(false);
        LoadoutCustomiseScreen.ExitLoadoutCustomisation();
        inCustomisation = false;
        //Unfreeze player movement
        player.FreezePlayerMovement(false);
        UIManager.instance.MenuMode(false);
    }
}
