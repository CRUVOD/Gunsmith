using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actions that takes place before triggering dialogue, and undos/releases after dialogue finish
/// </summary>
public class DialogueAction : MonoBehaviour
{
    /// <summary>
    /// Actions before the dialogue
    /// </summary>
    public virtual void Setup()
    {

    }

    /// <summary>
    /// Actions at the end of the dialogue
    /// </summary>
    public virtual void Release()
    {

    }
}
