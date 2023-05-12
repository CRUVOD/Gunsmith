using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script for the watchful guardian enemy to hook up with its animations correctly
/// </summary>
public class WatchfulGuardian : Enemy
{
    //Additional animations
    protected const string MeleeAttackAnimParametreName = "Melee Attack";
    protected int MeleeAttackAnimParametre;
    protected const string GazeAttackAnimParametreName = "Gaze Attack";
    protected int GazeAttackAnimParametre;

    //Reference to the attack action, for animation purposes
    private AIRandomAction attackAction;

    protected override void Start()
    {
        base.Start();
        //Find the attack state, and have a reference to the attack action
        for (int i = 0; i < core.States.Count; i++)
        {
            if (core.States[i].StateName.Equals("Attack"))
            {
                attackAction = core.States[i].Actions[0] as AIRandomAction;
            }
        }
    }

    /// <summary>
    /// We don't update the facing for this enemy
    /// </summary>
    protected override void HandleFacing()
    {
        return;
    }

    protected override void InitialiseAnimatorParametresExtra()
    {
        AnimatorExtensions.AddAnimatorParameterIfExists(Animator, MeleeAttackAnimParametreName, out MeleeAttackAnimParametre, AnimatorControllerParameterType.Bool, animatorParametres);
        AnimatorExtensions.AddAnimatorParameterIfExists(Animator, GazeAttackAnimParametreName, out GazeAttackAnimParametre, AnimatorControllerParameterType.Bool, animatorParametres);
    }

    protected override void UpdateAnimatorExtra()
    {
        bool inMelee = false;
        bool inGaze = false;

        if (attackAction != null)
        {
            inMelee = attackAction.GetSelectedActionLabel().Equals("Melee");
            inGaze = attackAction.GetSelectedActionLabel().Equals("Gaze");
        }

        AnimatorExtensions.UpdateAnimatorBool(Animator, MeleeAttackAnimParametre, inMelee, animatorParametres, RunAnimatorSanityChecks);
        AnimatorExtensions.UpdateAnimatorBool(Animator, GazeAttackAnimParametre, inGaze, animatorParametres, RunAnimatorSanityChecks);
    }
}