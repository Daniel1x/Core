using UnityEngine;
using UnityEngine.InputSystem;

public class DebugDisplay : MonoBehaviour
{
    // Toggle menu (default: Escape)
    [SerializeField] private InputAction action = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/escape");

    [Header("Settings")]
    [SerializeField] private bool vsyncOn = true;
    [SerializeField, Range(15, 240)] private int targetFrameRate = 60;
    [SerializeField, Range(0.1f, 10f)] private float guiScale = 1f;

    private const int MinFps = 15;
    private const int MaxFps = 240;

    private bool isVisible = false;

    private void OnEnable()
    {
        action.Enable();
        action.performed += toggleDebugDisplay;

        QualitySettings.vSyncCount = vsyncOn ? 1 : 0;
        ApplyTargetFrameRate(targetFrameRate);
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

    private void ApplyTargetFrameRate(int fps)
    {
        targetFrameRate = Mathf.Clamp(fps, MinFps, MaxFps);
        if (QualitySettings.vSyncCount == 0)
        {
            Application.targetFrameRate = targetFrameRate;
        }
        else
        {
            // Przy w³¹czonym VSync Application.targetFrameRate bywa ignorowane
            Application.targetFrameRate = -1;
        }
    }

    private void OnGUI()
    {
        if (!isVisible) return;

        // Skala GUI
        float s = Mathf.Max(0.1f, guiScale);

        int baseLabelFont = 16;
        float basePanelWidth = 300f;
        float baseLineH = 28f;
        float basePad = 10f;

        int labelFont = Mathf.RoundToInt(baseLabelFont * s);
        float panelWidth = basePanelWidth * s;
        float lineH = baseLineH * s;
        float pad = basePad * s;

        // Style
        var labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.UpperLeft, // lewy górny róg (¿eby tekst by³ pewnie widoczny)
            padding = new RectOffset(Mathf.RoundToInt(8 * s), Mathf.RoundToInt(8 * s), Mathf.RoundToInt(6 * s), Mathf.RoundToInt(6 * s)),
            fontSize = labelFont,
            wordWrap = false
        };
        labelStyle.normal.textColor = Color.white;

        var buttonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = Mathf.RoundToInt(labelFont * 0.9f)
        };

        // Pude³ko
        // 6 linii: FPS, VSync toggle, "Target Frame Rate", slider, "Target:", hint (opcjonalnie)
        float lines = 6f;
        Rect boxRect = new Rect(Screen.width - panelWidth - pad, pad, panelWidth, lines * lineH + pad);
        GUI.Box(boxRect, GUIContent.none);

        // Pozycje
        float x = boxRect.x + (8f * s);
        float y = boxRect.y + (6f * s);
        float w = panelWidth - (16f * s);

        // FPS
        float dt = Mathf.Max(1e-5f, Time.unscaledDeltaTime);
        float fps = 1f / dt;
        GUI.Label(new Rect(x, y, w, lineH), $"FPS: {fps:F1}", labelStyle);
        y += lineH;

        // VSync toggle
        bool newVsync = GUI.Toggle(new Rect(x, y, w, lineH), vsyncOn, $"VSync: {(vsyncOn ? "On" : "Off")}", buttonStyle);
        if (newVsync != vsyncOn)
        {
            vsyncOn = newVsync;
            QualitySettings.vSyncCount = vsyncOn ? 1 : 0;
            ApplyTargetFrameRate(targetFrameRate);
        }
        y += lineH;

        // Nag³ówek
        GUI.Label(new Rect(x, y, w, lineH), "Target Frame Rate", labelStyle);
        y += lineH * 0.9f;

        // Slider (bez przycisków +/-) + wartoœæ po prawej
        bool prevEnabled = GUI.enabled;
        GUI.enabled = !vsyncOn;

        float sliderW = w - (80f * s);
        int newFps = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(x, y, sliderW, lineH), targetFrameRate, MinFps, MaxFps));

        // Wyœwietl wartoœæ targetu
        GUI.Label(new Rect(x + sliderW + (8f * s), y, w - sliderW - (8f * s), lineH), $"{newFps} FPS", labelStyle);

        if (newFps != targetFrameRate)
        {
            ApplyTargetFrameRate(newFps);
        }
        y += lineH;

        GUI.enabled = prevEnabled;

        // Bie¿¹cy target (zawsze pokazuj)
        GUI.Label(new Rect(x, y, w, lineH), $"Target: {targetFrameRate} FPS", labelStyle);
        y += lineH;

        // Hint
        if (vsyncOn)
        {
            var hint = new GUIStyle(labelStyle) { fontSize = Mathf.RoundToInt(labelFont * 0.85f) };
            hint.normal.textColor = new Color(1f, 0.85f, 0.4f);
            GUI.Label(new Rect(x, y, w, lineH), "VSync overrides target FPS", hint);
        }
    }
}
