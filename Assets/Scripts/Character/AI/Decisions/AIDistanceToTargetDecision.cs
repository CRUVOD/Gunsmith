using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Decision will return true if the current Core's Target is within the specified range, false otherwise.
/// </summary>
public class AIDistanceToTargetDecision : AIDecision
{
    /// The possible comparison modes
    public enum ComparisonModes { StrictlyLowerThan, LowerThan, Equals, GreaterThan, StrictlyGreaterThan }
    /// the comparison mode
    [Tooltip("the comparison mode")]
    public ComparisonModes ComparisonMode = ComparisonModes.GreaterThan;
    /// the distance to compare with
    [Tooltip("the distance to compare with")]
    public float Distance;

    /// <summary>
    /// On Decide we check our distance to the Target
    /// </summary>
    /// <returns></returns>
    public override bool Decide()
    {
        return EvaluateDistance();
    }

    /// <summary>
    /// Returns true if the distance conditions are met
    /// </summary>
    /// <returns></returns>
    protected virtual bool EvaluateDistance()
    {
        if (core.TargetPosition == null)
        {
            return false;
        }

        float distance = Vector3.Distance(this.transform.position, core.TargetPosition);

        if (ComparisonMode == ComparisonModes.StrictlyLowerThan)
        {
            return (distance < Distance);
        }
        if (ComparisonMode == ComparisonModes.LowerThan)
        {
            return (distance <= Distance);
        }
        if (ComparisonMode == ComparisonModes.Equals)
        {
            return (distance == Distance);
        }
        if (ComparisonMode == ComparisonModes.GreaterThan)
        {
            return (distance >= Distance);
        }
        if (ComparisonMode == ComparisonModes.StrictlyGreaterThan)
        {
            return (distance > Distance);
        }
        return false;
    }
}
