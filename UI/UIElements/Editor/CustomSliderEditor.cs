using DL.Editor;
using UnityEditor;
using UnityEditor.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(CustomSlider), true)]
public class CustomSliderEditor : SliderEditor
{
    private CustomINavigationItemEditorPart<CustomSlider> customNavigationItemEditorPart = new();

    private SerializedProperty sliderStepSizeProperty = null;
    private SerializedProperty updateTextFieldProperty = null;
    private SerializedProperty addPercentageSignProperty = null;
    private SerializedProperty addSecondsSignProperty = null;
    private SerializedProperty valueTMProTextFieldProperty = null;
    private SerializedProperty displayDecimalProperty = null;
    private SerializedProperty decimalPlacesProperty = null;

    protected override void OnEnable()
    {
        base.OnEnable();
        customNavigationItemEditorPart.OnEnable(serializedObject, target);

        sliderStepSizeProperty = serializedObject.FindProperty("sliderStepSize");
        updateTextFieldProperty = serializedObject.FindProperty("updateTextOnValueChange");
        addPercentageSignProperty = serializedObject.FindProperty("addPercentageSign");
        addSecondsSignProperty = serializedObject.FindProperty("addSecondsSign");
        valueTMProTextFieldProperty = serializedObject.FindProperty("valueTMProTextField");
        displayDecimalProperty = serializedObject.FindProperty("displayDecimal");
        decimalPlacesProperty = serializedObject.FindProperty("decimalPlaces");
    }

    public override void OnInspectorGUI()
    {
        EditorDrawing.DrawScriptProperty(serializedObject);
        base.OnInspectorGUI();
        customNavigationItemEditorPart.OnInspectorGUI(serializedObject);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Slider Extensions:", EditorStyles.boldLabel);
        EditorGUILayout.Slider(sliderStepSizeProperty, 0.01f, 100f);
        EditorGUILayout.PropertyField(displayDecimalProperty);

        if (displayDecimalProperty.boolValue)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.IntSlider(decimalPlacesProperty, 0, 5);
            EditorGUI.indentLevel = 0;
        }

        EditorGUILayout.PropertyField(updateTextFieldProperty);

        if (updateTextFieldProperty.boolValue)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(valueTMProTextFieldProperty);
            EditorGUILayout.PropertyField(addPercentageSignProperty);
            EditorGUILayout.PropertyField(addSecondsSignProperty);
            EditorGUI.indentLevel = 0;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
