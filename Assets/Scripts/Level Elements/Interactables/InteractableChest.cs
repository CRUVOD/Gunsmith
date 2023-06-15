using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractableChest : MonoBehaviour, Interactable
{
    [HideInInspector]
    public Animator animator;

    public SpriteRenderer contentIcon;
    public TextMeshPro contentText;

    [Header("Animation")]
    public float delayBeforeShow;
    public float delayBeforeHide;

    //boolean to track if chest has been opened or not, so that it can only give the reward once
    private bool hasBeenOpened;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact(Player player)
    {
        if (!hasBeenOpened)
        {
            GiveChestContent(player);
            hasBeenOpened = true;
            StartCoroutine(Animation());
            animator.SetBool("open", true);
        }
    }

    /// <summary>
    /// Sets the content with a sprite and text to display to viewer
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="text"></param>
    public void SetContent(Sprite sprite, string text)
    {
        contentIcon.sprite = sprite;
        contentText.text = text;
    }

    /// <summary>
    /// This is if the content is currency
    /// </summary>
    /// <param name="amount"></param>
    public void SetContent(int amount)
    {

    }

    /// <summary>
    /// Override this in child scripts to give proper reward/content to the player on interaction
    /// </summary>
    /// <param name="player"></param>
    protected virtual void GiveChestContent(Player player)
    {
        return;
    }

    /// <summary>
    /// Plays the animations for the icon and the etext
    /// </summary>
    /// <returns></returns>
    private IEnumerator Animation()
    {
        Animator iconAnimator = contentIcon.gameObject.GetComponent<Animator>();
        Animator textAnimator = contentText.gameObject.GetComponent<Animator>();

        yield return new WaitForSeconds(delayBeforeShow);

        if (iconAnimator != null && textAnimator != null)
        {
            iconAnimator.SetTrigger("Show");
            textAnimator.SetTrigger("Show");

            yield return new WaitForSeconds(delayBeforeHide);

            iconAnimator.SetTrigger("Hide");
            textAnimator.SetTrigger("Hide");
        }

    }
}
