using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace RedHoney.StateMachine
{
    ///////////////////////////////////////////////////////////////////////////
    /// This component exposes the parameters of a state machine
    /// in order to control them from UnityEvents
    [CreateAssetMenu(fileName = "StateMachineParameter", menuName = "State Machine/Parameter", order = 1)]
    public class StateMachineParameter : ScriptableObject
    {
        [Tooltip("The state machine")]
        public StateMachine StateMachine;

        [SerializeField]
        private string parameter;

        [SerializeField]
        private AnimatorControllerParameterType parameterType;


        ///////////////////////////////////////////////////////////////////////////
        public void SetTrigger()
        {
            if (parameterType == AnimatorControllerParameterType.Trigger)
                StateMachine.Animator.SetTrigger(parameter);
        }


        ///////////////////////////////////////////////////////////////////////////
        public void SetBoolValue(bool b)
        {
            if (parameterType == AnimatorControllerParameterType.Trigger && b)
                StateMachine.Animator.SetTrigger(parameter);
            else if (parameterType == AnimatorControllerParameterType.Bool)
                StateMachine.Animator.SetBool(parameter, b);
            else if (parameterType == AnimatorControllerParameterType.Int)
                StateMachine.Animator.SetInteger(parameter, b ? 1 : 0);
            else if (parameterType == AnimatorControllerParameterType.Float)
                StateMachine.Animator.SetFloat(parameter, b ? 1.0f : 0.0f);
        }


        ///////////////////////////////////////////////////////////////////////////
        public void SetIntValue(int i)
        {
            if (parameterType == AnimatorControllerParameterType.Int)
                StateMachine.Animator.SetInteger(parameter, i);
            else if (parameterType == AnimatorControllerParameterType.Float)
                StateMachine.Animator.SetFloat(parameter, i);
            else
                SetBoolValue(i > 0);
        }


        ///////////////////////////////////////////////////////////////////////////
        public void SetFloatValue(float f)
        {
            if (parameterType == AnimatorControllerParameterType.Float)
                StateMachine.Animator.SetFloat(parameter, f);
            else
                SetIntValue((int)f);
        }


        ///////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        [CanEditMultipleObjects]
        [CustomEditor(typeof(StateMachineParameter))]
        public class StateMachineParameterEditor : Editor
        {
            private SerializedProperty stateMachineProp;
            private SerializedProperty parameterProp;
            private SerializedProperty parameterTypeProp;

            ///////////////////////////////////////////////////////////////////////////
            void OnEnable()
            {
                stateMachineProp = serializedObject.FindProperty(nameof(StateMachine));
                parameterProp = serializedObject.FindProperty(nameof(parameter));
                parameterTypeProp = serializedObject.FindProperty(nameof(parameterType));
            }

            ///////////////////////////////////////////////////////////////////////////
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(stateMachineProp);

                var parameter = target as StateMachineParameter;
                var stateMachine = parameter.StateMachine;
                if (stateMachine)
                {
                    var controller = stateMachine.Controller as AnimatorController;
                    if (controller)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.PrefixLabel(parameterProp.displayName);

                            // Retrieve all parameters from the animator
                            var parameters = controller.parameters;
                            var parametersNames = parameters.Select(p => p.name).ToArray();

                            // Retrieve info about the current parameter
                            var parameterName = parameterProp.stringValue;
                            var parameterIndex = Array.IndexOf(parametersNames, parameterName);

                            // Show a popup to change the parameter
                            int newParameterIndex = EditorGUILayout.Popup(parameterIndex, parametersNames);
                            if (newParameterIndex != parameterIndex)
                            {
                                parameterName = parametersNames[newParameterIndex];
                                parameterProp.stringValue = parametersNames[newParameterIndex];
                                parameterTypeProp.intValue = (int)parameters[newParameterIndex].type;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }

        }

#endif
    }
}