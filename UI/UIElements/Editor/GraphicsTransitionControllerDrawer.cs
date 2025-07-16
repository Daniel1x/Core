using DL.Editor;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GraphicsTransitionController))]
public class GraphicsTransitionControllerDrawer : PropertyDrawer
{
    private const int SPACE_ABOVE = 6;
    private const int PROPERTY_HEIGHT = EditorDrawing.DEFAULT_PROPERTY_HEIGHT;
    private const int SPACING = EditorDrawing.DEFAULT_SPACING;
    private const int SPACE_UNDER = 6;

    private int propertyCount = 0;

    private SerializedProperty cloneColorTransitionProperty = null;
    private SerializedProperty graphicsToCloneTransitionProperty = null;

    public override void OnGUI(Rect _mainPosition, SerializedProperty _mainProperty, GUIContent _label)
    {
        propertyCount = 0;

        Rect _singlePropertyPosition = _mainPosition;
        _singlePropertyPosition.height = PROPERTY_HEIGHT;
        _singlePropertyPosition.y -= PROPERTY_HEIGHT - SPACE_ABOVE;

        _singlePropertyPosition.y += PROPERTY_HEIGHT + SPACING;
        EditorGUI.LabelField(_singlePropertyPosition, "Graphic Transition Controller:", EditorStyles.boldLabel);
        propertyCount++;

        _singlePropertyPosition.y += PROPERTY_HEIGHT + SPACING;
        cloneColorTransitionProperty = _mainProperty.FindPropertyRelative("cloneColorTransitionToOthers");
        EditorGUI.PropertyField(_singlePropertyPosition, cloneColorTransitionProperty);
        propertyCount++;

        if (cloneColorTransitionProperty.boolValue)
        {
            using (new IndentScope())
            {
                graphicsToCloneTransitionProperty = _mainProperty.FindPropertyRelative("graphicsToCloneTransition");
                EditorDrawing.DrawArrayProperty(graphicsToCloneTransitionProperty, ref _singlePropertyPosition, ref propertyCount);
            }
        }

        _mainProperty.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        return SPACE_ABOVE + SPACING + ((PROPERTY_HEIGHT + SPACING) * propertyCount) + SPACE_UNDER;
    }
}
