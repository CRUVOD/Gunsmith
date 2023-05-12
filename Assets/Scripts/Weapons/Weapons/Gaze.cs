using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Weapon used by AI to hit player if they are in line of sight
/// </summary>
public class Gaze : HitScanWeapon
{
    [Header("Gaze")]
    //The indicator that will show for the player to know that a gaze check is coming
    public GameObject gazeIndicator;
    //Time in seconds after which Use() is called, the gaze check is activated
    public float gazeDelay;

    protected override void Start()
    {
        base.Start();
        gazeIndicator.SetActive(false);
        ToggleWeaponRotation(false);
    }

    /// <summary>
    /// Gaze is always ready to fire
    /// </summary>
    /// <returns></returns>
    public override bool ReadyToFire()
    {
        return true;
    }

    public override bool FireRaycast()
    {
        StartCoroutine(GazeAttack());
        return true;
    }

    private IEnumerator GazeAttack()
    {
        gazeIndicator.SetActive(true);
        yield return new WaitForSeconds(gazeDelay);
        gazeIndicator.SetActive(false);

        RaycastHit2D hit;

        Vector2 playerDirection = PlayerDirectionVector2();

        hit = Physics2D.Raycast(firePoint.position, playerDirection, rayTravelDistance, TargetLayerMask);

        if (hit)
        {
            Colliding(hit.collider.gameObject);
        }
    }

    /// <summary>
    /// Ends the coroutine that activates the gaze check
    /// </summary>
    public override void StopWeapon()
    {
        StopAllCoroutines();
        gazeIndicator.SetActive(false);
    }

    /// <summary>
    /// Overrides so that we correctly calcualtes knockback direction since weapon direction is frozen
    /// </summary>
    /// <param name="damageable"></param>
    protected override void OnCollideWithDamageable(IDamageable damageable)
    {
        HitDamageableFeedback?.PlayFeedbacks(this.transform.position);

        //we apply the damage to the thing we've collided with
        int randomDamage = (int)UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));

        Vector2 playerDirection = PlayerDirectionVector2();
        //Apply knockback/impact force on collided character
        damageable.Damage(randomDamage, gameObject, InvincibilityDuration, playerDirection, impactForce);
    }
}
