using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script that flips the sprite in the sprite renderer based on the scale transform
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlipWithScale : MonoBehaviour
{
    public bool FlipX;
    public bool FlipY;

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Flip();
    }

    private void Flip()
    {
        if (FlipX && transform.localScale.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (FlipX && transform.localScale.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        if (FlipY && transform.localScale.y < 0)
        {
            spriteRenderer.flipY = true;
        }
        else if (FlipY && transform.localScale.y > 0)
        {
            spriteRenderer.flipY = false;
        }
    }
}
