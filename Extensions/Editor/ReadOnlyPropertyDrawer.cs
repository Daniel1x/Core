namespace DL.Structs
{
    using UnityEditor;
    using UnityEngine;
    using static UnityEditor.EditorGUI;

    [CustomPropertyDrawer(typeof(ReadOnlyProperty))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            return EditorGUI.GetPropertyHeight(_property, _label, true);
        }

        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            using (new DisabledScope(attribute is ReadOnlyProperty _readOnly && _readOnly.DisableScope))
            {
                EditorGUI.PropertyField(_position, _property, _label, true);
            }
        }
    }
}
