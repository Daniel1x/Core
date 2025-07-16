using DL;
using System.Collections;
using UnityEngine;

public enum LogMode
{
    Always = 0,
    EditorOnly = 1,
    BuildOnly = 2,
    DevelopmentBuild = 3,
    ShipmentBuild = 4,
};

public static class MyLog
{
    public static IEnumerator DebugBreak(string _message, Color _textColor = default, LogMode _logMode = LogMode.Always, bool _condition = true, Object _context = null)
    {
        if (_condition == false || _logMode.isValidLogMode() == false)
        {
            yield break;
        }

        Log("DEBUG BREAK :: " + _message, _textColor, _logMode, _condition, _context);

#if UNITY_EDITOR
        Debug.Break();
        yield return null; //Skip frame to wait for editor pause
#endif
    }

    public static void Log(string _message, Color _textColor = default, LogMode _logMode = LogMode.Always, bool _condition = true, Object _context = null)
    {
        if (_condition && _logMode.isValidLogMode())
        {
            Debug.Log($"{_message}".applyColor(_textColor), _context);
        }
    }

    public static void Warning(string _message, Color _textColor = default, LogMode _logMode = LogMode.Always, bool _condition = true, Object _context = null)
    {
        if (_condition && _logMode.isValidLogMode())
        {
            Debug.LogWarning(_message.applyColor(_textColor), _context);
        }
    }

    public static void Error(string _message, Color _textColor = default, LogMode _logMode = LogMode.Always, bool _condition = true, Object _context = null)
    {
        if (_condition && _logMode.isValidLogMode())
        {
            Debug.LogError(_message.applyColor(_textColor), _context);
        }
    }

    private static string applyColor(this string _message, Color _color)
    {
#if !UNITY_EDITOR
        return _message;
#else
        if (_color == default)
        {
            return _message;
        }

        return _message.GetTMProTextWithColor(_color);
#endif
    }

    private static bool isValidLogMode(this LogMode _logMode)
    {
        return _logMode switch
        {
            LogMode.Always => true,
            LogMode.EditorOnly => CoreData.IS_EDITOR,
            LogMode.BuildOnly => CoreData.IS_BUILD,
            LogMode.DevelopmentBuild => CoreData.IS_DEVELOPMENT_BUILD,
            LogMode.ShipmentBuild => CoreData.IS_SHIPMENT_BUILD,
            _ => false,
        };
    }
}
