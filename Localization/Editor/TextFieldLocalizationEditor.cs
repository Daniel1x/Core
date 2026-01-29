namespace DL.Localization.Editor
{
    using DL.Localization;
    using UnityEditor;

    [CustomEditor(typeof(TextFieldLocalization))]
    public class TextFieldLocalizationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            bool _valueChanged = DrawDefaultInspector();

            if (_valueChanged)
            {
                (target as TextFieldLocalization).UpdateLocalizedText();
            }
        }
    }
}
