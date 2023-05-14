using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedFloorSpikes : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;

    [Tooltip("How long each cycle takes, must be larger than delay+active duration")]
    public float cycleDuration;
    float cycleTimer;
    //The delay/offset in time the the floor spike will operate on
    [Range(0f, 5f)]
    public float timeOffSet = 0f;

    [Header("Damage")]
    public int MinDamageCaused;
    public int MaxDamageCaused;
    public float InvincibilityDuration;

    [Header("Damage Area")]

    [Tooltip("an existing damage area to activate/handle as the weapon is used")]
    public DamageArea DamageArea;

    /// the initial delay to apply before triggering the damage area
    [Tooltip("the initial delay to apply before triggering the damage area")]
    public float InitialDelay = 0f;
    /// the duration during which the damage area is active
    [Tooltip("the duration during which the damage area is active")]
    public float ActiveDuration = 1f;

    [Tooltip("the feedback to play when hitting a Damageable")]
    public FeedbackPlayer HitDamageableFeedback;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (cycleDuration < InitialDelay + ActiveDuration)
        {
            cycleDuration = InitialDelay + ActiveDuration + 0.1f;
        }
        cycleTimer = cycleDuration - (InitialDelay + ActiveDuration) + timeOffSet;
        DamageArea.HitDamageableEvent.AddListener(OnCollideWithDamageable);
    }

    private void Update()
    {
        CountDownCycle();
    }

    private void CountDownCycle()
    {
        if (cycleTimer <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Activate());
            cycleTimer = cycleDuration - (InitialDelay + ActiveDuration);
        }
        else
        {
            cycleTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Enables the damage area.
    /// </summary>
    protected virtual void EnableDamageArea()
    {
        if (DamageArea != null)
        {
            DamageArea.damageCollider.enabled = true;
        }
    }


    /// <summary>
    /// Disables the damage area.
    /// </summary>
    protected virtual void DisableDamageArea()
    {
        if (DamageArea != null)
        {
            DamageArea.damageCollider.enabled = false;
        }
    }

    /// <summary>
    /// Triggers an attack, turning the damage area on and then off, also playing the animation for attacking
    /// </summary>
    /// <returns>The weapon attack.</returns>
    protected virtual IEnumerator Activate()
    {
        animator.SetTrigger("activated");
        yield return new WaitForSeconds(InitialDelay);
        EnableDamageArea();
        yield return new WaitForSeconds(ActiveDuration);
        DisableDamageArea();
    }

    /// <summary>
    /// Describes what happens when colliding with a damageable object
    /// </summary>
    /// <param name="health">Health.</param>
    protected virtual void OnCollideWithDamageable(IDamageable damageable)
    {
        if (!damageable.CanTakeDamageThisFrame())
        {
            return;
        }

        HitDamageableFeedback?.PlayFeedbacks(this.transform.position);

        //we apply the damage to the thing we've collided with
        int randomDamage = (int)UnityEngine.Random.Range(MinDamageCaused, Mathf.Max(MaxDamageCaused, MinDamageCaused));

        //Apply knockback/impact force on collided character and damage
        damageable.Damage(randomDamage, gameObject, InvincibilityDuration, Vector3.zero, 0);
    }
}
