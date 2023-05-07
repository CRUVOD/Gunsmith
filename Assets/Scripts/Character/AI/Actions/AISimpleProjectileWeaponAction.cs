using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISimpleProjectileWeaponAction : AIAction
{
    public enum FiringBehaviour { Burst, Other}

    public ProjectileWeapon projectileWeapon;
    public FiringBehaviour firingBehaviour;
    public bool applyRecoil;

    //If the AI is currently using the projectile weapon
    bool isFiring = false;

    /// <summary>
    /// Fires the weapon in bursts, the fields required for that to happen
    /// </summary>
    public int shotsPerBurst;
    public float timeBetweenFiring;
    public float timeBetweenBursts;
    float timeBetweenBurstsTimer;

    private void Start()
    {
        timeBetweenBurstsTimer = -1;
    }

    private void Update()
    {
        if (timeBetweenBurstsTimer > 0)
        {
            timeBetweenBurstsTimer -= Time.deltaTime;
        }
    }

    public override void PerformAction()
    {
        TryUseWeapon();
    }

    private void TryUseWeapon()
    {
        if (!isFiring)
        {
            //We don't need to check if weapon is ready to fire or not with the function ReadyToFire(), since this is an AI
            switch (firingBehaviour)
            {
                case (FiringBehaviour.Burst):
                    if (timeBetweenBurstsTimer < 0)
                    {
                        StartCoroutine(BurstFire());
                    }
                    break;
            }
        }
    }

    protected virtual IEnumerator BurstFire()
    {
        isFiring = true;
        for (int i = 0; i < shotsPerBurst; i++)
        {
            yield return new WaitForSeconds(timeBetweenFiring);
            bool success = projectileWeapon.Use();
            if (success && applyRecoil)
            {
                core.Owner.Impact(-projectileWeapon.weaponDirection, projectileWeapon.recoil);
            }
        }
        timeBetweenBurstsTimer = timeBetweenBursts;
        isFiring = false;
    }

    public override void OnExitState()
    {
        base.OnExitState();
        projectileWeapon.StopWeapon();
        StopAllCoroutines();
        isFiring = false;
    }
}
