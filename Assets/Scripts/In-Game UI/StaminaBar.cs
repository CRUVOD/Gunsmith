using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Image staminaBar;

    public void SetStaminaBar(float fillAmount)
    {
        staminaBar.fillAmount = fillAmount;
    }
}
