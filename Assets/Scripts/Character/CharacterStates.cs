using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The various states you can use to check if your character is doing something at the current frame
/// </summary>    
public class CharacterStates
{
    /// The possible character conditions
    public enum CharacterConditions
    {
        Normal,
        ControlledMovement,
        Frozen,
        Paused,
        Dead,
        Stunned
    }

    /// The possible Movement States the character can be in. These usually correspond to their own class, 
    /// but it's not mandatory
    public enum MovementStates
    {
        Null,
        Idle,
        Falling,
        Moving,
        Dodging,
        Pushing,
    }
}
