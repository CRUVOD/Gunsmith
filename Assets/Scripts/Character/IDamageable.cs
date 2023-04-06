using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(int damage, GameObject instigator, float invincibilityDuration, float force);
    void Damage(int damage, GameObject instigator, float invincibilityDuration, Vector3 direciton, float force);
    bool CanTakeDamageThisFrame();
}
