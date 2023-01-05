using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Decision will return true if the current Core's Enemy component is stunned
/// </summary>
public class AIIsStunnedDecision : AIDecision
{
    public override bool Decide()
    {
        return core.Owner.isStunned;
    }
}
