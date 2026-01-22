namespace DL.Structs
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            SerializedProperty _relativeMin = _property.FindPropertyRelative("Min");
            SerializedProperty _relativeMax = _property.FindPropertyRelative("Max");

            MinMaxSliderAttribute _attribute = attribute as MinMaxSliderAttribute;

            if (_attribute.LabelOffsetInPixels > 0)
            {
                _position.SplitHorizontalPixels(_attribute.LabelOffsetInPixels, true, out _, out _position);
            }

            MinMax _newMinMax = new MinMax(_relativeMin.floatValue, _relativeMax.floatValue);
            Rect _sliderPos = _position;
            Rect _minFieldPos = _position;
            Rect _maxFieldPos = _position;
            float _fieldWidth = Mathf.Clamp(0.1f * _position.width, 45f, 70f);
            float _valuesStartPos = _position.x + EditorGUIUtility.labelWidth - 14;
            float _labelWidth = Mathf.Abs(_valuesStartPos - _position.x);

            _minFieldPos.x = _valuesStartPos;
            _minFieldPos.width = _fieldWidth;

            EditorGUI.BeginProperty(_position, _label, _property);
            EditorGUI.LabelField(_position, _label);

            EditorGUI.BeginChangeCheck();
            _newMinMax.Min = EditorGUI.FloatField(_minFieldPos, _newMinMax.Min);

            if (EditorGUI.EndChangeCheck())
            {
                _relativeMin.floatValue = _newMinMax.Min;
            }

            float _sliderWidth = _position.width - (2 * _fieldWidth) - _labelWidth;
            _sliderPos.x = _minFieldPos.x + _fieldWidth;
            _sliderPos.width = _sliderWidth;

            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(_sliderPos, ref _newMinMax.Min, ref _newMinMax.Max, _attribute.Min, _attribute.Max);

            if (EditorGUI.EndChangeCheck())
            {
                _relativeMin.floatValue = _newMinMax.Min;
                _relativeMax.floatValue = _newMinMax.Max;
            }

            _maxFieldPos.x = _valuesStartPos + _sliderPos.width + _fieldWidth;
            _maxFieldPos.width = _fieldWidth;

            EditorGUI.BeginChangeCheck();
            _newMinMax.Max = EditorGUI.FloatField(_maxFieldPos, _newMinMax.Max);

            if (EditorGUI.EndChangeCheck())
            {
                _relativeMax.floatValue = _newMinMax.Max;
            }

            _property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
}
