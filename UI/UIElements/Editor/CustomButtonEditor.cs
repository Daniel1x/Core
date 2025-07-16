using DL.Editor;
using UnityEditor;
using UnityEditor.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(CustomButton), true)]
public class CustomButtonEditor : ButtonEditor
{
    private SerializedProperty forceClickOnPointerDown = null;
    private SerializedProperty fadeStyle = null;
    private SerializedProperty graphicsToFade = null;
    private CustomINavigationItemEditorPart<CustomButton> customNavigationItemEditorPart = new();

    protected override void OnEnable()
    {
        base.OnEnable();
        customNavigationItemEditorPart.OnEnable(serializedObject, target);
        fadeStyle = serializedObject.FindProperty("fadeStyle");
        graphicsToFade = serializedObject.FindProperty("graphicsToFade");
        forceClickOnPointerDown = serializedObject.FindProperty("forceClickOnPointerDown");
    }

    public override void OnInspectorGUI()
    {
        EditorDrawing.DrawScriptProperty(serializedObject);
        base.OnInspectorGUI();
        customNavigationItemEditorPart.OnInspectorGUI(serializedObject);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Button Extensions:", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(forceClickOnPointerDown);
        EditorGUILayout.PropertyField(fadeStyle);

        if (fadeStyle.intValue != (int)CustomButton.GraphicsFadeStyles.None)
        {
            EditorGUILayout.PropertyField(graphicsToFade);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
