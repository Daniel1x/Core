using DL.Editor;
using DL.Enum;
using UnityEditor;
using UnityEngine;
using static SelectionEventController;

[CustomPropertyDrawer(typeof(SelectionEventController))]
public class SelectionEventControllerDrawer : PropertyDrawer
{
    private const int SPACE_ABOVE = 6;
    private const int PROPERTY_HEIGHT = EditorDrawing.DEFAULT_PROPERTY_HEIGHT;
    private const int SPACING = EditorDrawing.DEFAULT_SPACING;
    private const int SPACE_UNDER = 6;

    private int propertyCount = 0;

    public override void OnGUI(Rect _mainPosition, SerializedProperty _mainProperty, GUIContent _label)
    {
        propertyCount = 0;

        Rect _singlePropertyPosition = _mainPosition;
        _singlePropertyPosition.height = PROPERTY_HEIGHT;
        _singlePropertyPosition.y -= PROPERTY_HEIGHT - SPACE_ABOVE;

        _singlePropertyPosition.y += PROPERTY_HEIGHT + SPACING;
        EditorGUI.LabelField(_singlePropertyPosition, _label.text + ":", EditorStyles.boldLabel);
        propertyCount++;

        if (_mainProperty.FindPropertyRelative("selectionEventMode") is SerializedProperty _selectionEventModeProperty)
        {
            _singlePropertyPosition.y += SPACING + PROPERTY_HEIGHT;

            try
            {
                EditorGUI.PropertyField(_singlePropertyPosition, _selectionEventModeProperty);
            }
            catch (System.Exception)
            {
                return;
            }

            propertyCount++;

            SelectionEventMode _mode = (SelectionEventMode)_selectionEventModeProperty.enumValueFlag;

            if (_mode != SelectionEventMode.None)
            {
                using (new IndentScope())
                {
                    if (_mode.ContainsFlag(SelectionEventMode.ObjectActive)
                        && _mainProperty.FindPropertyRelative("objectsToEnableOnSelect") is SerializedProperty _objectsToEnableOnSelectProperty)
                    {
                        EditorDrawing.DrawArrayProperty(_objectsToEnableOnSelectProperty, ref _singlePropertyPosition, ref propertyCount);
                    }

                    if (_mode.ContainsFlag(SelectionEventMode.BehaviourEnabled)
                        && _mainProperty.FindPropertyRelative("behavioursToEnableOnSelect") is SerializedProperty _behavioursToEnableOnSelectProperty)
                    {
                        EditorDrawing.DrawArrayProperty(_behavioursToEnableOnSelectProperty, ref _singlePropertyPosition, ref propertyCount);
                    }

                    if (_mode.ContainsFlag(SelectionEventMode.ObjectDisabled)
                        && _mainProperty.FindPropertyRelative("objectsToDisableOnSelect") is SerializedProperty _objectsToDisableOnSelectProperty)
                    {
                        EditorDrawing.DrawArrayProperty(_objectsToDisableOnSelectProperty, ref _singlePropertyPosition, ref propertyCount);
                    }

                    if (_mode.ContainsFlag(SelectionEventMode.BehaviourDisabled)
                        && _mainProperty.FindPropertyRelative("behavioursToDisableOnSelect") is SerializedProperty _behavioursToDisableOnSelectProperty)
                    {
                        EditorDrawing.DrawArrayProperty(_behavioursToDisableOnSelectProperty, ref _singlePropertyPosition, ref propertyCount);
                    }
                }
            }
        }

        _mainProperty.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        return SPACE_ABOVE + SPACING + ((PROPERTY_HEIGHT + SPACING) * propertyCount) + SPACE_UNDER;
    }
}
