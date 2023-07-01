using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Similar to a damage area, this script keeps tracks of a list of damageables inside this collider
/// This can be returned/accessed to find all IDamageables inside at a specific time
/// </summary>
public class DamageableDetector : MonoBehaviour
{
    [Tooltip("the layers that will be detected by this object")]
    public LayerMask TargetLayerMask;


    private List<IDamageable> IDamageables = new List<IDamageable>();
    public List<IDamageable> GetIDamageables() { return IDamageables; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable collidedDamageable = collision.gameObject.GetComponent<IDamageable>();

        // if what we're colliding with is not damageable
        if (collidedDamageable == null)
        {
            return;
        }

        if (!IDamageables.Contains(collidedDamageable) && EvaluateAvailability(collision.gameObject))
        {
            IDamageables.Add(collidedDamageable);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        IDamageable collidedDamageable = collision.gameObject.GetComponent<IDamageable>();

        // if what we're colliding with is not damageable
        if (collidedDamageable == null)
        {
            return;
        }

        if (!IDamageables.Contains(collidedDamageable) && EvaluateAvailability(collision.gameObject))
        {
            IDamageables.Add(collidedDamageable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IDamageable collidedDamageable = collision.gameObject.GetComponent<IDamageable>();

        // if what we're colliding with is not damageable
        if (collidedDamageable == null)
        {
            return;
        }

        IDamageables.Remove(collidedDamageable);
    }

    /// <summary>
    /// Checks if collider is in the target layer
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    protected virtual bool EvaluateAvailability(GameObject collider)
    {
        // if we're inactive, we do nothing
        if (!isActiveAndEnabled) { return false; }
        // if what we're colliding with isn't part of the target layers, we do nothing and exit
        if (!ExtraLayers.LayerInLayerMask(collider.layer, TargetLayerMask)) { return false; }

        IDamageable collidedDamageable = collider.gameObject.GetComponent<IDamageable>();

        // if we're on our first frame, we don't apply count this collision
        if (Time.time == 0f) { return false; }
        return true;
    }
}
