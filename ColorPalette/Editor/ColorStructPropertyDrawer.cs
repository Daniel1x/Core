namespace DL.ColorPalette.Editor
{
    using DL.ColorPalette.Structs;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(SingleColor)), CustomPropertyDrawer(typeof(SingleColorHDR))]
    public class ColorStructPropertyDrawer : PropertyDrawer
    {
        private static readonly string[] k_ColorPropertyNames = { "Color1", "Color2", "Color3", "Color4", "Color5", "Color6" };
        private static readonly int k_MaxColorCount = k_ColorPropertyNames.Length;

        private const string k_ColorFieldName = "Color";
        private static readonly StringBuilder sb = new StringBuilder(128);

        protected virtual int colorCount => 1;

        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            EditorGUI.BeginProperty(_position, _label, _property);

            //Draw Label
            float _labelWidth = EditorGUIUtility.labelWidth;
            Rect _labelRect = new Rect(_position.x, _position.y, _labelWidth, _position.height);
            EditorGUI.LabelField(_labelRect, _label);

            //Calculate rect for color fields
            Rect _colorRect = new Rect(_position.x + _labelWidth, _position.y, _position.width - _labelWidth, _position.height);
            float _colorFieldWidth = _colorRect.width / colorCount;

            var _defaultIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            for (int i = 1; i <= colorCount; i++)
            {
                Rect _fieldRect = new Rect(_colorRect.x + (i - 1) * _colorFieldWidth, _colorRect.y, _colorFieldWidth, _colorRect.height);
                drawColorProperty(_property, _fieldRect, i);
            }

            EditorGUI.indentLevel = _defaultIndent;
            EditorGUI.EndProperty();
        }

        protected static void drawColorProperty(SerializedProperty _property, Rect _rect, int _colorID)
        {
            SerializedProperty _colorProp = _property.FindPropertyRelative(getColorPropertyName(_colorID));

            if (_colorProp != null)
            {
                EditorGUI.PropertyField(_rect, _colorProp, GUIContent.none);
            }
        }

        protected static string getColorPropertyName(int _colorID)
        {
            if (_colorID < 1 || _colorID > k_MaxColorCount)
            {
                sb.Clear();
                sb.Append(k_ColorFieldName).Append(_colorID);
                return sb.ToString();
            }

            return k_ColorPropertyNames[_colorID - 1];
        }
    }

    [CustomPropertyDrawer(typeof(ColorPair)), CustomPropertyDrawer(typeof(ColorPairHDR))]
    public class ColorPairStructPropertyDrawer : ColorStructPropertyDrawer { protected override int colorCount => 2; }

    [CustomPropertyDrawer(typeof(ColorTriplet)), CustomPropertyDrawer(typeof(ColorTripletHDR))]
    public class ColorTripletStructPropertyDrawer : ColorStructPropertyDrawer { protected override int colorCount => 3; }

    [CustomPropertyDrawer(typeof(ColorQuartet)), CustomPropertyDrawer(typeof(ColorQuartetHDR))]
    public class ColorQuartetStructPropertyDrawer : ColorStructPropertyDrawer { protected override int colorCount => 4; }

    [CustomPropertyDrawer(typeof(ColorQuintet)), CustomPropertyDrawer(typeof(ColorQuintetHDR))]
    public class ColorQuintetStructPropertyDrawer : ColorStructPropertyDrawer { protected override int colorCount => 5; }

    [CustomPropertyDrawer(typeof(ColorSextet)), CustomPropertyDrawer(typeof(ColorSextetHDR))]
    public class ColorSextetStructPropertyDrawer : ColorStructPropertyDrawer { protected override int colorCount => 6; }
}
