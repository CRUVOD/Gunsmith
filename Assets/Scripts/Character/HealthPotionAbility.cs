using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grants the player the ability to use health potions
/// </summary>
public class HealthPotionAbility : MonoBehaviour
{
    public int potionsMaxAmount;
    [HideInInspector]
    public int potionsInInventory;
    public float potionHealAmount;

    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
        potionsInInventory = potionsMaxAmount;
        UIManager.instance.potionUI.SetHealthPotionsAmountDisplay(potionsInInventory);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            TryUseHealingPotion();
        }
    }

    /// <summary>
    /// Uses a healing potion on the player
    /// </summary>
    private void TryUseHealingPotion()
    {
        if (player == null)
        {
            //player does not exist
            return;
        }

        if (potionsInInventory <= 0)
        {
            //we have no more potions
            return;
        }

        if (player.CurrentHealth == player.InitialHealth)
        {
            //player has full health
            return;
        }

        float targetHealthAmount = Math.Clamp(player.CurrentHealth + potionHealAmount, 0 , player.InitialHealth);
        player.SetHealth(targetHealthAmount);
        potionsInInventory -= 1;
        UIManager.instance.potionUI.SetHealthPotionsAmountDisplay(potionsInInventory);
    }

}
