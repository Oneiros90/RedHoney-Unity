using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace RedHoney.Animator
{
    ///////////////////////////////////////////////////////////////////////////
    /// This component exposes the parameters of an animator
    /// in order to control them from UnityEvents
    public class AnimatorParameter : MonoBehaviour
    {
        [Tooltip("The animator")]
        public UnityEngine.Animator Animator;

        [SerializeField]
        private string parameter;

        [SerializeField]
        private AnimatorControllerParameterType parameterType;


        ///////////////////////////////////////////////////////////////////////////
        public void SetTrigger()
        {
            if (parameterType == AnimatorControllerParameterType.Trigger)
                Animator.SetTrigger(parameter);
        }


        ///////////////////////////////////////////////////////////////////////////
        public void SetBoolValue(bool b)
        {
            if (parameterType == AnimatorControllerParameterType.Trigger && b)
                Animator.SetTrigger(parameter);
            else if (parameterType == AnimatorControllerParameterType.Bool)
                Animator.SetBool(parameter, b);
            else if (parameterType == AnimatorControllerParameterType.Int)
                Animator.SetInteger(parameter, b ? 1 : 0);
            else if (parameterType == AnimatorControllerParameterType.Float)
                Animator.SetFloat(parameter, b ? 1.0f : 0.0f);
        }


        ///////////////////////////////////////////////////////////////////////////
        public void SetIntValue(int i)
        {
            if (parameterType == AnimatorControllerParameterType.Int)
                Animator.SetInteger(parameter, i);
            else if (parameterType == AnimatorControllerParameterType.Float)
                Animator.SetFloat(parameter, i);
            else
                SetBoolValue(i > 0);
        }


        ///////////////////////////////////////////////////////////////////////////
        public void SetFloatValue(float f)
        {
            if (parameterType == AnimatorControllerParameterType.Float)
                Animator.SetFloat(parameter, f);
            else
                SetIntValue((int)f);
        }


        ///////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        [CanEditMultipleObjects]
        [CustomEditor(typeof(AnimatorParameter))]
        public class AnimatorParameterEditor : Editor
        {
            private SerializedProperty animatorProp;
            private SerializedProperty parameterProp;
            private SerializedProperty parameterTypeProp;

            ///////////////////////////////////////////////////////////////////////////
            void OnEnable()
            {
                animatorProp = serializedObject.FindProperty(nameof(Animator));
                parameterProp = serializedObject.FindProperty(nameof(parameter));
                parameterTypeProp = serializedObject.FindProperty(nameof(parameterType));
            }

            ///////////////////////////////////////////////////////////////////////////
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.PropertyField(animatorProp);

                var parameter = target as AnimatorParameter;
                var animator = parameter.Animator;
                if (animator)
                {
                    var controller = animator.runtimeAnimatorController as AnimatorController;
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