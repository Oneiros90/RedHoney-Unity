using UnityEngine;

namespace RedHoney.StateMachine.Utilities
{
    public class IncrementParameter : StateMachineBehaviour
    {
        [Tooltip("")]
        public string ParameterName;

        [Tooltip("")]
        public AnimatorControllerParameterType ParameterType = AnimatorControllerParameterType.Int;

        [Tooltip("")]
        public float Increment;

        public enum Transition { OnEnter, OnExit };
        [Tooltip("")]
        public Transition TriggeringEvent;


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (TriggeringEvent == Transition.OnEnter)
                incrementParam(animator);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (TriggeringEvent == Transition.OnExit)
                incrementParam(animator);
        }

        // Actual parameter increment
        private void incrementParam(Animator animator)
        {
            if (ParameterType == AnimatorControllerParameterType.Float)
            {
                float oldValue = animator.GetFloat(ParameterName);
                animator.SetFloat(ParameterName, oldValue + Increment);
            }
            else if (ParameterType == AnimatorControllerParameterType.Int)
            {
                int oldValue = animator.GetInteger(ParameterName);
                animator.SetInteger(ParameterName, oldValue + Mathf.RoundToInt(Increment));
            }
        }

        // Parameters validation
        private void OnValidate()
        {
            if (ParameterType == AnimatorControllerParameterType.Int)
            {
                Increment = Mathf.RoundToInt(Increment);
            }
        }
    }
}