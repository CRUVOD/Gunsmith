using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    public Image potrait;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;

    public void SetText(string name, string content, Sprite potrait)
    {
        nameText.text = name + ":";
        contentText.text = content;
        this.potrait.sprite = potrait;
    }
}
