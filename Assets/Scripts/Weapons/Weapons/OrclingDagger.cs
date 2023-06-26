using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrclingDagger : MeleeWeapon
{
    public override void StopWeapon()
    {
        StopAllCoroutines();
        DisableDamageArea();
        attackInProgress = false;
        WeaponUsedFeedback?.StopFeedbacks();
    }

    protected override IEnumerator MeleeWeaponAttack()
    {
        if (attackInProgress) { yield break; }

        attackInProgress = true;
        WeaponUsedFeedback?.PlayFeedbacks();
        Animator.SetTrigger("Attacking");
        yield return new WaitForSeconds(InitialDelay);
        EnableDamageArea();
        yield return new WaitForSeconds(ActiveDuration);
        DisableDamageArea();
        yield return new WaitForSeconds(CoolDownDuration);
        attackInProgress = false;
    }
}
