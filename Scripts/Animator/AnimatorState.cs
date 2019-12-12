using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace RedHoney.Animator
{
    ///////////////////////////////////////////////////////////////////////////
    /// This MonoBehaviour exposes the events of an animator state as UnityEvents
    public class AnimatorState : MonoBehaviour
    {
        private static Dictionary<UnityEngine.Animator, List<AnimatorState>> StatesDict = new Dictionary<UnityEngine.Animator, List<AnimatorState>>();

        private static List<AnimatorState> EmptyList = new List<AnimatorState>();

        public static IReadOnlyList<AnimatorState> GetStates(UnityEngine.Animator animator)
        {
            StatesDict.TryGetValue(animator, out List<AnimatorState> states);
            if (states == null)
                states = EmptyList;
            return states;
        }


        [SerializeField]
        [Tooltip("The animator")]
        private UnityEngine.Animator Animator;

        [SerializeField]
        [Tooltip("The event of entering in the state")]
        private UnityEvent OnEnter;

        [SerializeField]
        [Tooltip("The event of exiting from the state")]
        private UnityEvent OnExit;

        [SerializeField]
        [Tooltip("The state controller")]
        private AnimatorStateController stateController;

        ///////////////////////////////////////////////////////////////////////////
        private void Start()
        {
            StatesDict.TryGetValue(Animator, out List<AnimatorState> list);
            if (list == null)
            {
                list = new List<AnimatorState>();
                StatesDict.Add(Animator, list);
            }

            list.Add(this);
        }

        ///////////////////////////////////////////////////////////////////////////
        public void OnStateEnter(AnimatorStateController controller)
        {
            if (stateController == controller)
                OnEnter.Invoke();
        }

        ///////////////////////////////////////////////////////////////////////////
        public void OnStateExit(AnimatorStateController controller)
        {
            if (stateController == controller)
                OnExit.Invoke();
        }

        ///////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        [CanEditMultipleObjects]
        [CustomEditor(typeof(AnimatorState))]
        public class StateMachineEventsEditor : Editor
        {
            private SerializedProperty animatorProp;
            private SerializedProperty onEnterProp;
            private SerializedProperty onExitProp;
            private SerializedProperty stateProp;

            ///////////////////////////////////////////////////////////////////////////
            void OnEnable()
            {
                animatorProp = serializedObject.FindProperty(nameof(Animator));
                onEnterProp = serializedObject.FindProperty(nameof(OnEnter));
                onExitProp = serializedObject.FindProperty(nameof(OnExit));
                stateProp = serializedObject.FindProperty(nameof(stateController));
            }

            ///////////////////////////////////////////////////////////////////////////
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(animatorProp);

                var animatorState = target as AnimatorState;
                var animator = animatorState.Animator;
                if (animator)
                {
                    var controller = animator.runtimeAnimatorController as AnimatorController;
                    if (controller)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.PrefixLabel(stateProp.displayName);

                            // Retrieve all animator nodes from the animator
                            var allNodes = FindAllBehaviours<AnimatorStateController>(animator.runtimeAnimatorController);
                            var allNodesNames = allNodes.Keys.ToArray();

                            // Retrieve info about the current serialized node
                            var node = stateProp.objectReferenceValue as AnimatorStateController;
                            var state = allNodes.FirstOrDefault(pair => pair.Value == node).Key;
                            var stateIndex = Array.IndexOf(allNodesNames, state);

                            // Show a popup to change the animator node
                            int newStateIndex = EditorGUILayout.Popup(stateIndex, allNodesNames);
                            if (newStateIndex != stateIndex)
                            {
                                state = allNodesNames[newStateIndex];
                                stateProp.objectReferenceValue = allNodes[state];
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.PropertyField(onEnterProp);
                EditorGUILayout.PropertyField(onExitProp);

                serializedObject.ApplyModifiedProperties();
            }

            private static SortedDictionary<string, T> FindAllBehaviours<T>(RuntimeAnimatorController runtimeController) where T : StateMachineBehaviour
            {
                var behaviours = new SortedDictionary<string, T>();

                AnimatorController controller = runtimeController as AnimatorController;

                if (controller != null)
                {
                    // This utility local function fills the behaviours dictionary
                    void lookForBehaviour(StateMachineBehaviour bhv, params string[] nameComponents)
                    {
                        if (bhv != null && bhv is T)
                        {
                            string behaviourName = string.Join(".", nameComponents.Where(c => c.Length > 0).ToArray());
                            behaviours.Add(behaviourName, bhv as T);
                        }
                    }

                    // This utility local function deep-scans all possible behaviours in an animator
                    void scanStateMachine(string layerName, AnimatorStateMachine stateMachine)
                    {
                        // Scan behaviours on the animator itself (only for sub-state machines)
                        foreach (var behaviour in stateMachine.behaviours)
                            lookForBehaviour(behaviour, layerName, stateMachine.name + " (sub-state)");

                        // Scan each state
                        foreach (ChildAnimatorState state in stateMachine.states)
                            foreach (var behaviour in state.state.behaviours)
                                lookForBehaviour(behaviour, layerName, stateMachine.name, state.state.name);

                        // Scan each sub-state machines
                        foreach (var subStateMachine in stateMachine.stateMachines)
                            scanStateMachine(stateMachine.name, subStateMachine.stateMachine);
                    }

                    // Start a recursive scan from each layer of the controller
                    foreach (AnimatorControllerLayer layer in controller.layers)
                        scanStateMachine("", layer.stateMachine);
                }
                return behaviours;
            }
        }

#endif
    }
}