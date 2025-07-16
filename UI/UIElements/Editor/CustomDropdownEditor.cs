using DL.Editor;
using TMPro.EditorUtilities;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(CustomDropdown))]
public class CustomDropdownEditor : DropdownEditor
{
    protected SerializedProperty forceSelectOnDropdownCloseProperty = null;
    private CustomINavigationItemEditorPart<CustomDropdown> customNavigationItemEditorPart = new();

    protected override void OnEnable()
    {
        base.OnEnable();

        customNavigationItemEditorPart.OnEnable(serializedObject, target);
        forceSelectOnDropdownCloseProperty = serializedObject.FindProperty("forceSelectOnDropdownClose");
    }

    public override void OnInspectorGUI()
    {
        EditorDrawing.DrawScriptProperty(serializedObject);
        base.OnInspectorGUI();
        customNavigationItemEditorPart.OnInspectorGUI(serializedObject);
        EditorGUILayout.LabelField("Dropdown Extensions:", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(forceSelectOnDropdownCloseProperty);
        serializedObject.ApplyModifiedProperties();
    }
}
