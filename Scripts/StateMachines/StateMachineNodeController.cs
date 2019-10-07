using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

///////////////////////////////////////////////////////////////////////////
/// This MonoBehaviour exposes the events of an animator state as UnityEvents
public class StateMachineNodeController : MonoBehaviour
{
    [Tooltip("The state machine")]
    public StateMachine StateMachine;

    [Tooltip("The event of entering in the state")]
    public UnityEvent OnEnter;

    [Tooltip("The event of exiting from the state")]
    public UnityEvent OnExit;

    [Tooltip("Enable this to have debug logs")]
    public bool DebugLogs = false;

    [SerializeField]
    private StateMachineNode state;


    ///////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        if (state == null)
        {
            Debug.LogWarningFormat("Null state in {0}", name);
            return;
        }
        state.OnEnter.AddListener(OnEnter.Invoke);
        state.OnExit.AddListener(OnExit.Invoke);
        if (DebugLogs)
        {
            state.OnEnter.AddListener(() => Debug.LogFormat("Entering state {0}", name));
            state.OnExit.AddListener(() => Debug.LogFormat("Exiting state {0}", name));
        }
    }

    ///////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StateMachineNodeController))]
    public class StateMachineEventsEditor : Editor
    {
        private SerializedProperty stateMachineProp;
        private SerializedProperty onEnterProp;
        private SerializedProperty onExitProp;
        private SerializedProperty stateProp;
        private SerializedProperty debugLogsProp;

        ///////////////////////////////////////////////////////////////////////////
        void OnEnable()
        {
            stateMachineProp = serializedObject.FindProperty(nameof(StateMachine));
            onEnterProp = serializedObject.FindProperty(nameof(OnEnter));
            onExitProp = serializedObject.FindProperty(nameof(OnExit));
            stateProp = serializedObject.FindProperty(nameof(state));
            debugLogsProp = serializedObject.FindProperty(nameof(DebugLogs));
        }

        ///////////////////////////////////////////////////////////////////////////
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(stateMachineProp);

            var nodeController = target as StateMachineNodeController;
            var stateMachine = nodeController.StateMachine;
            if (stateMachine)
            {
                var controller = stateMachine.Controller as AnimatorController;
                if (controller)
                {
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PrefixLabel(stateProp.displayName);

                        // Retrieve all state machine nodes from the animator
                        var allNodes = StateMachine.FindAllBehaviours<StateMachineNode>(controller);
                        var allNodesNames = allNodes.Keys.ToArray();

                        // Retrieve info about the current serialized node
                        var node = stateProp.objectReferenceValue as StateMachineNode;
                        var state = allNodes.FirstOrDefault(pair => pair.Value == node).Key;
                        var stateIndex = Array.IndexOf(allNodesNames, state);

                        // Show a popup to change the state machine node
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
            EditorGUILayout.PropertyField(debugLogsProp);

            serializedObject.ApplyModifiedProperties();
        }

    }

#endif
}
