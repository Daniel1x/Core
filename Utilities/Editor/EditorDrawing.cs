namespace DL.Editor
{
    using DL.Enum;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Utility class for creating custom Editors
    /// </summary> 
    public static class EditorDrawing
    {
        public const string LEFT_ARROW = "◄";
        public const string RIGHT_ARROW = "►";
        public const string UP_ARROW = "▲";
        public const string DOWN_ARROW = "▼";
        public const char CORRECT_MARK = '✓';

        public const string INFINITY_STRING = "\u221E";
        public const char INFINITY_CHAR = '\u221E';

        public static readonly string LEFT_ARROW_SLIM = char.ConvertFromUtf32(0x2190);
        public static readonly string UP_ARROW_SLIM = char.ConvertFromUtf32(0x2191);
        public static readonly string RIGHT_ARROW_SLIM = char.ConvertFromUtf32(0x2192);
        public static readonly string DOWN_ARROW_SLIM = char.ConvertFromUtf32(0x2193);
        public static readonly string LEFT_TO_RIGHT_ARROW_SLIM = char.ConvertFromUtf32(0x2194);
        public static readonly string UP_TO_DOWN_ARROW_SLIM = char.ConvertFromUtf32(0x2195);
        public static readonly string UPPER_LEFT_ARROW_SLIM = char.ConvertFromUtf32(0x2196);
        public static readonly string UPPER_RIGHT_ARROW_SLIM = char.ConvertFromUtf32(0x2197);
        public static readonly string LOWER_RIGHT_ARROW_SLIM = char.ConvertFromUtf32(0x2198);
        public static readonly string LOWER_LEFT_ARROW_SLIM = char.ConvertFromUtf32(0x2199);

        public static string GetSlimArrow(bool _right) => _right ? RIGHT_ARROW_SLIM : LEFT_ARROW_SLIM;
        public static string GetArrow(bool _right) => _right ? RIGHT_ARROW : LEFT_ARROW;

        public const string ASSETS = "Assets/";
        public static readonly int MINIMAL_PATH_LENGTH = ASSETS.Length;

        private const float MINI_BUTTON_MIN_WIDTH = 18f;

        public static float LabelWidth { get => EditorGUIUtility.labelWidth; set => EditorGUIUtility.labelWidth = value; }
        public static float FieldWidth { get => EditorGUIUtility.fieldWidth; set => EditorGUIUtility.fieldWidth = value; }
        public static float CurrentViewWidth => EditorGUIUtility.currentViewWidth;
        public static float SingleLineHeight => EditorGUIUtility.singleLineHeight;
        public static float StandardVerticalSpacing => EditorGUIUtility.standardVerticalSpacing;
        public static float LabelWidthWithVerticalSpacing => EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;

        public static readonly Color RED = new Color(255f, 179f, 179f, 255f) / 255f;
        public static readonly Color BLUE = new Color(179f, 236f, 255f, 255f) / 255f;
        public static readonly Color ORANGE = new Color(255f, 191f, 128f, 255f) / 255f;
        public static readonly Color GREEN = new Color(153f, 255f, 187f, 255f) / 255f;
        public static readonly Color BLUE_GREEN = new Color(128f, 255f, 229f, 255f) / 255f;
        public static readonly Color YELLOW = new Color(255f, 255f, 179f, 255f) / 255f;
        public static readonly Color PURPLE = new Color(214f, 153f, 255f, 255f) / 255f;
        public static readonly Color PINK = new Color(255f, 173f, 255f, 255f) / 255f;
        public static readonly Color GRAY = new Color(0.7f, 0.7f, 0.7f, 1f);
        public static readonly Color DARK_GRAY = new Color(0.3f, 0.3f, 0.3f, 1f);
        public static readonly Color WHITE = Color.white;
        public static readonly Color BLACK = Color.black;

        public const int DEFAULT_PROPERTY_HEIGHT = 18;
        public const int DEFAULT_SPACING = 2;

        private const float INT_FIELD_SPACING = 2f;

        public const int MIN_TEXTURE_SIZE = 16;
        public const float SCROLL_VIEW_HEIGHT_SPACING = 12f;

        public static readonly float SINGLE_LINE_HEIGHT = EditorGUIUtility.singleLineHeight;
        public static readonly float VERTICAL_SPACING = EditorGUIUtility.standardVerticalSpacing;
        public static readonly float LINE_HEIGHT_WITH_SPACING = SINGLE_LINE_HEIGHT + VERTICAL_SPACING;

        public static void Space(float _spaceSize)
        {
            GUILayout.Space(_spaceSize);
        }

        public static void FlexibleSpace()
        {
            GUILayout.FlexibleSpace();
        }

        public static void DrawBoldLabel(string _label)
        {
            DrawBoldLabel(_label, GUI.color);
        }

        public static void DrawBoldLabel(string _label, float _width)
        {
            DrawBoldLabel(_label, GetElementWidth(_width));
        }

        public static void DrawBoldLabelWithCenteredText(string _label)
        {
            GUIStyle _centeredLabel = new GUIStyle(EditorStyles.boldLabel);
            _centeredLabel.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label(_label, _centeredLabel);
        }

        public static void DrawBoldLabelWithCenteredText(string _label, float _width)
        {
            GUIStyle _centeredLabel = new GUIStyle(EditorStyles.boldLabel);
            _centeredLabel.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label(_label, _centeredLabel, GetElementWidth(_width));
        }

        public static void DrawBoldLabel(string _label, params GUILayoutOption[] _options)
        {
            GUILayout.Label(_label, EditorStyles.boldLabel, _options);
        }

        public static void DrawBoldLabel(string _label, Color _color)
        {
            using (new GUIColorScope(_color))
            {
                GUILayout.Label(_label, EditorStyles.boldLabel);
            }
        }

        public static bool ScriptProperty(SerializedProperty _property)
        {
            if (_property.displayName == "Script")
            {
                using (new DisabledScope())
                {
                    EditorGUILayout.PropertyField(_property);
                }

                return true;
            }

            return false;
        }

        public static void DrawScriptProperty(this SerializedObject _serializedObject)
        {
            using (new DisabledScope())
            {
                EditorGUILayout.PropertyField(_serializedObject.FindProperty("m_Script"), true);
            }
        }

        public static void IntFieldWithButtons(ref int _value, string _label, params GUILayoutOption[] _options)
        {
            using (new HorizontalScope())
            {
                _value = EditorGUILayout.IntField(_label, _value, _options);

                if (GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false), GUILayout.MinWidth(MINI_BUTTON_MIN_WIDTH)))
                {
                    _value--;
                }
                if (GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false), GUILayout.MinWidth(MINI_BUTTON_MIN_WIDTH)))
                {
                    _value++;
                }
            }
        }

        public static void IntPropertyWithButtons(SerializedProperty _intProperty, params GUILayoutOption[] _options)
        {
            if (_intProperty.propertyType != SerializedPropertyType.Integer)
            {
                MyLog.Warning("EDITOR :: Trying to draw non-integer property as an integer! Aborting");
                return;
            }

            using (new HorizontalScope())
            {
                EditorGUILayout.PropertyField(_intProperty, _options);

                if (GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false), GUILayout.MinWidth(MINI_BUTTON_MIN_WIDTH)))
                {
                    _intProperty.intValue--;
                }
                if (GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false), GUILayout.MinWidth(MINI_BUTTON_MIN_WIDTH)))
                {
                    _intProperty.intValue++;
                }
            }
        }

        public static void DrawDebugRect(Rect _rect, Color _color)
        {
            EditorGUI.DrawRect(_rect, _color);
        }

        public static void DrawEditorLine()
        {
            DrawEditorLine(Color.gray, 3f);
        }

        public static void DrawEditorLine(float _height)
        {
            DrawEditorLine(Color.gray, _height);
        }

        public static void DrawEditorLine(Color _color, float _height = 3f)
        {
            using (new GUIColorScope(_color))
            {
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(_height));
            }
        }

        public static void DrawEditorVerticalLine()
        {
            DrawEditorVerticalLine(Color.gray, 3f);
        }

        public static void DrawEditorVerticalLine(float _width)
        {
            DrawEditorVerticalLine(Color.gray, _width);
        }

        public static void DrawEditorVerticalLine(Color _color, float _width = 3f)
        {
            Color _defaultColor = GUI.color;

            GUI.color = _color;
            GUILayout.Box("", GUILayout.Width(_width), GUILayout.ExpandHeight(true));
            GUI.color = _defaultColor;
        }

        public static void DrawCenteredBoldLabel(string _labelString, Color _color)
        {
            using (new GUIColorScope(_color))
            {
                DrawCenteredBoldLabel(_labelString);
            }
        }

        public static void DrawCenteredBoldLabel(string _labelString, float _fixedWidth)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                FlexibleSpace();

                using (new EditorGUILayout.HorizontalScope(GetElementWidth(_fixedWidth)))
                {
                    DrawCenteredBoldLabel(_labelString);
                }

                FlexibleSpace();
            }
        }

        public static void DrawCenteredBoldLabel(string _labelString)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUIStyle _centered = GetCenteredStyle(EditorStyles.boldLabel);

                FlexibleSpace();
                EditorGUILayout.LabelField(_labelString, _centered);
                FlexibleSpace();
            }
        }

        public static bool DrawToggleBox(bool _value)
        {
            Rect _rect = GUILayoutUtility.GetRect(new GUIContent(), EditorStyles.toggle, GUILayout.ExpandWidth(false));
            return EditorGUI.Toggle(_rect, _value);
        }

        public static void DrawHeader(string _label)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(_label, EditorStyles.boldLabel);
        }

        public static bool DrawHeaderAsFoldout(string _label, bool _open, Color _lineColor, TextAnchor _labelAlignment = TextAnchor.MiddleLeft, float _lineHeight = 3f)
        {
            EditorGUILayout.Space();

            using (new GUILayout.HorizontalScope())
            {
                float _width = (Screen.width / 2f) - 20f;

                GUIStyle _labelStyle = new GUIStyle(EditorStyles.boldLabel);
                _labelStyle.alignment = _labelAlignment;

                EditorGUILayout.LabelField(_label, _labelStyle, GetElementWidth(_width));

                GUIStyle _symbolStyle = new GUIStyle(EditorStyles.label);
                _symbolStyle.alignment = TextAnchor.MiddleRight;

                string _buttonName = _open == true ? UP_ARROW : DOWN_ARROW;

                if (GUILayout.Button(_buttonName, _symbolStyle, GetElementWidth(_width)))
                {
                    return !_open;
                }

                GUILayout.FlexibleSpace();
            }

            DrawEditorLine(_lineColor, _lineHeight);
            return _open;
        }

        public static bool FoldoutWithBackground(bool _foldout, string _label, bool _toggleOnLabelClick = true, bool _drawLine = true, UnityEngine.Object _objectToPreview = null)
        {
            return FoldoutWithBackground(_foldout, _label, Color.gray, _toggleOnLabelClick, _drawLine, _objectToPreview);
        }

        public static bool FoldoutWithBackground(bool _foldout, string _label, Color _backgroundColor, bool _toggleOnLabelClick = true, bool _drawLine = true, UnityEngine.Object _objectToPreview = null)
        {
            using (HorizontalScope _scope = new HorizontalScope(GetElementHeight(20f)))
            {
                float _width = EditorGUIUtility.currentViewWidth;

                using (new GUIBackgroundColorScope(_backgroundColor))
                {
                    Rect _scropRect = _scope.rect;
                    _scropRect.x = 0f;
                    _scropRect.width = _width;

                    GUI.Box(_scropRect, "");
                }

                if (_drawLine)
                {
                    using (new GUIBackgroundColorScope(Color.black))
                    {
                        Rect _lineRect = _scope.rect;
                        _lineRect.x = 0f;
                        _lineRect.width = _width;
                        _lineRect.height = 1f;

                        GUI.Box(_lineRect, "");
                    }
                }

                _foldout = EditorGUILayout.Foldout(_foldout, _label, _toggleOnLabelClick, EditorStyles.foldout);

                if (_objectToPreview != null)
                {
                    EditorGUILayout.ObjectField(_objectToPreview, _objectToPreview.GetType(), false, GetElementWidth(_width / 2f));
                }
            }

            return _foldout;
        }

        public static T EnumPopup<T>(T _selected, T[] _options, params GUILayoutOption[] _layoutOptions) where T : System.Enum
        {
            if (_options == null || _options.Length <= 0)
            {
                EditorGUILayout.Popup(0, new string[] { _selected.ToString() });
                return _selected;
            }

            int _selectedID = _options.Contains(_selected) == false ? 0 : _options.IndexOf(_selected);
            string[] _optionNames = new string[_options.Length];

            for (int i = 0; i < _options.Length; i++)
            {
                _optionNames[i] = _options[i].ToString();
            }

            return _options[EditorGUILayout.Popup(_selectedID, _optionNames, _layoutOptions)];
        }

        public static T EnumPopup<T>(string _label, T _selected, T[] _options, params GUILayoutOption[] _layoutOptions) where T : System.Enum
        {
            if (_options == null || _options.Length <= 0)
            {
                EditorGUILayout.Popup(0, new string[] { _selected.ToString() });
                return _selected;
            }

            int _selectedID = _options.Contains(_selected) == false ? 0 : _options.IndexOf(_selected);
            string[] _optionNames = new string[_options.Length];

            for (int i = 0; i < _options.Length; i++)
            {
                _optionNames[i] = _options[i].ToString();
            }

            return _options[EditorGUILayout.Popup(_label, _selectedID, _optionNames, _layoutOptions)];
        }

        public static string[] DrawMultiSelectField(string[] _selected, string[] _allAvailable)
        {
            return DrawMultiSelectField(null, _selected, _allAvailable);
        }

        public static string[] DrawMultiSelectField(string _label, string[] _selected, string[] _allAvailable)
        {
            int _selectedMask = 0;
            int _allOptionsCount = _allAvailable.Length;

            if (_allOptionsCount == 0)
            {
                if (_label.IsNullEmptyOrWhitespace())
                {
                    EditorGUILayout.MaskField(_selectedMask, new string[] { "-- No Options Available--" });
                }
                else
                {
                    EditorGUILayout.MaskField(_label, _selectedMask, new string[] { "-- No Options Available--" });
                }

                return _selected;
            }

            if (_allOptionsCount > 32)
            {
                MyLog.Log($"EDITOR :: Flag popup can draw max 32 values! Some values will not be drawn! 32/{_allOptionsCount}");
                _allOptionsCount = 32; //Max 32 bits
            }

            for (int i = 0; i < _allOptionsCount; i++)
            {
                if (_selected.Contains(_allAvailable[i]))
                {
                    _selectedMask |= (1 << i);
                }
            }

            int _newSelectedMask = _label.IsNullEmptyOrWhitespace()
                ? EditorGUILayout.MaskField(_selectedMask, _allAvailable)
                : EditorGUILayout.MaskField(_label, _selectedMask, _allAvailable);

            if (_newSelectedMask != _selectedMask)
            {
                List<string> _newSelection = new List<string>();

                for (int i = 0; i < _allOptionsCount; i++)
                {
                    if ((_newSelectedMask & (1 << i)) != 0)
                    {
                        _newSelection.Add(_allAvailable[i]);
                    }
                }

                return _newSelection.ToArray();
            }
            else
            {
                return _selected;
            }
        }

        public static T[] DrawMultiSelectField<T>(string _label, T[] _selected, T[] _allAvailable, System.Func<T, string> _optionCreator)
        {
            if (_optionCreator == null)
            {
                _optionCreator = _e => _e.ToString();
            }

            int _selectedMask = 0;
            int _allOptionsCount = _allAvailable.Length;

            if (_allOptionsCount == 0)
            {
                if (_label.IsNullEmptyOrWhitespace())
                {
                    EditorGUILayout.MaskField(_selectedMask, new string[] { "-- No Options Available--" });
                }
                else
                {
                    EditorGUILayout.MaskField(_label, _selectedMask, new string[] { "-- No Options Available--" });
                }

                return _selected;
            }

            if (_allOptionsCount > 32)
            {
                MyLog.Log($"EDITOR :: Flag popup can draw max 32 values! Some values will not be drawn! 32/{_allOptionsCount}");
                _allOptionsCount = 32; //Max 31 bits + 1 bit as nothing selected
            }

            string[] _options = new string[_allOptionsCount];

            for (int i = 0; i < _allOptionsCount; i++)
            {
                _options[i] = _optionCreator(_allAvailable[i]);

                if (_selected.Contains(_allAvailable[i]))
                {
                    _selectedMask |= (1 << i);
                }
            }

            int _newSelectedMask = _label.IsNullEmptyOrWhitespace()
                ? EditorGUILayout.MaskField(_selectedMask, _options)
                : EditorGUILayout.MaskField(_label, _selectedMask, _options);

            if (_newSelectedMask != _selectedMask)
            {
                List<T> _newSelection = new List<T>();

                for (int i = 0; i < _allOptionsCount; i++)
                {
                    if ((_newSelectedMask & (1 << i)) != 0)
                    {
                        _newSelection.Add(_allAvailable[i]);
                    }
                }

                return _newSelection.ToArray();
            }
            else
            {
                return _selected;
            }
        }

        public static void DrawPropertyWithSingleLineBackground(SerializedProperty _property, float _heightOffset = 0f, params GUILayoutOption[] _layoutOptions)
        {
            DrawPropertyWithSingleLineBackground(_property, BLACK, _heightOffset, _layoutOptions);
        }

        public static void DrawPropertyWithSingleLineBackground(SerializedProperty _property, Color _backgroundColor, float _heightOffset = 0f, params GUILayoutOption[] _layoutOptions)
        {
            using (VerticalScope _scope = new VerticalScope())
            {
                using (new GUIBackgroundColorScope(_backgroundColor))
                {
                    Rect _scopeRect = _scope.rect;
                    _scopeRect.x = 0f;
                    _scopeRect.width = EditorGUIUtility.currentViewWidth;
                    _scopeRect.height = EditorGUIUtility.singleLineHeight;

                    if (_heightOffset != 0f)
                    {
                        _scopeRect.height += _heightOffset;
                        _scopeRect.y -= _heightOffset * 0.5f;
                    }

                    GUI.Box(_scopeRect, "");
                }

                EditorGUILayout.PropertyField(_property, _layoutOptions);
            }
        }

        public static int DrawRadioButtonsWithFlexibleScrollView(ref Vector2 _scrollViewPosition, string _mainLabel, string[] _labels, int _currentSelected, ref int _columns, ref int _lines,
            int _linesMin = 1, int _linesMax = 10, int _columnsMin = 1, int _columnsMax = 10, int _indent = 1, float _boxXOffset = -2, float _boxYOffset = -4, params GUILayoutOption[] _toggleLayoutOptions)
        {
            return DrawRadioButtonsWithFlexibleScrollView(ref _scrollViewPosition, _mainLabel, _labels, _currentSelected, Color.white, ref _columns, ref _lines, _linesMin, _linesMax, _columnsMin, _columnsMax, _indent, _boxXOffset, _boxYOffset, _toggleLayoutOptions);
        }

        public static int DrawRadioButtonsWithFlexibleScrollView(ref Vector2 _scrollViewPosition, string _mainLabel, string[] _labels, int _currentSelected, Color _boxColor, ref int _columns, ref int _lines,
            int _linesMin = 1, int _linesMax = 10, int _columnsMin = 1, int _columnsMax = 10, int _indent = 1, float _boxXOffset = -2, float _boxYOffset = -4, params GUILayoutOption[] _toggleLayoutOptions)
        {
            const float BUTTON_WIDTH = 33.5f;

            using (new HorizontalScope())
            {
                EditorGUILayout.LabelField(_mainLabel);
                _columns = Mathf.Clamp(EditorGUILayout.IntField(_columns, GUILayout.MaxWidth(BUTTON_WIDTH)), _columnsMin, _columnsMax);
                _lines = Mathf.Clamp(EditorGUILayout.IntField(_lines, GUILayout.MaxWidth(BUTTON_WIDTH)), _linesMin, _linesMax);
            }

            return DrawRadioButtonsWithScrollView(ref _scrollViewPosition, _labels, _currentSelected, _boxColor, _lines, _columns, _indent, _boxXOffset, _boxYOffset, _toggleLayoutOptions);

        }

        public static int DrawRadioButtonsWithScrollView(ref Vector2 _scrollViewPosition, string[] _labels, int _currentSelected, int _lines = 10, int _columns = 1, int _indent = 1, float _boxXOffset = -2, float _boxYOffset = -4, params GUILayoutOption[] _toggleLayoutOptions)
        {
            return DrawRadioButtonsWithScrollView(ref _scrollViewPosition, _labels, _currentSelected, Color.white, _lines, _columns, _indent, _boxXOffset, _boxYOffset, _toggleLayoutOptions);
        }

        public static int DrawRadioButtonsWithScrollView(ref Vector2 _scrollViewPosition, string[] _labels, int _currentSelected, Color _boxColor, int _lines = 10, int _columns = 1, int _indent = 1, float _boxXOffset = -2, float _boxYOffset = -4, params GUILayoutOption[] _toggleLayoutOptions)
        {
            int _linesToShow = Mathf.CeilToInt((float)_labels.Length / _columns);

            if (_linesToShow > _lines)
            {
                float _maxScrollViewHeight = _lines * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

                using (EditorGUILayout.ScrollViewScope _scrolView = new EditorGUILayout.ScrollViewScope(_scrollViewPosition, GUILayout.MaxHeight(_maxScrollViewHeight)))
                {
                    _scrollViewPosition = _scrolView.scrollPosition;
                    _currentSelected = DrawRadioButtons(_labels, _currentSelected, _boxColor, _columns, _indent, _boxXOffset, _boxYOffset, _toggleLayoutOptions);
                }
            }
            else
            {
                _currentSelected = DrawRadioButtons(_labels, _currentSelected, _boxColor, _columns, _indent, _boxXOffset, _boxYOffset, _toggleLayoutOptions);
            }

            return _currentSelected;
        }

        public static int DrawRadioButtons(string[] _labels, int _currentSelected, int _columns = 1, int _indent = 1, float _boxXOffset = -2, float _boxYOffset = -4, params GUILayoutOption[] _toggleLayoutOptions)
        {
            return DrawRadioButtons(_labels, _currentSelected, Color.white, _columns, _indent, _boxXOffset, _boxYOffset, _toggleLayoutOptions);
        }

        public static int DrawRadioButtons(string[] _labels, int _currentSelected, Color _boxColor, int _columns = 1, int _indent = 1, float _boxXOffset = -2, float _boxYOffset = -4, params GUILayoutOption[] _toggleLayoutOptions)
        {
            int _itemCount = _labels.Length;
            int _itemsInColumn = Mathf.CeilToInt((float)_itemCount / _columns);

            using (new BoxScope(_boxColor, _boxXOffset, _boxYOffset))
            {
                using (new HorizontalScope(_indent))
                {
                    for (int i = 0; i < _columns; i++)
                    {
                        using (new VerticalScope())
                        {
                            for (int j = 0; j < _itemsInColumn && (i * _itemsInColumn) + j < _itemCount; j++)
                            {
                                int _id = (i * _itemsInColumn) + j;

                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.ToggleLeft(_labels[_id], _currentSelected == _id, _toggleLayoutOptions);

                                if (EditorGUI.EndChangeCheck())
                                {
                                    _currentSelected = _id;
                                }
                            }
                        }
                    }
                }
            }

            return _currentSelected;
        }

        public static bool DrawColoredButton(GUIContent _buttonLabel, Color _guiBackgroundColor, params GUILayoutOption[] _options)
        {
            using (new GUIBackgroundColorScope(_guiBackgroundColor))
            {
                return GUILayout.Button(_buttonLabel, _options);
            }
        }

        public static bool DrawColoredButton(string _buttonLabel, Color _guiBackgroundColor, params GUILayoutOption[] _options)
        {
            using (new GUIBackgroundColorScope(_guiBackgroundColor))
            {
                return GUILayout.Button(_buttonLabel, _options);
            }
        }

        public static bool DrawColoredButton(string _buttonLabel, Color _guiBackgroundColor, float _width)
        {
            using (new GUIBackgroundColorScope(_guiBackgroundColor))
            {
                return GUILayout.Button(_buttonLabel, GetElementWidth(_width));
            }
        }

        public static bool DrawColoredButton(string _buttonLabel, Color _guiBackgroundColor, float _width, GUIStyle _style)
        {
            using (new GUIBackgroundColorScope(_guiBackgroundColor))
            {
                return GUILayout.Button(_buttonLabel, _style, GetElementWidth(_width));
            }
        }

        public static bool DrawCenteredButton(string _buttonLabel, float _width, GUIStyle _style)
        {
            return DrawCenteredButton(_buttonLabel, GUI.backgroundColor, _width, _style);
        }

        public static bool DrawCenteredButton(string _buttonLabel, Color _guiBackgroundColor, float _width)
        {
            using (new GUIBackgroundColorScope(_guiBackgroundColor))
            {
                return DrawCenteredButton(_buttonLabel, _width);
            }
        }

        public static bool DrawCenteredButton(string _buttonLabel, Color _guiBackgroundColor, float _width, GUIStyle _style)
        {
            bool _button = false;

            using (new GUIBackgroundColorScope(_guiBackgroundColor))
            {
                using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
                {
                    FlexibleSpace();
                    _button = GUILayout.Button(_buttonLabel, _style, GetElementWidth(_width));
                    FlexibleSpace();
                }
            }

            return _button;
        }

        public static bool DrawCenteredButton(string _buttonLabel, float _width = 0f)
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                FlexibleSpace();

                GUIStyle _centered = GetCenteredStyle(GUI.skin.button);
                bool _clicked = false;

                if (_width == 0f)
                {
                    _clicked = GUILayout.Button(_buttonLabel, _centered);
                }
                else
                {
                    _clicked = GUILayout.Button(_buttonLabel, _centered, GUILayout.Width(_width));
                }

                FlexibleSpace();

                return _clicked;
            }
        }

        public static GUIStyle GetCenteredBoldLabelStyle()
        {
            return GetCenteredStyle(EditorStyles.boldLabel);
        }

        public static GUIStyle GetCenteredStyle(GUIStyle _other)
        {
            GUIStyle _centered = new GUIStyle(_other);
            _centered.alignment = TextAnchor.MiddleCenter;
            _centered.clipping = TextClipping.Overflow;

            return _centered;
        }

        public static GUIStyle GetCenteredButtonStyle()
        {
            var _style = new GUIStyle(GUI.skin.button);
            _style.alignment = TextAnchor.MiddleCenter;
            _style.clipping = TextClipping.Overflow;
            return _style;
        }

        public static GUIStyle GetBoldFoldoutStyle()
        {
            var _style = new GUIStyle(EditorStyles.foldout);
            _style.fontStyle = FontStyle.Bold;
            return _style;
        }

        public static GUILayoutOption[] GetElementWidth(float _width)
        {
            return new GUILayoutOption[] { GUILayout.MaxWidth(_width), GUILayout.MinWidth(_width) };
        }

        public static GUILayoutOption[] GetElementHeight(float _height)
        {
            return new GUILayoutOption[] { GUILayout.MinHeight(_height), GUILayout.MaxHeight(_height) };
        }

        public static GUILayoutOption[] GetElementWidthAndHeight(float _width, float _height)
        {
            return new GUILayoutOption[] { GUILayout.MaxWidth(_width), GUILayout.MinWidth(_width), GUILayout.MinHeight(_height), GUILayout.MaxHeight(_height) };
        }

        public static void LoseFocus()
        {
            GUI.FocusControl(null);
        }

        public static void DrawPingButton<T>(this T _objectToPing, string _label = "Ping", float _buttonWidth = 40f, bool _select = true) where T : UnityEngine.Object
        {
            using (new DisabledScope(_objectToPing != null))
            {
                if (GUILayout.Button(_label, GetElementWidth(_buttonWidth)))
                {
                    AssetsUtility.PingObject(_objectToPing, _select);
                }
            }
        }

        public static EnumType DrawEnumToolbar<EnumType>(EnumType _currentType, GUIStyle _style = null) where EnumType : System.Enum
        {
            EnumType[] _values = EnumExtensions.GetEnumValues<EnumType>();
            string[] _options = EnumExtensions.GetEnumNames<EnumType>();

            for (int i = 0; i < _options.Length; i++)
            {
                _options[i] = _options[i].InsertSpaceBeforeUpperCaseAndNumeric();
            }

            _currentType = _values[GUILayout.Toolbar(_values.IndexOf(_currentType), _options, _style)];

            return _currentType;
        }

        public static EnumType DrawEnumTab<EnumType>(EnumType _currentValue, float _horizontalRange01 = 1f, GUIStyle _style = null, float _relativeSpacing = 0f) where EnumType : System.Enum
        {
            return DrawEnumTab(_currentValue, BLUE_GREEN, GRAY, _horizontalRange01, _style, _relativeSpacing);
        }

        public static EnumType DrawEnumTab<EnumType>(EnumType _currentValue, Color _selectedColor, Color _defaultColor, float _horizontalRange01 = 1, GUIStyle _style = null, float _relativeSpacing = 0f) where EnumType : System.Enum
        {
            EnumType[] _options = EnumExtensions.GetEnumValues<EnumType>();
            int _count = _options.Length;

            if (_count <= 0)
            {
                return _currentValue;
            }

            _horizontalRange01 = Mathf.Clamp01(_horizontalRange01);

            float _rangeWidth = (CurrentViewWidth - StandardVerticalSpacing) * _horizontalRange01;
            float _emptySpaceWidth = _calculateSpacingSize(out float _widthTakenByEmptySpaces);
            float _singleButtonWidth = (_rangeWidth - _widthTakenByEmptySpaces) / _count;

            GUILayoutOption[] _fixedElementWidth = GetElementWidth(_singleButtonWidth);

            if (_style == null)
            {
                _style = new GUIStyle(EditorStyles.toolbarButton);
                _style.fontStyle = FontStyle.Bold;
                _style.normal.textColor = WHITE;

                _style.fixedWidth = _singleButtonWidth;
            }

            using (new HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                for (int i = 0; i < _count; i++)
                {
                    string _option = _options[i].ToString().InsertSpaceBeforeUpperCaseAndNumeric();
                    Color _itemColor = _options[i].ToInt() == _currentValue.ToInt() ? _selectedColor : _defaultColor;

                    if (_emptySpaceWidth > 0f && i > 0)
                    {
                        EditorGUILayout.Space(_emptySpaceWidth);
                    }

                    using (new GUIBackgroundColorScope(_itemColor))
                    {
                        if (GUILayout.Button(_option, _style, _fixedElementWidth))
                        {
                            _currentValue = _options[i];
                        }
                    }
                }

                GUILayout.FlexibleSpace();
            }

            return _currentValue;

            float _calculateSpacingSize(out float _widthTakenByEmptySpaces)
            {
                if (_count < 2)
                {
                    _widthTakenByEmptySpaces = 0f;
                    return 0f;
                }

                _relativeSpacing = Mathf.Clamp(_relativeSpacing, 0f, 0.5f);
                _widthTakenByEmptySpaces = _rangeWidth * _relativeSpacing;
                float _spaceSize = _widthTakenByEmptySpaces / (_count - 1);

                return _spaceSize.ClampMin(0f);
            }
        }

        public static void DrawDirectoryPath(ref string _directoryPath, string _label = "Directory Path:", float _labelWidth = -1f)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                string _newPath = "";

                if (_labelWidth < 0f)
                {
                    _newPath = EditorGUILayout.TextField(_label, _directoryPath);
                }
                else
                {
                    using (new LabelWidthScope(_labelWidth))
                    {
                        _newPath = EditorGUILayout.TextField(_label, _directoryPath);
                    }
                }

                if (GUILayout.Button("Select", GUILayout.MaxWidth(60f)))
                {
                    _newPath = EditorUtility.SaveFolderPanel(_label, _directoryPath, "");
                    _newPath = AssetsUtility.TrimPathUntilItStartsWithAssetDirectory(_newPath);
                }

                if (_newPath.Length > MINIMAL_PATH_LENGTH && _newPath.StartsWith(ASSETS))
                {
                    _directoryPath = _newPath;
                }
            }
        }

        public static void DrawProperty(ref Rect _position, float _height, SerializedProperty _property, GUIContent _label = null)
        {
            float _originalHeight = _position.height;
            _position.height = _height;

            if (_label == null)
            {
                EditorGUI.PropertyField(_position, _property);
            }
            else
            {
                EditorGUI.PropertyField(_position, _property, _label);
            }

            _position.y += _position.height + EditorGUIUtility.standardVerticalSpacing;
            _position.height = _originalHeight;
        }

        public static bool DrawFoldout(ref Rect _position, float _height, bool _foldout, GUIContent _label)
        {
            float _originalHeight = _position.height;
            _position.height = _height;

            _foldout = EditorGUI.Foldout(_position, _foldout, _label);

            _position.y += _position.height + EditorGUIUtility.standardVerticalSpacing;
            _position.height = _originalHeight;

            return _foldout;
        }

        public static void DrawLabel(ref Rect _position, float _height, string _label, GUIStyle _style = null)
        {
            DrawLabel(ref _position, _height, new GUIContent(_label), _style);
        }

        public static void DrawLabel(ref Rect _position, float _height, GUIContent _label, GUIStyle _style = null)
        {
            float _originalHeight = _position.height;
            _position.height = _height;

            if (_style == null)
            {
                EditorGUI.LabelField(_position, _label);
            }
            else
            {
                EditorGUI.LabelField(_position, _label, _style);
            }

            _position.y += _position.height + EditorGUIUtility.standardVerticalSpacing;
            _position.height = _originalHeight;
        }

        public static void DrawBox(ref Rect _position, in Vector2 _boxPadding)
        {
            EditorGUIUtility.labelWidth -= _boxPadding.x;

            EditorGUI.HelpBox(EditorGUI.IndentedRect(_position), "", MessageType.None);

            _position.x += _boxPadding.x;
            _position.width -= _boxPadding.x * 2f;
            _position.y += _boxPadding.y;
        }

        public static void DrawIntFieldWithButtons(ref Rect _position, float _height, ref int _value, string _label = null)
        {
            float _originalHeight = _position.height;
            float _originalWidth = _position.width;

            _position.height = _height;
            _position.width -= MINI_BUTTON_MIN_WIDTH * 2f + INT_FIELD_SPACING;

            Rect _buttonPosition = new Rect(_position)
            {
                width = MINI_BUTTON_MIN_WIDTH,
                x = _position.x + _position.width + INT_FIELD_SPACING,
            };

            if (_label == null)
            {
                _value = EditorGUI.IntField(_position, _value);
            }
            else
            {
                _value = EditorGUI.IntField(_position, _label, _value);
            }

            if (GUI.Button(_buttonPosition, "-", EditorStyles.miniButtonLeft))
            {
                _value--;
            }

            _buttonPosition.x += _buttonPosition.width;

            if (GUI.Button(_buttonPosition, "+", EditorStyles.miniButtonRight))
            {
                _value++;
            }

            _position.y += _position.height + EditorGUIUtility.standardVerticalSpacing;
            _position.height = _originalHeight;
            _position.width = _originalWidth;
        }

        public static void ApplyModifiedProperties(this SerializedProperty _object)
        {
            _object.serializedObject.ApplyModifiedProperties();
        }

        public static void DrawArrayProperty(SerializedProperty _arrayProperty, ref Rect _propertyPosition, ref int _propertyCounter, float _propertyHeight = DEFAULT_PROPERTY_HEIGHT, float _spacing = DEFAULT_SPACING)
        {
            if (_arrayProperty == null)
            {
                return;
            }

            int _lineCount = CountLinesNeededToDrawProperty(_arrayProperty);
            float _heightPerProperty = _propertyHeight + _spacing;
            _propertyPosition.y += _heightPerProperty;
            Rect _arrayPropertyRect = _propertyPosition;

            _arrayPropertyRect.height = _lineCount * _heightPerProperty;
            _propertyCounter += _lineCount;

            EditorGUI.BeginProperty(_arrayPropertyRect, null, _arrayProperty);
            EditorGUI.PropertyField(_arrayPropertyRect, _arrayProperty, true);
            EditorGUI.EndProperty();

            _propertyPosition.y += _arrayPropertyRect.height - _propertyPosition.height;
        }

        public static int CountLinesNeededToDrawProperty(SerializedProperty _property)
        {
            if (_property.isExpanded == false)
            {
                return 1;
            }

            int _lineCount = 2;

            if (_property.isArray == false)
            {
                SerializedProperty[] _childProperties = _property.GetChildProperties();

                for (int i = 0; i < _childProperties.Length; i++)
                {
                    _lineCount += CountLinesNeededToDrawProperty(_childProperties[i]);
                }

                return _lineCount;
            }

            _lineCount++;
            int _arraySize = _property.arraySize;

            for (int i = 0; i < _arraySize; i++)
            {
                _lineCount += CountLinesNeededToDrawProperty(_property.GetArrayElementAtIndex(i));
            }

            if (_arraySize == 0)
            {
                _lineCount++;
            }

            return _lineCount;
        }

        public static SerializedProperty[] GetChildProperties(this SerializedProperty _property)
        {
            List<SerializedProperty> _children = new List<SerializedProperty>();

            _property.IterateOverChildProperties((_childProperty) =>
            {
                _children.Add(_childProperty);
            });

            return _children.ToArray();
        }

        public static void IterateOverChildProperties(this SerializedProperty _property, Action<SerializedProperty> _action, bool _skipScriptField = false)
        {
            using (SerializedProperty _iterator = _property.Copy())
            {
                if (_iterator.NextVisible(true) == false)
                {
                    return;
                }

                if (_skipScriptField)
                {
                    if (_iterator.NextVisible(false) == false)
                    {
                        return;
                    }
                }

                do
                {
                    SerializedProperty _childProperty = _property.FindPropertyRelative(_iterator.name);

                    if (_childProperty == null)
                    {
                        break;
                    }

                    _action.Invoke(_childProperty);
                }
                while (_iterator.NextVisible(false));
            }
        }
    }
}
