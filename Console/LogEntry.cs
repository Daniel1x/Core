namespace DL.LogConsole
{
    using UnityEngine;

    public class LogEntry
    {
        public readonly string Message;
        public readonly string StackTrace;
        public readonly LogType Type;

        public LogEntry(string _message, string _stackTrace, LogType _type)
        {
            Message = _message;
            StackTrace = _stackTrace;
            Type = _type;
        }
    }
}
