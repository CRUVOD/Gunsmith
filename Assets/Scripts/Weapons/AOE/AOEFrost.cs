using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEFrost : AOESpellInstance
{
    public DamageableDetector damageableDetector;

    public int damage;
    public float invincibilityDuration;
    //Time between ticks, aka times at which we deal damage to things inside
    public float tickRate;
    public float speedModifier;

    private float tickTimer;

    private Animator animator;

    private void Start()
    {
        tickTimer = 0;
    }

    public override void Cast()
    {
        base.Cast();
    }

    protected override void Effect()
    {
        base.Effect();
        tickTimer -= Time.deltaTime;

        if (tickTimer <= 0)
        {
            //A tick has been reached, we apply damage
            IDamageable[] damageables = damageableDetector.GetIDamageables().ToArray();
            foreach (IDamageable damageable in damageables)
            {
                //For now we don't apply any force
                damageable.Damage(damage, this.gameObject, invincibilityDuration, 0);
            }
            tickTimer = tickRate;
        }
    }
}
