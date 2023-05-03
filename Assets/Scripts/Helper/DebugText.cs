using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Simple debug text, can be called by any class to show text on screen rather than through console
/// </summary>
public class DebugText : MonoBehaviour
{
    [HideInInspector]
    public static DebugText instance;
    public TextMeshProUGUI debugText;

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

    public void SetText(string text)
    {
        debugText.text = text;
    }
}
