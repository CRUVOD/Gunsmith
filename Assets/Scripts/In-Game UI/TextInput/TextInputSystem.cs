using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInputSystem : MonoBehaviour
{
    public static TextInputSystem instance;
    public TextInputPrompt inputPrompt;

    public delegate void ReturnDelegate(string text);
    public ReturnDelegate returnDelegate;

    void Awake()
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

    /// <summary>
    /// Shows the text input, we also time when the prompt is open
    /// </summary>
    /// <param name="title"></param>
    /// <param name="returnDelegate"></param>
    public static void Prompt(string title, ReturnDelegate returnDelegate)
    {
        TimeScaleEvent.Trigger(TimeScaleMethods.For, 0, 1, false, 0, true);
        instance.inputPrompt.SetText(title, returnDelegate);
        instance.inputPrompt.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the text input, and resumes time
    /// </summary>
    public static void Close()
    {
        TimeScaleEvent.Trigger(TimeScaleMethods.Unfreeze, 1, 1, false, 0, false);
        instance.inputPrompt.gameObject.SetActive(false);
    }
}
