using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GraphicsTransitionController
{
    /// <summary>
    /// Same names and values as the private UnityEngine.UI.Selectable.SelectionState enum.
    /// </summary>
    public enum SelectionState
    {
        Normal = 0,
        Highlighted = 1,
        Pressed = 2,
        Selected = 3,
        Disabled = 4
    }

    [SerializeField] private bool cloneColorTransitionToOthers = false;
    [SerializeField] private Graphic[] graphicsToCloneTransition = new Graphic[] { };

    private bool canPerformTransition = false;
    private Selectable selectable = null;

    public void SetSelectable(Selectable _selectable)
    {
        selectable = _selectable;
        canPerformTransition = true;
    }

    public void DoStateTransition(SelectionState _state, bool _instant, bool _ignoreTimeScale = true, bool _useAlpha = false)
    {
        if (canPerformTransition == false
            || cloneColorTransitionToOthers == false
            || graphicsToCloneTransition.Length == 0
            || selectable == null)
        {
            return;
        }

        Color _newColor = GetStateColor(selectable.colors, _state);
        float _fadeDuration = _instant ? 0f : selectable.colors.fadeDuration;

        foreach (Graphic _graphic in graphicsToCloneTransition)
        {
            if (_graphic != null)
            {
                _graphic.CrossFadeColor(_newColor, _fadeDuration, _ignoreTimeScale, _useAlpha);
            }
        }
    }

    public static Color GetStateColor(ColorBlock _colorBlock, SelectionState _state)
    {
        return _state switch
        {
            SelectionState.Normal => _colorBlock.normalColor,
            SelectionState.Highlighted => _colorBlock.highlightedColor,
            SelectionState.Pressed => _colorBlock.pressedColor,
            SelectionState.Selected => _colorBlock.selectedColor,
            SelectionState.Disabled => _colorBlock.disabledColor,
            _ => _colorBlock.normalColor,
        };
    }
}
