namespace DL.LogConsole
{
    using UnityEngine;

    [System.Serializable]
    public class LogColorSettings
    {
        [SerializeField] private Color logColor = Color.white;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color errorColor = new Color(1f, 0.3f, 0.3f);
        [SerializeField] private Color exceptionColor = Color.red;
        [SerializeField] private Color assertColor = new Color(1f, 0f, 1f);

        public string ApplyColor(string _message, LogType _type) => ApplyColor(_message, GetLogColor(_type));
        public string ApplyColor(string _message, Color _color) => $"<color=#{ColorUtility.ToHtmlStringRGB(_color)}>{_message}</color>";
        public Color GetLogColor(LogType _type) => _type switch
        {
            LogType.Log => logColor,
            LogType.Warning => warningColor,
            LogType.Error => errorColor,
            LogType.Exception => exceptionColor,
            LogType.Assert => assertColor,
            _ => logColor,
        };
    }
}
