using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageBlinker : ImageBlinkerBase
{
    [SerializeField] private AnimationCurve blinkCurve = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField] private bool stop = false;
    [SerializeField, Range(0f, 1f)] private float alphaWhenStopped = 0f;

    protected override void updateBlinkState(Image _image, float _time)
    {
        if (blinkCurve == null || blinkCurve.length == 0)
        {
            return;
        }

        Color _color = _image.color;

        if (stop)
        {
            _color.a = alphaWhenStopped;
        }
        else
        {
            _color.a = blinkCurve.Evaluate(_time % blinkCurve[blinkCurve.length - 1].time); ;
        }

        _image.color = _color;
    }
}
