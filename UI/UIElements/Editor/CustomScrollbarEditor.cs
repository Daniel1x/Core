using DL.Editor;
using UnityEditor;
using UnityEditor.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(CustomScrollbar))]
public class CustomScrollbarEditor : ScrollbarEditor
{
    private CustomINavigationItemEditorPart<CustomScrollbar> customNavigationItemEditorPart = new();

    protected override void OnEnable()
    {
        base.OnEnable();
        customNavigationItemEditorPart.OnEnable(serializedObject, target);
    }

    public override void OnInspectorGUI()
    {
        EditorDrawing.DrawScriptProperty(serializedObject);
        base.OnInspectorGUI();
        customNavigationItemEditorPart.OnInspectorGUI(serializedObject);
        serializedObject.ApplyModifiedProperties();
    }
}
