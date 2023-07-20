using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnterGiveLoadout : MonoBehaviour
{
    //bool to keep track if current loadout has been given to player
    private bool loadoutGiven;
    private Player player;

    private void Awake()
    {
        loadoutGiven = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && !loadoutGiven)
        {
            player = collision.gameObject.GetComponent<Player>();
            loadoutGiven = true;

            player.UpdateLoadout(SaveSystem.LoadPlayer()); 
        }
    }

    /// <summary>
    /// Makes it so this script can be activated again and give the player
    /// the loadout again once entering
    /// </summary>
    public void ResetLoadoutGiven()
    {
        loadoutGiven = false;
    }
}
