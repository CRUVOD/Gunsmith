using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthPotionUI : MonoBehaviour
{
    public TextMeshProUGUI healthPotionsAmountText;

    public void SetHealthPotionsAmountDisplay(int amount)
    {
        healthPotionsAmountText.SetText(amount.ToString());
    }
}
