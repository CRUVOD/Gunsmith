using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICore : MonoBehaviour
{
    [Header("Owner")]
    /// the owner of that AI Brain, usually the associated character
    public Enemy Owner;
    /// the collection of states
    public List<AIState> States;
    /// this brain's current state
    public AIState CurrentState { get; protected set; }
    /// the time we've spent in the current state
    [HideInInspector]
    public float TimeInThisState;
    /// the current target
    [HideInInspector]
    public Transform Target;
    /// the last known world position of the target
    [HideInInspector]
    public Vector3 _lastKnownTargetPosition = Vector3.zero;

    [Header("State")]
    /// whether or not this core is active
    public bool CoreActive = true;
    public bool ResetCoreOnStart = true;
    public bool ResetCoreOnEnable = false;

    [Header("Frequencies")]
    /// the frequency (in seconds) at which to perform actions (lower values : higher frequency, high values : lower frequency but better performance)
    public float ActionsFrequency = 0f;
    /// the frequency (in seconds) at which to evaluate decisions
    public float DecisionFrequency = 0f;

    /// whether or not to randomize the action and decision frequencies
    public bool RandomizeFrequencies = false;
    /// the min and max values between which to randomize the action frequency
    public Vector2 RandomActionFrequency = new Vector2(0.5f, 1f);
    /// the min and max values between which to randomize the decision frequency
    public Vector2 RandomDecisionFrequency = new Vector2(0.5f, 1f);

    protected AIDecision[] decisions;
    protected AIAction[] actions;
    protected float lastActionsUpdate = 0f;
    protected float lastDecisionsUpdate = 0f;
    protected AIState initialState;

    public virtual AIAction[] GetAttachedActions()
    {
        AIAction[] actions = this.gameObject.GetComponentsInChildren<AIAction>();
        return actions;
    }

    public virtual AIDecision[] GetAttachedDecisions()
    {
        AIDecision[] decisions = this.gameObject.GetComponentsInChildren<AIDecision>();
        return decisions;
    }

    protected void OnEnable()
    {
        if (ResetCoreOnEnable)
        {
            ResetAICore();
        }
    }

    /// <summary>
    /// On awake we set our brain for all states
    /// </summary>
    protected virtual void Awake()
    {
        foreach (AIState state in States)
        {
            state.SetCore(this);
        }
        decisions = GetAttachedDecisions();
        actions = GetAttachedActions();
        if (RandomizeFrequencies)
        {
            ActionsFrequency = Random.Range(RandomActionFrequency.x, RandomActionFrequency.y);
            DecisionFrequency = Random.Range(RandomDecisionFrequency.x, RandomDecisionFrequency.y);
        }
    }

    /// <summary>
    /// On Start we set our first state
    /// </summary>
    protected virtual void Start()
    {
        if (ResetCoreOnStart)
        {
            ResetAICore();
        }
    }

    /// <summary>
    /// Every frame we update our current state
    /// </summary>
    protected virtual void FixedUpdate()
    {
        if (!CoreActive || (CurrentState == null) || (Time.timeScale == 0f))
        {
            return;
        }

        if (Time.time - lastActionsUpdate > ActionsFrequency)
        {
            CurrentState.PerformActions();
            lastActionsUpdate = Time.time;
        }

        if (!CoreActive)
        {
            return;
        }

        if (Time.time - lastDecisionsUpdate > DecisionFrequency)
        {
            CurrentState.EvaluateTransitions();
            lastDecisionsUpdate = Time.time;
        }

        TimeInThisState += Time.deltaTime;

        StoreLastKnownPosition();
    }

    /// <summary>
    /// Transitions to the specified state, trigger exit and enter states events
    /// </summary>
    /// <param name="newStateName"></param>
    public virtual void TransitionToState(string newStateName)
    {
        if (CurrentState == null)
        {
            CurrentState = FindState(newStateName);
            if (CurrentState != null)
            {
                CurrentState.EnterState();
            }
            return;
        }
        if (newStateName != CurrentState.StateName)
        {
            CurrentState.ExitState();
            OnExitState();

            CurrentState = FindState(newStateName);
            if (CurrentState != null)
            {
                CurrentState.EnterState();
            }
        }
    }

    /// <summary>
    /// When exiting a state we reset our time counter
    /// </summary>
    protected virtual void OnExitState()
    {
        TimeInThisState = 0f;
    }

    /// <summary>
    /// Initializes all decisions
    /// </summary>
    protected virtual void InitialiseDecisions()
    {
        if (decisions == null)
        {
            decisions = GetAttachedDecisions();
        }
        foreach (AIDecision decision in decisions)
        {
            decision.Initialization();
        }
    }

    /// <summary>
    /// Initializes all actions
    /// </summary>
    protected virtual void InitialiseActions()
    {
        if (actions == null)
        {
            actions = GetAttachedActions();
        }
        foreach (AIAction action in actions)
        {
            action.Initialisation();
        }
    }

    /// <summary>
    /// Returns a state based on the specified state name
    /// </summary>
    /// <param name="stateName"></param>
    /// <returns></returns>
    protected AIState FindState(string stateName)
    {
        foreach (AIState state in States)
        {
            if (state.StateName == stateName)
            {
                return state;
            }
        }
        if (stateName != "")
        {
            Debug.LogError("You're trying to transition to state '" + stateName + "' in " + this.gameObject.name + "'s AI Brain, but no state of this name exists. Make sure your states are named properly, and that your transitions states match existing states.");
        }
        return null;
    }

    /// <summary>
    /// Stores the last known position of the target
    /// </summary>
    protected virtual void StoreLastKnownPosition()
    {
        if (Target != null)
        {
            _lastKnownTargetPosition = Target.transform.position;
        }
    }

    /// <summary>
    /// Resets the brain, forcing it to enter its first state
    /// </summary>
    public virtual void ResetAICore()
    {
        InitialiseDecisions();
        InitialiseActions();
        CoreActive = true;
        this.enabled = true;

        if (CurrentState != null)
        {
            CurrentState.ExitState();
            OnExitState();
        }

        if (States.Count > 0)
        {
            CurrentState = States[0];
            CurrentState?.EnterState();
        }
    }
}
