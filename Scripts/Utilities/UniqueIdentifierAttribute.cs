using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RedHoney.Utilities
{
    ///////////////////////////////////////////////////////////////////////////
    public class UniqueIdentifierAttribute : PropertyAttribute { }


    ///////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UniqueIdentifierAttribute))]
    public class UniqueIdentifierDrawer : PropertyDrawer
    {
        ///////////////////////////////////////////////////////////////////////////
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            string assetPath = AssetDatabase.GetAssetPath(prop.serializedObject.targetObject.GetInstanceID());
            string uniqueId = AssetDatabase.AssetPathToGUID(assetPath);

            prop.stringValue = uniqueId;

            Rect textFieldPosition = position;
            textFieldPosition.height = 16;
            DrawLabelField(textFieldPosition, prop, label);
        }

        ///////////////////////////////////////////////////////////////////////////
        void DrawLabelField(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.LabelField(position, label, new GUIContent(prop.stringValue));
        }
    }
#endif
}