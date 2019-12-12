using UnityEngine;

namespace RedHoney.Animator
{
    ///////////////////////////////////////////////////////////////////////////
    /// This StateMachineBehaviour exposes enter/exit state events as UnityEvents
    [SharedBetweenAnimators]
    public class AnimatorStateController : StateMachineBehaviour
    {
        ///////////////////////////////////////////////////////////////////////////
        // Called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var state in AnimatorState.GetStates(animator))
                state.OnStateEnter(this);
        }

        ///////////////////////////////////////////////////////////////////////////
        // Called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (var state in AnimatorState.GetStates(animator))
                state.OnStateExit(this);
        }

    }
}