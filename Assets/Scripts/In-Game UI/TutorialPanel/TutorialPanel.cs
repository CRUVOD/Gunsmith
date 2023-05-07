using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    public Image displayImage;
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

    /// <summary>
    /// Sets the tutorial image
    /// </summary>
    /// <param name="image"></param>
    public void SetImage(Sprite image)
    {
        displayImage.sprite = image;
    }
}
