using DL.Editor;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(CustomToggle))]
public class CustomToggleEditor : ToggleEditor
{
    private SerializedProperty onObjectProperty = null;
    private SerializedProperty offObjectProperty = null;
    private CustomINavigationItemEditorPart<CustomToggle> customNavigationItemEditorPart = new();

    protected override void OnEnable()
    {
        base.OnEnable();
        customNavigationItemEditorPart.OnEnable(serializedObject, target);
        onObjectProperty = serializedObject.FindProperty("onObject");
        offObjectProperty = serializedObject.FindProperty("offObject");
    }

    public override void OnInspectorGUI()
    {
        EditorDrawing.DrawScriptProperty(serializedObject);

        //Forcing instant transitions of check marks! Fade is breaking cloned transitions of graphicsTransitionController.
        if (customNavigationItemEditorPart.Selectable.toggleTransition is Toggle.ToggleTransition.Fade)
        {
            customNavigationItemEditorPart.Selectable.toggleTransition = Toggle.ToggleTransition.None;
        }

        base.OnInspectorGUI();

        customNavigationItemEditorPart.OnInspectorGUI(serializedObject);

        EditorGUILayout.LabelField("Custom Toggle:", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(onObjectProperty);
        EditorGUILayout.PropertyField(offObjectProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
