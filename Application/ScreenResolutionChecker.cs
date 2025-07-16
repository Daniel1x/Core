using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ScreenResolutionChecker
{
    public event UnityAction<ScreenResolution> OnResolutionModified = null;

    public ScreenResolution Resolution { get; private set; } = new ScreenResolution();

    public bool CheckForResolutionChange()
    {
        Resolution _screenResolution = Screen.currentResolution;

        if (Resolution != _screenResolution)
        {
            Resolution = new ScreenResolution(_screenResolution);
            OnResolutionModified?.Invoke(Resolution);
            MyLog.Log($"SCREEN RESOLUTION :: Modified: {Resolution}");

            return true;
        }

        return false;
    }

    public bool CheckForRectChange(RectTransform _rectTransform)
    {
        if (_rectTransform == null)
        {
            return false;
        }

        Vector2 _size = _rectTransform.rect.size;
        _size.Scale(_rectTransform.lossyScale);

        ScreenResolution _pixels = new ScreenResolution(_size.ToNearestInt());

        if (Resolution != _pixels)
        {
            Resolution = _pixels;
            OnResolutionModified?.Invoke(Resolution);
            MyLog.Log($"RECT RESOLUTION :: Rect pixel count modified: {Resolution}");

            return true;
        }

        return false;
    }
}
