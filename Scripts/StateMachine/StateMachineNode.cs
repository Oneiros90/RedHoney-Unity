﻿using UnityEngine;
using UnityEngine.Events;

namespace RedHoney.StateMachine
{
    ///////////////////////////////////////////////////////////////////////////
    /// This StateMachineBehaviour exposes enter/exit state events as UnityEvents
    [SharedBetweenAnimators]
    public class StateMachineNode : StateMachineBehaviour
    {
        [Tooltip("The event of entering in the state (or sub-state machine)")]
        public UnityEvent OnEnter;

        [Tooltip("The event of exiting from the state (or sub-state machine)")]
        public UnityEvent OnExit;


        private bool inStateMachine;
        private bool inState;
        private bool debugLogs = false;

        ///////////////////////////////////////////////////////////////////////////
        public static void InitializeAll(Animator animator)
        {
            var nodes = animator.GetBehaviours<StateMachineNode>();
            foreach (StateMachineNode node in nodes)
                node.Start();
        }

        ///////////////////////////////////////////////////////////////////////////
        private void Start()
        {
            inStateMachine = false;
            inState = false;
        }

        ///////////////////////////////////////////////////////////////////////////
        // Called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (inStateMachine)
                return;

            if (inState)
                return;

            if (debugLogs)
                Debug.LogFormat("[{0}, {1}] OnStateEnter", GetHashCode(), stateInfo.fullPathHash);

            OnEnter.Invoke();
            inState = true;
        }

        ///////////////////////////////////////////////////////////////////////////
        // Called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (inStateMachine)
                return;

            if (!inState)
                return;

            if (debugLogs)
                Debug.LogFormat("[{0}, {1}] OnStateExit", GetHashCode(), stateInfo.fullPathHash);

            OnExit.Invoke();
            inState = false;
        }

        ///////////////////////////////////////////////////////////////////////////
        // Called on the first Update frame when making a transition to a state machine.
        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            if (inStateMachine)
                return;

            Debug.LogWarning("Warning: You are using a StateMachineNode in a SubStateMachine. You may encounter on unexpected behaviours");

            if (debugLogs)
                Debug.LogFormat("[{0}, {1}] OnStateMachineEnter", GetHashCode(), stateMachinePathHash);

            OnEnter.Invoke();
            inStateMachine = true;
        }

        ///////////////////////////////////////////////////////////////////////////
        // Called on the first Update frame when making a transition from a state machine.
        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            if (!inStateMachine)
                return;

            if (debugLogs)
                Debug.LogFormat("[{0}, {1}] OnStateMachineExit", GetHashCode(), stateMachinePathHash);

            OnExit.Invoke();
            inStateMachine = false;
        }

    }
}