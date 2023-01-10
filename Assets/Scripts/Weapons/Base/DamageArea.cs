using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class DamageArea : MonoBehaviour
{
    public Collider2D damageCollider;

    /// the layers that will be damaged by this object
    [Tooltip("the layers that will be damaged by this object")]
    public LayerMask TargetLayerMask;

    [Header("Events")]
    /// Events
    /// an event to trigger when hitting a Damageable
    public UnityEvent<Character> HitDamageableEvent;
    /// an event to trigger when hitting a non Damageable
    public UnityEvent<GameObject> HitNonDamageableEvent;

    [Flags]
    public enum TriggerAndCollisionMask
    {
        IgnoreAll = 0,
        OnTriggerEnter = 1 << 0,
        OnTriggerStay = 1 << 1,
        OnTriggerEnter2D = 1 << 6,
        OnTriggerStay2D = 1 << 7,

        All_3D = OnTriggerEnter | OnTriggerStay,
        All_2D = OnTriggerEnter2D | OnTriggerStay2D,
        All = All_3D | All_2D
    }

    protected const TriggerAndCollisionMask AllowedTriggerCallbacks = TriggerAndCollisionMask.OnTriggerEnter
                                                                  | TriggerAndCollisionMask.OnTriggerStay
                                                                  | TriggerAndCollisionMask.OnTriggerEnter2D
                                                                  | TriggerAndCollisionMask.OnTriggerStay2D;

    /// Defines on what triggers the damage should be applied, by default on enter and stay (both 2D and 3D) but this field will let you exclude triggers if needed
    [Tooltip(
        "Defines on what triggers the damage should be applied, by default on enter and stay (both 2D and 3D) but this field will let you exclude triggers if needed")]
    public TriggerAndCollisionMask TriggerFilter = AllowedTriggerCallbacks;

    #region CollisionDetection

    /// <summary>
    /// When a collision with the player is triggered, we give damage to the player and knock it back
    /// </summary>
    /// <param name="collider">what's colliding with the object.</param>
    public virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerStay2D)) return;
        Colliding(collider.gameObject);
    }

    /// <summary>
    /// On trigger enter 2D, we call our colliding endpoint
    /// </summary>
    /// <param name="collider"></param>S
    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerEnter2D)) return;
        Colliding(collider.gameObject);
    }

    /// <summary>
    /// On trigger stay, we call our colliding endpoint
    /// </summary>
    /// <param name="collider"></param>
    public virtual void OnTriggerStay(Collider collider)
    {
        if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerStay)) return;
        Colliding(collider.gameObject);
    }

    /// <summary>
    /// On trigger enter, we call our colliding endpoint
    /// </summary>
    /// <param name="collider"></param>
    public virtual void OnTriggerEnter(Collider collider)
    {
        if (0 == (TriggerFilter & TriggerAndCollisionMask.OnTriggerEnter)) return;
        Colliding(collider.gameObject);
    }

    #endregion

    #region OnCollision

    /// <summary>
    /// When colliding, we apply the appropriate damage
    /// </summary>
    /// <param name="collider"></param>
    protected virtual void Colliding(GameObject collider)
    {
        if (!EvaluateAvailability(collider))
        {
            return;
        }

        Character collidedCharacter = collider.gameObject.GetComponent<Character>();

        // if what we're colliding with is damageable
        if (collidedCharacter != null)
        {
            HitDamageableEvent?.Invoke(collidedCharacter);
        }
        else // if what we're colliding with can't be damaged
        {
            HitNonDamageableEvent?.Invoke(collider);
        }
    }

    /// <summary>
    /// Checks whether or not damage should be applied this frame
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    protected virtual bool EvaluateAvailability(GameObject collider)
    {
        // if we're inactive, we do nothing
        if (!isActiveAndEnabled) { return false; }
        // if what we're colliding with isn't part of the target layers, we do nothing and exit
        if (!ExtraLayers.LayerInLayerMask(collider.layer, TargetLayerMask)) { return false; }

        // if we're on our first frame, we don't apply damage
        if (Time.time == 0f) { return false; }
        return true;
    }

    #endregion

    
}
