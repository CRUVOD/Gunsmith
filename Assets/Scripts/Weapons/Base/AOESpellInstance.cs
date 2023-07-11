using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used by staffs/magic spells to create AOE effects
/// </summary>
public class AOESpellInstance : MonoBehaviour
{
    [SerializeField]
    //The radius/size of the sprite in world length by default"
    protected float defaultRadius;

    //How long it takes to cast this AOE instance
    public float castTime;
    protected float castTimer;

    public bool successfullyCast { get; protected set; }

    //How long the AOE lasts for when activated/successfully casted, if it is zero, we only call the effect function once
    public float effectTime;
    protected float effectTimer;

    //bool to determine if we only call the effect function once
    protected bool singleEffect = false;
    private bool AOEfinished;

    //The time after effect has ended to destroy this gameobject, used to allow for lingering animations
    public float timeToDestroyAfterFinish;

    private void Awake()
    {
        successfullyCast = false;
        castTimer = castTime;
        effectTimer = effectTime;

        if (effectTime == 0)
        {
            singleEffect = true;
        }
    }

    protected virtual void Update()
    {
        if (successfullyCast && !AOEfinished)
        { 
            if (effectTimer <= 0 && !singleEffect)
            {
                //We have finished the effective duration of this AOE
                AOEfinished = true;
                AOEFinish();
                return;
            }

            if (!singleEffect)
            {
                //Apply the AOE effect
                effectTimer -= Time.deltaTime;
                Effect();
            }
            else
            {
                //If it is a single instant effect spell, we call the effect function once and exit
                Effect();
                AOEfinished = true;
                AOEFinish();
                return;
            }
        }
    }

    /// <summary>
    /// Override basic properties of this AOE spell instance
    /// </summary>
    /// <param name="castTime"></param>
    /// <param name="effectTime"></param>
    public void SetBasicProperties(float castTime, float effectTime)
    {
        successfullyCast = false;

        this.castTime = castTime;
        castTimer = castTime;
        this.effectTime = effectTime;
        effectTimer = effectTime;

        if (effectTime == 0)
        {
            singleEffect = true;
        }
    }

    /// <summary>
    /// This AOE instance is being casted/activated by a weapon, to be called by another class/weapon every update
    /// </summary>
    public virtual void Cast()
    {
        if (successfullyCast)
        {
            return;
        }

        castTimer -= Time.deltaTime;

        if (castTimer <= 0)
        {
            //we have successfully casted this AOE
            Activate();
        }
    }

    /// <summary>
    /// The casting for this AOE instance has been cancelled
    /// </summary>
    public virtual void CancelCast()
    {
        castTimer = castTime;
        successfullyCast = false;
        //Disable srpite, and destroy with slight delay by default
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(this.gameObject, 0.1f);
    }

    /// <summary>
    /// Successfully casted
    /// </summary>
    protected virtual void Activate()
    {
        successfullyCast = true;
    }

    /// <summary>
    /// Apply the effect of the AOE, countdown is in the update of the AOE spell instance class
    /// Called every update if not single effect
    /// </summary>
    protected virtual void Effect()
    {

    }

    /// <summary>
    /// The AOE has finished and we destory this instance
    /// </summary>
    public void AOEFinish()
    {
        Destroy(this.gameObject, timeToDestroyAfterFinish);
    }

}
