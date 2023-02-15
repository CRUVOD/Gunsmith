using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrenade : GrenadeWeapon
{
    [Header("HandGrenade")]
    [Tooltip("show the cooldown duration between shots in the cooldown circle")]
    //Don't set to true if the fire rate is high
    public bool useCoolDownCircleUIBetweenShots;

    public FeedbackPlayer weaponUseFeedback;

    /// <summary>
    /// Creates a new grenade and send it flying
    /// </summary>
    /// <returns></returns>
    public override bool FireGrenade()
    {
        bool ready = ReadyToFire();

        if (ready)
        {
            Grenade newGrenade;

            //Instantiate the bullet and send it flying
            if (User == CharacterTypes.Player)
            {
                newGrenade = Instantiate(grenade, firePoint.position, MouseDirectionQuaternion());
            }
            else
            {
                newGrenade = Instantiate(grenade, firePoint.position, PlayerDirectionQuaternion());
            }

            newGrenade.SetVelocity(weaponDirection.normalized * grenadeSpeed);
            newGrenade.SetDamage(impactMinDamage, impactMaxDamage);
            newGrenade.SetLifeTime(lifeTime);
            newGrenade.SetExplosionDamage(explosionDamage);

            //Reset time between shots according to fire rate
            ResetTimeBetweenShots();
            if (useCoolDownCircleUIBetweenShots)
            {
                UIManager.instance.crosshair.StartCoolDownUI(60f / fireRate);
            }

            //Calls to play feedback
            //weaponUseFeedback.PlayFeedbacks();

            return true;
        }

        return false;
    }
}
