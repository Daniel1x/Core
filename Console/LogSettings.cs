namespace DL.LogConsole
{
    using UnityEngine;

    [System.Serializable]
    public class LogSettings
    {
        public event System.Action OnSettingsChanged;

        [SerializeField] private bool showLogType = true;
        [SerializeField] private bool showStackTrace = false;
        [SerializeField] private bool showLogs = true;
        [SerializeField] private bool showWarnings = true;
        [SerializeField] private bool showErrors = true;
        [SerializeField] private bool showAssertions = true;
        [SerializeField] private bool showExceptions = true;

        public bool ShowLogType
        {
            get => showLogType;
            set
            {
                if (showLogType != value)
                {
                    showLogType = value;
                    OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool ShowStackTrace
        {
            get => showStackTrace;
            set
            {
                if (showStackTrace != value)
                {
                    showStackTrace = value;
                    OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool ShowLogs
        {
            get => showLogs;
            set
            {
                if (showLogs != value)
                {
                    showLogs = value;
                    OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool ShowWarnings
        {
            get => showWarnings;
            set
            {
                if (showWarnings != value)
                {
                    showWarnings = value;
                    OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool ShowErrors
        {
            get => showErrors;
            set
            {
                if (showErrors != value)
                {
                    showErrors = value;
                    OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool ShowAssertions
        {
            get => showAssertions;
            set
            {
                if (showAssertions != value)
                {
                    showAssertions = value;
                    OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool ShowExceptions
        {
            get => showExceptions;
            set
            {
                if (showExceptions != value)
                {
                    showExceptions = value;
                    OnSettingsChanged?.Invoke();
                }
            }
        }

        public bool IsLogAllowed(LogType logType)
        {
            return logType switch
            {
                LogType.Log => ShowLogs,
                LogType.Warning => ShowWarnings,
                LogType.Error => ShowErrors,
                LogType.Assert => ShowAssertions,
                LogType.Exception => ShowExceptions,
                _ => false,
            };
        }
    }
}
