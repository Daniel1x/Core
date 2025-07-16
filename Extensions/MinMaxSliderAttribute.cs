using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MinMaxSliderAttribute : PropertyAttribute
{
    public readonly float Min;
    public readonly float Max;
    public readonly int LabelOffsetInPixels;

    public MinMaxSliderAttribute(float _min, float _max, int _labelOffsetInPixels = 0)
    {
        Min = _min;
        Max = _max;
        LabelOffsetInPixels = _labelOffsetInPixels;
    }
}
