using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGrenadeWeaponAction : AIAction
{
    public enum FiringBehaviour { Burst, Other}

    public GrenadeWeapon grenadeWeapon;
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
        if (timeBetweenBurstsTimer >= 0)
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
        if (grenadeWeapon.ReadyToFire() && !isFiring)
        {
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
            bool success = grenadeWeapon.Use();
            if (success && applyRecoil)
            {
                core.Owner.Impact(-grenadeWeapon.weaponDirection, grenadeWeapon.recoil);
            }
            yield return new WaitForSeconds(timeBetweenFiring);
        }
        timeBetweenBurstsTimer = timeBetweenBursts;
        isFiring = false;
    }

    public override void OnExitState()
    {
        base.OnExitState();
        grenadeWeapon.StopWeapon();
        StopAllCoroutines();
        isFiring = false;
    }
}
