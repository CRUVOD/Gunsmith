using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI will randomly perform one of the actions listed based on the weight assigned to them
/// </summary>
public class AIRandomAction : AIAction
{
    public AIRandomActionChoice[] actions;

    private float totalWeight;
    private AIAction currentAction;

    private void Start()
    {
        CalculateTotalWeight();
    }

    private void CalculateTotalWeight()
    {
        for (int i = 0; i < actions.Length; i++)
        {
            totalWeight += actions[i].weight;
        }
    }

    /// <summary>
    /// Selects random action based on weights
    /// </summary>
    /// <returns></returns>
    private AIAction SelectRandomAction()
    {
        float selection = UnityEngine.Random.Range(0, totalWeight);
        for (int i = 0; i < actions.Length; i++)
        {
            selection -= actions[i].weight;
            if (selection < 0)
            {
                //Return the first one which causes selection to be negative
                return actions[i].action;
            }
        }
        //Return the last one
        return actions[actions.Length - 1].action;
    }

    /// <summary>
    /// Selects an action to perform on enter
    /// </summary>
    public override void OnEnterState()
    {
        base.OnEnterState();
        currentAction = SelectRandomAction();
        currentAction.OnEnterState();
    }

    /// <summary>
    /// Performs the selected action
    /// </summary>
    public override void PerformAction()
    {   
        currentAction.PerformAction();
    }

    /// <summary>
    /// Perform the action's on exit state methods on exit
    /// </summary>
    public override void OnExitState()
    {
        base.OnExitState();
        currentAction.OnExitState();
    }

    /// <summary>
    /// Returns the label of the currently selected aciton
    /// </summary>
    /// <returns></returns>
    public string GetSelectedActionLabel()
    {
        if (currentAction != null)
        {
            return currentAction.Label;
        }
        return "NoAction";
    }
}

/// <summary>
/// Simple class that assigns each action with a weight for random selection
/// </summary>
[System.Serializable]
public class AIRandomActionChoice
{
    public AIAction action;
    public float weight;
}
