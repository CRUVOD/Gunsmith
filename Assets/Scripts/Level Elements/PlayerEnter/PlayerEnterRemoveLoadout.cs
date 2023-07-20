using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnterRemoveLoadout : MonoBehaviour
{
    //bool to keep track if current loadout has been given to player
    private Player player;
    public PlayerEnterGiveLoadout giveLoadout;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            player = collision.gameObject.GetComponent<Player>();
            player.ClearLoadout();

            giveLoadout?.ResetLoadoutGiven();
        }
    }
}
