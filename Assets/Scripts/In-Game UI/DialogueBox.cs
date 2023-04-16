using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    public Image potrait;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;
    [HideInInspector]
    public Animator animator;

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

    public void SetText(string name, string content, Sprite potrait)
    {
        nameText.text = name + ":";
        this.potrait.sprite = potrait;
        StopAllCoroutines();
        StartCoroutine(TypeContent(content));
    }

    IEnumerator TypeContent(string content)
    {
        contentText.text = "";
        foreach (char letter in content.ToCharArray())
        {
            contentText.text += letter;
            yield return null;
        }
    }
}
