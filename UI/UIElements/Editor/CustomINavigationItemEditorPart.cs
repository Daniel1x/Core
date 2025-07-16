using DL.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CustomINavigationItemEditorPart<T> where T : Selectable, INavigationItem<T>
{
    public T Selectable { get; private set; } = null;

    private SerializedProperty invokeOnAnyItemSelected = null;
    private SerializedProperty allowNavigationToOther = null;
    private SerializedProperty allowNavigationToThis = null;
    private SerializedProperty forceSelectEventOnEnable = null;
    private SerializedProperty forceDeselectEventOnDisable = null;
    private SerializedProperty forceSelectOnPointerEnter = null;
    private SerializedProperty forceRefreshColorTransitionAfterFrame = null;

    private SerializedProperty explicitNavigator = null;
    private SerializedProperty selectionEventController = null;
    private SerializedProperty graphicsTransitionController = null;

    public void OnEnable(SerializedObject _serializedObject, UnityEngine.Object _target)
    {
        Selectable = (T)_target;

        invokeOnAnyItemSelected = _serializedObject.FindProperty("invokeOnAnyItemSelected");
        allowNavigationToOther = _serializedObject.FindProperty("allowNavigationToOtherParent");
        allowNavigationToThis = _serializedObject.FindProperty("allowNavitationToThisObject");
        forceSelectEventOnEnable = _serializedObject.FindProperty("forceSelectEventOnEnable");
        forceDeselectEventOnDisable = _serializedObject.FindProperty("forceDeselectEventOnDisable");
        forceSelectOnPointerEnter = _serializedObject.FindProperty("forceSelectOnPointerEnter");
        forceRefreshColorTransitionAfterFrame = _serializedObject.FindProperty("forceRefreshColorTransitionAfterFrame");

        explicitNavigator = _serializedObject.FindProperty("explicitNavigator");
        selectionEventController = _serializedObject.FindProperty("selectionEventController");
        graphicsTransitionController = _serializedObject.FindProperty("graphicsTransitionController");
    }

    public void OnInspectorGUI(SerializedObject _serializedObject, bool _applyModifiedProperties = false)
    {
        EditorGUILayout.LabelField("Navigation Item Extensions:", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(invokeOnAnyItemSelected);
        EditorGUILayout.PropertyField(allowNavigationToOther);
        EditorGUILayout.PropertyField(allowNavigationToThis);
        EditorGUILayout.PropertyField(forceSelectEventOnEnable);
        EditorGUILayout.PropertyField(forceDeselectEventOnDisable);
        EditorGUILayout.PropertyField(forceSelectOnPointerEnter);
        EditorGUILayout.PropertyField(forceRefreshColorTransitionAfterFrame);

        CustomINavigationItemLayoutUtilities.DrawUtilities(Selectable, _serializedObject);

        EditorGUILayout.Space();

        if (Selectable.navigation.mode is Navigation.Mode.Explicit)
        {
            EditorGUILayout.PropertyField(explicitNavigator);
        }

        EditorGUILayout.PropertyField(selectionEventController);
        EditorGUILayout.PropertyField(graphicsTransitionController);

        if (_applyModifiedProperties)
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}

public static class CustomINavigationItemLayoutUtilities
{
    private static bool showLayoutUtilities = false;
    private static Selectable selectableToCloneTransitionSettingsFrom = null;

    public static void DrawUtilities(Selectable _serializedSelectable, SerializedObject _serializedObject)
    {
        using (new IndentScope())
        {
            showLayoutUtilities = EditorGUILayout.Foldout(showLayoutUtilities, "Layout Utilities", true, EditorStyles.foldoutHeader);

            if (showLayoutUtilities)
            {
                drawTransitionSettingsCloneUtilities(_serializedSelectable, _serializedObject);
            }
        }
    }

    private static void drawTransitionSettingsCloneUtilities(Selectable _serializedSelectable, SerializedObject _serializedObject)
    {
        using (var _check = new ScopeGroup(new ChangeCheckScope(), new IndentScope(), new HorizontalScope()))
        {
            selectableToCloneTransitionSettingsFrom = EditorGUILayout.ObjectField("Clone Transition Settings", selectableToCloneTransitionSettingsFrom, typeof(Selectable), true) as Selectable;
            bool _canPerform = selectableToCloneTransitionSettingsFrom != null && _serializedSelectable != null && selectableToCloneTransitionSettingsFrom != _serializedSelectable;

            using (new DisabledScope(_canPerform))
            {
                if (GUILayout.Button("Clone", GUILayout.Width(60f)) && _canPerform)
                {
                    _serializedSelectable.colors = selectableToCloneTransitionSettingsFrom.colors;
                    _serializedSelectable.SetDirty();
                    _serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
