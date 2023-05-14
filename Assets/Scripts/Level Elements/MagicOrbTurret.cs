using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicOrbTurret : FixedProjectileTurret
{
    [HideInInspector]
    public Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void FireTurret()
    {
        if (timeBetweenShotsTimer > 0)
        {
            return;
        }

        Projectile newProjectile = Instantiate(projectile, firePoint.position, directionOfFire);

        newProjectile.SetVelocity(directionOfFire * Vector2.right * projectileSpeed);
        newProjectile.SetDamage(minDamage, maxDamage);
        newProjectile.SetLifeTime(projectileLifeTime);

        ResetTimeBetweenShots();
        animator.SetTrigger("fired");
    }
}
