using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using System.Text.RegularExpressions;
using TMPro;
using System;
using UnityEngine.Audio;

public class DialogueBox : MonoBehaviour
{
    public Image potrait;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;
    [HideInInspector]
    public Animator animator;
    [Header("Audio")]
    public AudioClip dialogueBoxSFX;
    public AudioMixerGroup targetAudioMixerGroup;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Show()
    {
        animator.SetBool("IsOpen", true);
    }

    public void Hide()
    {
        animator.SetBool("IsOpen", false);
    }

    /// <summary>
    /// Public method called by dialogue system to set the text
    /// </summary>
    /// <param name="name"></param>
    /// <param name="content"></param>
    /// <param name="potrait"></param>
    public void SetText(string name, string content, Sprite potrait)
    {
        nameText.text = ProcessTextContent(name) + ":";
        this.potrait.sprite = potrait;
        StopAllCoroutines();
        StartCoroutine(TypeContent(ProcessTextContent(content)));
    }

    /// <summary>
    /// Process the content to replace with variables
    /// </summary>
    /// <returns></returns>
    private string ProcessTextContent(string content)
    {
        Regex regex = new Regex("(?<={{)(.*?)(?=}})");
        MatchCollection matches = regex.Matches(content);

        for (int i = 0; i < matches.Count; i++)
        {
            content = content.Replace("{{" + matches[i] + "}}", GetVariableValue(matches[i]));
        }

        return content;
    }

    /// <summary>
    /// Input a variable in dialogue, and returns the according variable value from data
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    private string GetVariableValue(Match match)
    {
        switch (match.Value)
        {
            case "PlayerName":
                return SaveSystem.LoadPlayer().playerName;
            default:
                return "Unknown value";
        }
    }

    IEnumerator TypeContent(string content)
    {
        contentText.text = "";
        //Play the sfx at the very start of each dialogue
        SfxEvent.Trigger(dialogueBoxSFX, targetAudioMixerGroup, 0.65f);
        foreach (char letter in content.ToCharArray())
        {
            contentText.text += letter;

            if (letter.ToString().Equals(" "))
            {
                //Then we play the sfx when there is a space character
                SfxEvent.Trigger(dialogueBoxSFX, targetAudioMixerGroup, 0.65f);
            }


            yield return new WaitForSeconds(0.03f);
        }
    }
}
