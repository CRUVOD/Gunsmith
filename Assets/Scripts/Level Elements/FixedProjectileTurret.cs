using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static turret in the wall that fires projectiles
/// </summary>
public class FixedProjectileTurret : MonoBehaviour
{
    public enum FiringDirection { Up, Down, Left, Right};
    protected Quaternion directionOfFire;
    //The direction that the turret fires, 90 degree angles only
    public FiringDirection firingDirection;
    public Transform firePoint;
    //The projectile that the turret fires
    public Projectile projectile;
    //projectile damage
    public int minDamage;
    public int maxDamage;
    public float projectileSpeed;
    //how long the projectile lasts for
    public float projectileLifeTime;
    public float timeBetweenShots;
    //The delay/offset in time the turret will fire
    [Range(0f, 5f)]
    public float timeOffSet = 0f;
    //time between shots, calculated from fireRate
    protected float timeBetweenShotsTimer;

    protected virtual void Start()
    {
        InitialiseDirectionOfFire();
        timeBetweenShotsTimer = timeOffSet;
    }

    /// <summary>
    /// Takes the firingDirection enum and set the correct vector2 direction to fire
    /// </summary>
    private void InitialiseDirectionOfFire()
    {
        Vector2 directionOfFireVector = Vector2.up;

        switch (firingDirection)
        {
            case FiringDirection.Up:
                directionOfFireVector = Vector2.up;
                break;
            case FiringDirection.Down:
                directionOfFireVector = Vector2.down;
                break;
            case FiringDirection.Left:
                directionOfFireVector = Vector2.left;
                break;
            case FiringDirection.Right:
                directionOfFireVector = Vector2.right;
                break;
        }

        float rotZ = Mathf.Atan2(directionOfFireVector.y, directionOfFireVector.x) * Mathf.Rad2Deg;
        directionOfFire = Quaternion.Euler(0, 0, rotZ);
    }

    public virtual void Update()
    {
        countDownTimeBetweenShots();
        FireTurret();
    }

    public void ResetTimeBetweenShots()
    {
        timeBetweenShotsTimer = timeBetweenShots;
    }

    /// <summary>
    /// counts down the timer between shots, call every update
    /// </summary>
    public virtual void countDownTimeBetweenShots()
    {
        if (timeBetweenShotsTimer > 0)
        {
            timeBetweenShotsTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Fires the turret if it can
    /// </summary>
    protected virtual void FireTurret()
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
    }
}
