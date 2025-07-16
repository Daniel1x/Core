namespace DL.Structs
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(MinMaxIntSliderAttribute))]
    public class MinMaxIntSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            SerializedProperty _relativeMin = _property.FindPropertyRelative("Min");
            SerializedProperty _relativeMax = _property.FindPropertyRelative("Max");

            MinMaxIntSliderAttribute _attribute = attribute as MinMaxIntSliderAttribute;

            if (_attribute.LabelOffsetInPixels > 0)
            {
                _position.SplitHorizontalPixels(_attribute.LabelOffsetInPixels, true, out _, out _position);
            }

            MinMaxInt _newMinMax = new MinMaxInt(_relativeMin.intValue, _relativeMax.intValue);
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
            _newMinMax.Min = EditorGUI.IntField(_minFieldPos, _newMinMax.Min);

            if (EditorGUI.EndChangeCheck())
            {
                _relativeMin.intValue = _newMinMax.Min;
            }

            float _sliderWidth = _position.width - (2 * _fieldWidth) - _labelWidth;
            _sliderPos.x = _minFieldPos.x + _fieldWidth;
            _sliderPos.width = _sliderWidth;

            float _refMin = (float)_newMinMax.Min;
            float _refMax = (float)_newMinMax.Max;

            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(_sliderPos, ref _refMin, ref _refMax, _attribute.Min, _attribute.Max);

            _newMinMax.Min = Mathf.RoundToInt(_refMin);
            _newMinMax.Max = Mathf.RoundToInt(_refMax);

            if (EditorGUI.EndChangeCheck())
            {
                _relativeMin.intValue = _newMinMax.Min;
                _relativeMax.intValue = _newMinMax.Max;
            }

            _maxFieldPos.x = _valuesStartPos + _sliderPos.width + _fieldWidth;
            _maxFieldPos.width = _fieldWidth;

            EditorGUI.BeginChangeCheck();
            _newMinMax.Max = EditorGUI.IntField(_maxFieldPos, _newMinMax.Max);

            if (EditorGUI.EndChangeCheck())
            {
                _relativeMax.intValue = _newMinMax.Max;
            }

            _property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }
    }
}
