using UnityEngine;
using UnityEngine.Events;

///////////////////////////////////////////////////////////////////////////
/// This StateMachineBehaviour exposes enter/exit state events as UnityEvents
[SharedBetweenAnimators]
public class StateMachineNode : StateMachineBehaviour
{
    [Tooltip("The event of entering in the state (or sub-state machine)")]
    public UnityEvent OnEnter;

    [Tooltip("The event of exiting from the state (or sub-state machine)")]
    public UnityEvent OnExit;


    private StateMachine StateMachine;

    ///////////////////////////////////////////////////////////////////////////
    public void Init(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    ///////////////////////////////////////////////////////////////////////////
    // Called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //name = nameof(StateMachineNode) + stateInfo.fullPathHash.ToString();
        if (animator == StateMachine.Animator)
            StateMachine.TryToEnter(this);
    }

    ///////////////////////////////////////////////////////////////////////////
    // Called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator == StateMachine.Animator)
            StateMachine.TryToExit(this);
    }

}