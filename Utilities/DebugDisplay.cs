using UnityEngine;
using UnityEngine.InputSystem;

public class DebugDisplay : MonoBehaviour
{
    private const int MIN_FPS = 15;
    private const int MAX_FPS = 240;

    [SerializeField] private InputAction action = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/escape");

    [Header("Settings")]
    [SerializeField] private bool vsyncOn = true;
    [SerializeField, Range(15, 240)] private int targetFrameRate = 60;
    [SerializeField, Range(0.1f, 10f)] private float guiScale = 1f;

    private bool isVisible = false;

    private void OnEnable()
    {
        action.Enable();
        action.performed += toggleDebugDisplay;

        QualitySettings.vSyncCount = vsyncOn ? 1 : 0;
        applyTargetFrameRate(targetFrameRate);
    }

    private void OnDisable()
    {
        action.performed -= toggleDebugDisplay;
        action.Disable();
    }

    private void toggleDebugDisplay(InputAction.CallbackContext _)
    {
        isVisible = !isVisible;
    }

    private void applyTargetFrameRate(int _fps)
    {
        targetFrameRate = Mathf.Clamp(_fps, MIN_FPS, MAX_FPS);

        Application.targetFrameRate = QualitySettings.vSyncCount == 0
            ? targetFrameRate
            : -1;
    }

    private void OnGUI()
    {
        if (!isVisible)
        {
            return;
        }

        float _scale = Mathf.Max(0.1f, guiScale);

        int _baseLabelFont = 16;
        float _basePanelWidth = 300f;
        float _baseLineH = 28f;
        float _basePad = 10f;

        int _labelFont = Mathf.RoundToInt(_baseLabelFont * _scale);
        float _panelWidth = _basePanelWidth * _scale;
        float _lineH = _baseLineH * _scale;
        float _pad = _basePad * _scale;

        GUIStyle _labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.UpperLeft,
            padding = new RectOffset(Mathf.RoundToInt(8 * _scale), Mathf.RoundToInt(8 * _scale), Mathf.RoundToInt(6 * _scale), Mathf.RoundToInt(6 * _scale)),
            fontSize = _labelFont,
            wordWrap = false
        };

        _labelStyle.normal.textColor = Color.white;

        GUIStyle _buttonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = Mathf.RoundToInt(_labelFont * 0.9f)
        };

        float _lines = 6f;
        Rect _boxRect = new Rect(Screen.width - _panelWidth - _pad, _pad, _panelWidth, _lines * _lineH + _pad);
        GUI.Box(_boxRect, GUIContent.none);

        float _x = _boxRect.x + (8f * _scale);
        float _y = _boxRect.y + (6f * _scale);
        float _w = _panelWidth - (16f * _scale);

        float _dt = Mathf.Max(1e-5f, Time.unscaledDeltaTime);
        float _fps = 1f / _dt;
        GUI.Label(new Rect(_x, _y, _w, _lineH), $"FPS: {_fps:F1}", _labelStyle);
        _y += _lineH;

        bool _newVsync = GUI.Toggle(new Rect(_x, _y, _w, _lineH), vsyncOn, $"VSync: {(vsyncOn ? "On" : "Off")}", _buttonStyle);

        if (_newVsync != vsyncOn)
        {
            vsyncOn = _newVsync;
            QualitySettings.vSyncCount = vsyncOn ? 1 : 0;
            applyTargetFrameRate(targetFrameRate);
        }

        _y += _lineH;
        GUI.Label(new Rect(_x, _y, _w, _lineH), "Target Frame Rate", _labelStyle);
        _y += _lineH * 0.9f;

        bool _prevEnabled = GUI.enabled;
        GUI.enabled = !vsyncOn;

        float _sliderW = _w - (80f * _scale);
        int _newFps = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(_x, _y, _sliderW, _lineH), targetFrameRate, MIN_FPS, MAX_FPS));

        GUI.Label(new Rect(_x + _sliderW + (8f * _scale), _y, _w - _sliderW - (8f * _scale), _lineH), $"{_newFps} FPS", _labelStyle);

        if (_newFps != targetFrameRate)
        {
            applyTargetFrameRate(_newFps);
        }

        _y += _lineH;
        GUI.enabled = _prevEnabled;
        GUI.Label(new Rect(_x, _y, _w, _lineH), $"Target: {targetFrameRate} FPS", _labelStyle);
        _y += _lineH;

        if (vsyncOn)
        {
            GUIStyle _hint = new GUIStyle(_labelStyle) { fontSize = Mathf.RoundToInt(_labelFont * 0.85f) };
            _hint.normal.textColor = new Color(1f, 0.85f, 0.4f);
            GUI.Label(new Rect(_x, _y, _w, _lineH), "VSync overrides target FPS", _hint);
        }
    }
}
