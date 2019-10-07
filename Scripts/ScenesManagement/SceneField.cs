using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RedHoney.ScenesManagement
{
    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Useful field to avoid using strings when referencing scenes in the inspector
    /// </summary>
    [System.Serializable]
    public class SceneField
    {

        [SerializeField]
        private Object _sceneAsset;


        [SerializeField]
        private string _name = "";
        public string Name => _name;


        public SceneField(string name)
        {
            _name = name;
        }

        // makes it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string(SceneField sceneField) => sceneField.Name;
        public static implicit operator SceneField(string name) => new SceneField(name);

        public override bool Equals(object obj) => obj is SceneField scene2 ? Name.Equals(scene2.Name) : false;

        public override int GetHashCode() => Name.GetHashCode();


        ///////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        /// <summary>
        /// Custom property drawer for the SceneField
        /// </summary>
        [CustomPropertyDrawer(typeof(SceneField))]
        public class SceneFieldPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, GUIContent.none, property);
                SerializedProperty sceneAsset = property.FindPropertyRelative(nameof(_sceneAsset));
                SerializedProperty sceneName = property.FindPropertyRelative(nameof(_name));
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                if (sceneAsset != null && !property.serializedObject.isEditingMultipleObjects)
                {
                    sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
                    if (sceneAsset.objectReferenceValue != null)
                    {
                        sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
                    }
                }
                EditorGUI.EndProperty();
            }
        }
#endif
    }
}