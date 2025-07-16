using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MinMaxIntSliderAttribute : PropertyAttribute
{
    public readonly int Min;
    public readonly int Max;
    public readonly int LabelOffsetInPixels;

    public MinMaxIntSliderAttribute(int _min, int _max, int _labelOffsetInPixels = 0)
    {
        Min = _min;
        Max = _max;
        LabelOffsetInPixels = _labelOffsetInPixels;
    }
}
