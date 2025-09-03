using DL.Structs;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationExtensions
{
    public static void OptimizeAnimationClip(this AnimationClip _clip, float _valueRangeThreshold = 0.01f)
    {
#if UNITY_EDITOR
        if (_clip == null)
        {
            Debug.LogError("AnimationClip is null");
            return;
        }

        UnityEditor.EditorCurveBinding[] _bindings = UnityEditor.AnimationUtility.GetCurveBindings(_clip);
        List<Keyframe> _optimizedKeys = new List<Keyframe>();
        int _numberOfRemovedKeys = 0;

        foreach (var _binding in _bindings)
        {
            AnimationCurve _curve = UnityEditor.AnimationUtility.GetEditorCurve(_clip, _binding);

            if (_curve == null || _curve.keys.Length < 2)
            {
                continue;
            }

            _optimizedKeys.Clear();
            _optimizedKeys.Add(_curve.keys[0]);

            MinMax _minMax = _curve.GetValueRange();
            float _threshold = Mathf.Max(_valueRangeThreshold * _minMax.Range, 0.0001f);

            int _redundantCount = 0;
            int _redundantWithThresholdCount = 0;
            int _tangentRedundantCount = 0;

            for (int i = 1; i < _curve.keys.Length - 1; i++)
            {
                Keyframe _prevKey = _curve.keys[i - 1];
                Keyframe _currKey = _curve.keys[i];
                Keyframe _nextKey = _curve.keys[i + 1];

                if (IsRedundant(_prevKey, _currKey, _nextKey))
                {
                    _redundantCount++;
                    continue;
                }

                if (IsRedundant(_prevKey, _currKey, _nextKey, _threshold))
                {
                    _redundantWithThresholdCount++;
                    continue;
                }

                //Check if tangents are pointing to the middle key, if so it is redundant
                if (Mathf.Approximately(_prevKey.outTangent, _currKey.inTangent) &&
                    Mathf.Approximately(_currKey.inTangent, _nextKey.inTangent) &&
                    Mathf.Approximately(_prevKey.outTangent, _nextKey.inTangent))
                {
                    _tangentRedundantCount++;
                    continue;
                }

                _optimizedKeys.Add(_currKey);
            }

            _optimizedKeys.Add(_curve.keys[_curve.keys.Length - 1]);

            if (_optimizedKeys.Count == _curve.keys.Length)
            {
                continue;
            }

            _numberOfRemovedKeys += _curve.keys.Length - _optimizedKeys.Count;
            AnimationCurve _optimizedCurve = new AnimationCurve(_optimizedKeys.ToArray());
            UnityEditor.AnimationUtility.SetEditorCurve(_clip, _binding, _optimizedCurve);
        }

        Debug.Log($"Optimized AnimationClip: {_clip.name} - Removed {_numberOfRemovedKeys} redundant keyframes.");
        UnityEditor.EditorUtility.SetDirty(_clip);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    public static MinMax GetValueRange(this AnimationCurve _curve)
    {
        if (_curve == null || _curve.keys.Length == 0)
        {
            return new MinMax(0f, 0f);
        }

        float _min = float.MaxValue;
        float _max = float.MinValue;

        foreach (var _key in _curve.keys)
        {
            if (_key.value < _min) _min = _key.value;
            if (_key.value > _max) _max = _key.value;
        }

        return new MinMax(_min, _max);
    }

    public static bool IsRedundant(Keyframe _prev, Keyframe _curr, Keyframe _next, float _threshold)
    {
        float _t = (_curr.time - _prev.time) / (_next.time - _prev.time);
        float _interpolatedValue = Mathf.Lerp(_prev.value, _next.value, _t);
        return Mathf.Abs(_interpolatedValue - _curr.value) <= _threshold;
    }

    public static bool IsRedundant(Keyframe _prev, Keyframe _curr, Keyframe _next)
    {
        float _t = (_curr.time - _prev.time) / (_next.time - _prev.time);
        float _interpolatedValue = Mathf.Lerp(_prev.value, _next.value, _t);
        return Mathf.Approximately(_interpolatedValue, _curr.value);
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Optimize Animation Clip", priority = 2000)]
    private static void optimizeSelectedClips()
    {
        foreach (Object _obj in UnityEditor.Selection.objects)
        {
            if (_obj is AnimationClip _clip)
            {
                _clip.OptimizeAnimationClip();
            }
        }
    }

    [UnityEditor.MenuItem("Assets/Optimize Animation Clip", validate = true)]
    private static bool optimizeSelectedClips_Validate()
    {
        return UnityEditor.Selection.objects != null
            && System.Array.Exists(UnityEditor.Selection.objects, _object => _object is AnimationClip);
    }
#endif
}
