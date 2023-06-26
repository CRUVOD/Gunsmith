using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Click 'interact' to open the mission board, select level and load into mission
/// </summary>
public class MissionBoard : MonoBehaviour, Interactable
{
    //The display for mission board UI
    public Canvas canvas;

    //Level icons kind of represent all levels available in game for the player
    public LevelIcon[] levelIcons;
    public LevelInfoDisplayer levelInfoDisplayer;

    //bool to keep track if player is in this menu or not
    [HideInInspector]
    public bool inMissionSelect;

    private void Start()
    {
        //Assign the displayer to each level icon
        for (int i = 0; i < levelIcons.Length; i++)
        {
            levelIcons[i].levelInfoDisplayer = levelInfoDisplayer;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitMissionSelect();
        }
    }

    public void Interact(Player player)
    {
        if (!inMissionSelect)
        {
            EnterMissionSelect(player);
        }
        else
        {
            ExitMissionSelect(player);
        }
    }

    private void EnterMissionSelect(Player player)
    {
        canvas.gameObject.SetActive(true);
        inMissionSelect = true;
        //Freeze player movement
        player.FreezePlayerMovement(true);
        //Toggle UI
        UIManager.instance.MenuMode(true);
    }

    private void ExitMissionSelect()
    {
        canvas.gameObject.SetActive(false);
        inMissionSelect = false;
        //Freeze player movement
        LevelManager.instance.player.FreezePlayerMovement(false);
        //Toggle UI
        UIManager.instance.MenuMode(false);
    }

    private void ExitMissionSelect(Player player)
    {
        canvas.gameObject.SetActive(false);
        inMissionSelect = false;
        //Freeze player movement
        player.FreezePlayerMovement(false);
        //Toggle UI
        UIManager.instance.MenuMode(false);
    }

}
