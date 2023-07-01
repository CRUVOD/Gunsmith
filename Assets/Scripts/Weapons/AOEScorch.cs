using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

/// <summary>
/// This AOE applies damage once to characters inside it, with a delay before damage is triggered/counted
/// </summary>
public class AOEScorch : AOESpellInstance
{
    public DamageableDetector damageableDetector;

    public int damage;
    public float invincibilityDuration;

    [Header("Alpha change during cast")]
    public float startAlpha;
    public float endAlpha;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        //Disable animator at start to allow for alpha transition
        animator.enabled = false;
    }

    public void SetSingleDamageProperties(float radius, int damage, float invincibilityDuration)
    {
        this.damage = damage;
        this.invincibilityDuration = invincibilityDuration;

        //since this is a single damage instance, we force the instance to be single instant effect
        effectTime = 0;
        effectTimer = 0;
        singleEffect = true;
    }

    public override void Cast()
    {
        base.Cast();
        //During cast, we fade in the sprite
        float targetAlpha = math.pow((math.remap(0, castTime, startAlpha, endAlpha, castTime - castTimer) / 100), 2);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, targetAlpha);
    }

    protected override void Activate()
    {
        base.Activate();
        animator.enabled = true;
        animator?.SetTrigger("Activate");
    }

    protected override void Effect()
    {
        base.Effect();
        IDamageable[] damageables = damageableDetector.GetIDamageables().ToArray();
        foreach(IDamageable damageable in damageables)
        {
            //For now we don't apply any force
            damageable.Damage(damage, this.gameObject, invincibilityDuration, 0);
        }
    }
}
