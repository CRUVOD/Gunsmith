using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private BoxCollider2D doorCollider;
    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public bool isOpen { get; private set; }

    private void Start()
    {
        doorCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        Open();
    }

    /// <summary>
    /// Opens the door, disabling collisions
    /// </summary>
    public void Open()
    {
        isOpen = true;
        doorCollider.enabled = false;
        if (animator != null)
        {
            animator.SetBool("isOpen", isOpen);
        }
    }

    public void Close()
    {
        isOpen = false;
        doorCollider.enabled = true;
        if (animator != null)
        {
            animator.SetBool("isOpen", isOpen);
        }
    }
}
