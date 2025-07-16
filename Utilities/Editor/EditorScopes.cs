namespace DL.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public abstract class GUIPropertyScopeBase<T> : GUI.Scope
    {
        protected T defaultValue = default;

        protected abstract T currentValue { get; set; }

        public GUIPropertyScopeBase(T _newValue)
        {
            defaultValue = currentValue;
            currentValue = _newValue;
        }

        protected override void CloseScope()
        {
            currentValue = defaultValue;
        }
    }

    public abstract class GUIColorScopeBase : GUIPropertyScopeBase<Color>
    {
        public GUIColorScopeBase(Color _newColor) : base(_newColor) { }
    }

    public class GUIColorScope : GUIColorScopeBase
    {
        protected override Color currentValue { get => GUI.color; set => GUI.color = value; }
        public GUIColorScope(Color _newColor) : base(_newColor) { }
    }

    public class GUIContentColorScope : GUIColorScopeBase
    {
        protected override Color currentValue { get => GUI.contentColor; set => GUI.contentColor = value; }
        public GUIContentColorScope(Color _newColor) : base(_newColor) { }
    }

    public class GUIBackgroundColorScope : GUIColorScopeBase
    {
        protected override Color currentValue { get => GUI.backgroundColor; set => GUI.backgroundColor = value; }
        public GUIBackgroundColorScope(Color _newColor) : base(_newColor) { }
    }

    public class IndentScope : GUI.Scope
    {
        public static IndentScope NoIndent => new IndentScope(-EditorGUI.indentLevel);

        private int defaultIndent = 0;

        public IndentScope(int _relativeIndent = 1, bool _useAsNotRelative = false)
        {
            defaultIndent = EditorGUI.indentLevel;

            EditorGUI.indentLevel = _useAsNotRelative
                ? _relativeIndent
                : defaultIndent + _relativeIndent;
        }

        protected override void CloseScope()
        {
            EditorGUI.indentLevel = defaultIndent;
        }
    }

    public class LabelWidthScope : GUIPropertyScopeBase<float>
    {
        protected override float currentValue { get => EditorGUIUtility.labelWidth; set => EditorGUIUtility.labelWidth = value; }
        public LabelWidthScope(float _labelWidth) : base(_labelWidth) { }
    }

    public class FieldWidthScope : GUIPropertyScopeBase<float>
    {
        protected override float currentValue { get => EditorGUIUtility.fieldWidth; set => EditorGUIUtility.fieldWidth = value; }
        public FieldWidthScope(float _fieldWidth) : base(_fieldWidth) { }
    }

    public class DisabledScope : GUIPropertyScopeBase<bool>
    {
        protected override bool currentValue { get => GUI.enabled; set => GUI.enabled = value; }
        public DisabledScope(bool _enabled = false) : base(_enabled) { }
    }

    public class HorizontalScope : EditorGUILayout.HorizontalScope
    {
        private int defaultIndent = 0;
        private float defaultLabelWidth = 0;
        private bool defaultGUIEnabled = true;

        public HorizontalScope(GUIStyle _style) : base(_style)
        {
            defaultIndent = EditorGUI.indentLevel;
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultGUIEnabled = GUI.enabled;
        }

        public HorizontalScope(params GUILayoutOption[] _layoutOptions) : base(_layoutOptions)
        {
            defaultIndent = EditorGUI.indentLevel;
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultGUIEnabled = GUI.enabled;
        }

        public HorizontalScope(GUIStyle _style, params GUILayoutOption[] _layoutOptions) : base(_style, _layoutOptions)
        {
            defaultIndent = EditorGUI.indentLevel;
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultGUIEnabled = GUI.enabled;
        }

        public HorizontalScope() : base()
        {
            defaultIndent = EditorGUI.indentLevel;
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultGUIEnabled = GUI.enabled;
        }

        public HorizontalScope(int _relativeIndent) : this()
        {
            EditorGUI.indentLevel = defaultIndent + _relativeIndent;
        }

        public HorizontalScope(float _labelWidth) : this()
        {
            EditorGUIUtility.labelWidth = _labelWidth;
        }

        public HorizontalScope(bool _disabled) : this()
        {
            GUI.enabled = !_disabled;
        }

        public HorizontalScope(int _relativeIndent, float _labelWidth) : this()
        {
            EditorGUI.indentLevel = defaultIndent + _relativeIndent;
            EditorGUIUtility.labelWidth = _labelWidth;
        }

        public HorizontalScope(float _labelWidth, bool _disabled) : this()
        {
            EditorGUIUtility.labelWidth = _labelWidth;
            GUI.enabled = !_disabled;
        }

        public HorizontalScope(int _relativeIndent, bool _disabled) : this()
        {
            EditorGUI.indentLevel = defaultIndent + _relativeIndent;
            GUI.enabled = !_disabled;
        }

        public HorizontalScope(int _relativeIndent, float _labelWidth, bool _disabled) : this()
        {
            EditorGUI.indentLevel = defaultIndent + _relativeIndent;
            EditorGUIUtility.labelWidth = _labelWidth;
            GUI.enabled = !_disabled;
        }

        protected override void CloseScope()
        {
            base.CloseScope();
            EditorGUI.indentLevel = defaultIndent;
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            GUI.enabled = defaultGUIEnabled;
        }
    }

    public class VerticalScope : EditorGUILayout.VerticalScope
    {
        private int defaultIndent = 0;
        private float defaultLabelWidth = 0;
        private bool defaultGUIEnabled = true;

        public VerticalScope(GUIStyle _style) : base(_style)
        {
            defaultIndent = EditorGUI.indentLevel;
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultGUIEnabled = GUI.enabled;
        }

        public VerticalScope(params GUILayoutOption[] _layoutOptions) : base(_layoutOptions)
        {
            defaultIndent = EditorGUI.indentLevel;
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultGUIEnabled = GUI.enabled;
        }

        public VerticalScope(GUIStyle _style, params GUILayoutOption[] _layoutOptions) : base(_style, _layoutOptions)
        {
            defaultIndent = EditorGUI.indentLevel;
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultGUIEnabled = GUI.enabled;
        }

        public VerticalScope() : base()
        {
            defaultIndent = EditorGUI.indentLevel;
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultGUIEnabled = GUI.enabled;
        }

        public VerticalScope(int _relativeIndent) : this()
        {
            EditorGUI.indentLevel = defaultIndent + _relativeIndent;
        }

        public VerticalScope(float _labelWidth) : this()
        {
            EditorGUIUtility.labelWidth = _labelWidth;
        }

        public VerticalScope(bool _disabled) : this()
        {
            GUI.enabled = !_disabled;
        }

        public VerticalScope(int _relativeIndent, float _labelWidth) : this()
        {
            EditorGUI.indentLevel = defaultIndent + _relativeIndent;
            EditorGUIUtility.labelWidth = _labelWidth;
        }

        public VerticalScope(float _labelWidth, bool _disabled) : this()
        {
            EditorGUIUtility.labelWidth = _labelWidth;
            GUI.enabled = !_disabled;
        }

        public VerticalScope(int _relativeIndent, bool _disabled) : this()
        {
            EditorGUI.indentLevel = defaultIndent + _relativeIndent;
            GUI.enabled = !_disabled;
        }

        public VerticalScope(int _relativeIndent, float _labelWidth, bool _disabled) : this()
        {
            EditorGUI.indentLevel = defaultIndent + _relativeIndent;
            EditorGUIUtility.labelWidth = _labelWidth;
            GUI.enabled = !_disabled;
        }

        protected override void CloseScope()
        {
            base.CloseScope();
            EditorGUI.indentLevel = defaultIndent;
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            GUI.enabled = defaultGUIEnabled;
        }
    }

    public class BoxScope : EditorGUILayout.VerticalScope
    {
        private Color defaultBackgroundColor = default;
        private Rect boxRect = default;

        public Rect BoxRect => boxRect;

        public BoxScope(Color _color, float _horizontalOffset = 0f, float _verticalOffset = 0f) : base()
        {
            boxRect = rect;
            boxRect.x += _horizontalOffset;
            boxRect.width -= 2 * _horizontalOffset;
            boxRect.y += _verticalOffset;
            boxRect.height -= 2 * _verticalOffset;

            defaultBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = _color;
            GUI.Box(boxRect, "");
            GUI.backgroundColor = defaultBackgroundColor;
        }

        public BoxScope(float _horizontalOffset = 0f, float _verticalOffset = 0f) : this(Color.black, _horizontalOffset, _verticalOffset) { }
    }

    public class MixedValueScope : GUI.Scope
    {
        private bool defaultShowMixedValues = default;

        public MixedValueScope(bool _showMixedValue = true)
        {
            defaultShowMixedValues = EditorGUI.showMixedValue;
            EditorGUI.showMixedValue = _showMixedValue;
        }

        protected override void CloseScope()
        {
            EditorGUI.showMixedValue = defaultShowMixedValues;
        }
    }

    public class ChangeCheckScope : EditorGUI.ChangeCheckScope
    {
        public static implicit operator bool(ChangeCheckScope _scope) => _scope.changed;
    }

    public class RotateRectScope : GUI.Scope
    {
        private Matrix4x4 guiMatrix = default;

        public RotateRectScope(float _angle, Rect _rect)
        {
            guiMatrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(_angle, _rect.center);
        }

        protected override void CloseScope()
        {
            GUI.matrix = guiMatrix;
        }
    }

    public class ScopeGroup : GUI.Scope
    {
        public static ScopeGroup HorizontalCheck => new ScopeGroup(new ChangeCheckScope(), new HorizontalScope());
        public static ScopeGroup VerticalCheck => new ScopeGroup(new ChangeCheckScope(), new VerticalScope());

        private GUI.Scope[] scopes = null;
        private List<EditorGUI.ChangeCheckScope> changeCheckScopes = new List<EditorGUI.ChangeCheckScope>();

        public bool Changed
        {
            get
            {
                for (int i = 0; i < changeCheckScopes.Count; i++)
                {
                    if (changeCheckScopes[i].changed)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public ScopeGroup(params GUI.Scope[] _scopes)
        {
            scopes = _scopes;

            if (_scopes != null)
            {
                for (int i = 0; i < _scopes.Length; i++)
                {
                    if (_scopes[i] is EditorGUI.ChangeCheckScope _checkScope)
                    {
                        changeCheckScopes.Add(_checkScope);
                    }
                }
            }
        }

        protected override void CloseScope()
        {
            if (scopes != null)
            {
                for (int i = scopes.Length - 1; i >= 0; i--) //Have to be disposed in reversed order
                {
                    scopes[i].Dispose();
                }
            }
        }

        public static implicit operator bool(ScopeGroup _scope) => _scope.Changed;
    }

    public class ScrollViewScope : GUILayout.ScrollViewScope
    {
        public ScrollViewScope(Vector2 _scrollPosition, params GUILayoutOption[] _options) : base(_scrollPosition, _options) { }
        public ScrollViewScope(Vector2 _scrollPosition, GUIStyle _style, params GUILayoutOption[] _options) : base(_scrollPosition, _style, _options) { }
        public ScrollViewScope(Vector2 _scrollPosition, bool _alwaysShowHorizontal, bool _alwaysShowVertical, params GUILayoutOption[] _options) : base(_scrollPosition, _alwaysShowHorizontal, _alwaysShowVertical, _options) { }
        public ScrollViewScope(Vector2 _scrollPosition, GUIStyle _horizontalScrollbar, GUIStyle _verticalScrollbar, params GUILayoutOption[] _options) : base(_scrollPosition, _horizontalScrollbar, _verticalScrollbar, _options) { }
        public ScrollViewScope(Vector2 _scrollPosition, bool _alwaysShowHorizontal, bool _alwaysShowVertical, GUIStyle _horizontalScrollbar, GUIStyle _verticalScrollbar, params GUILayoutOption[] _options) : base(_scrollPosition, _alwaysShowHorizontal, _alwaysShowVertical, _horizontalScrollbar, _verticalScrollbar, _options) { }
        public ScrollViewScope(Vector2 _scrollPosition, bool _alwaysShowHorizontal, bool _alwaysShowVertical, GUIStyle _horizontalScrollbar, GUIStyle _verticalScrollbar, GUIStyle _background, params GUILayoutOption[] _options) : base(_scrollPosition, _alwaysShowHorizontal, _alwaysShowVertical, _horizontalScrollbar, _verticalScrollbar, _background, _options) { }
    }
}
