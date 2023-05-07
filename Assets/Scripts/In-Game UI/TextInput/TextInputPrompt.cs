using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextInputPrompt : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TMP_InputField inputPrompter;

    public TextInputSystem.ReturnDelegate returnDelegate;

    private void Awake()
    {
        inputPrompter.onEndEdit.AddListener(OnEndEdit);
    }

    internal void SetText(string title, TextInputSystem.ReturnDelegate returnDelegate)
    {
        this.title.text = title + ":";
        this.returnDelegate = returnDelegate;
    }

    /// <summary>
    /// When the text prompt is entered/submitted, this function gets called
    /// </summary>
    /// <param name="text"></param>
    public void OnEndEdit(string text)
    {
        //Pass to the call back function
        returnDelegate?.Invoke(text);
        //Hide the text input
        TextInputSystem.Close();
    }
}
