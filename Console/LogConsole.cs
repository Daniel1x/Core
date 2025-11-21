namespace DL.LogConsole
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class LogConsole : MonoBehaviour
    {
        private const int DEFAULT_MAX_LOG_COUNT = 100;
        private const int DEFAULT_LINE_CAPACITY = 100;
        private const int DEFAULT_STRINGBUILDER_CAPACITY = DEFAULT_MAX_LOG_COUNT * DEFAULT_LINE_CAPACITY;

        [Header("UI References")]
        [SerializeField] private TMP_Text consoleText;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Slider fontSizeSlider;
        [SerializeField] private Toggle showLogTypeToggle;
        [SerializeField] private Toggle showStackTraceToggle;
        [SerializeField] private Toggle showInfoToggle;
        [SerializeField] private Toggle showWarningToggle;
        [SerializeField] private Toggle showErrorToggle;

        [Header("Settings")]
        [SerializeField, Min(1)] private int maxLogCount = DEFAULT_MAX_LOG_COUNT;
        [SerializeField] private float defaultFontSize = 14f;
        [SerializeField] private LogSettings logSettings = new();

        [Header("Color Settings")]
        [SerializeField] private LogColorSettings colorSettings = new();

        private readonly List<LogEntry> logs = new List<LogEntry>(DEFAULT_MAX_LOG_COUNT);
        private readonly ConcurrentQueue<LogEntry> threadedQueue = new ConcurrentQueue<LogEntry>();
        private readonly StringBuilder consoleStringBuilder = new StringBuilder(DEFAULT_STRINGBUILDER_CAPACITY);
        private readonly StringBuilder lineStringBuilder = new StringBuilder(DEFAULT_LINE_CAPACITY);

        private bool pendingRebuild = false;

        private void OnEnable()
        {
            setDefaultUIValues();
            Application.logMessageReceivedThreaded += onThreadedLog;
            logSettings.OnSettingsChanged += markForRebuild;
            registerEvents();
            rebuildConsole();
        }

        private void OnDisable()
        {
            Application.logMessageReceivedThreaded -= onThreadedLog;
            logSettings.OnSettingsChanged -= markForRebuild;
            unregisterEvents();
        }

        private void Update()
        {
            processQueuedLogs();

            if (pendingRebuild)
            {
                rebuildConsole();
                pendingRebuild = false;
            }
        }

        private void processQueuedLogs()
        {
            bool _any = false;

            while (threadedQueue.TryDequeue(out LogEntry entry))
            {
                _any = true;
                logs.Add(entry);

                if (logs.Count > maxLogCount)
                {
                    logs.RemoveAt(0);
                }
            }

            if (_any)
            {
                pendingRebuild = true;
            }
        }

        private void registerEvents()
        {
            if (fontSizeSlider != null)
            {
                fontSizeSlider.onValueChanged.AddListener(onFontSizeChanged);
            }

            _addToggleListener(showLogTypeToggle, onShowLogTypeToggleChanged);
            _addToggleListener(showStackTraceToggle, onShowStackTraceToggleChanged);
            _addToggleListener(showInfoToggle, onShowInfoToggleChanged);
            _addToggleListener(showWarningToggle, onShowWarningToggleChanged);
            _addToggleListener(showErrorToggle, onShowErrorToggleChanged);

            void _addToggleListener(Toggle _toggle, UnityAction<bool> _handler)
            {
                if (_toggle != null)
                {
                    _toggle.onValueChanged.AddListener(_handler);
                }
            }
        }

        private void unregisterEvents()
        {
            if (fontSizeSlider != null)
            {
                fontSizeSlider.onValueChanged.RemoveListener(onFontSizeChanged);
            }

            _removeToggleListener(showLogTypeToggle, onShowLogTypeToggleChanged);
            _removeToggleListener(showStackTraceToggle, onShowStackTraceToggleChanged);
            _removeToggleListener(showInfoToggle, onShowInfoToggleChanged);
            _removeToggleListener(showWarningToggle, onShowWarningToggleChanged);
            _removeToggleListener(showErrorToggle, onShowErrorToggleChanged);

            void _removeToggleListener(Toggle _toggle, UnityAction<bool> _handler)
            {
                if (_toggle != null)
                {
                    _toggle.onValueChanged.RemoveListener(_handler);
                }
            }
        }

        private void onShowLogTypeToggleChanged(bool value) => logSettings.ShowLogType = value;
        private void onShowStackTraceToggleChanged(bool value) => logSettings.ShowStackTrace = value;
        private void onShowInfoToggleChanged(bool value) => logSettings.ShowLogs = value;
        private void onShowWarningToggleChanged(bool value) => logSettings.ShowWarnings = value;
        private void onShowErrorToggleChanged(bool value) => logSettings.ShowErrors = value;
        private void onThreadedLog(string message, string stackTrace, LogType type) => threadedQueue.Enqueue(new LogEntry(message, stackTrace, type));

        private void onFontSizeChanged(float size)
        {
            if (consoleText == null)
            {
                return;
            }

            consoleText.fontSize = size;
            markForRebuild();
        }

        private void markForRebuild() => pendingRebuild = true;

        private void rebuildConsole()
        {
            if (consoleText == null)
            {
                return;
            }

            consoleStringBuilder.Clear();

            foreach (LogEntry entry in logs)
            {
                if (!logSettings.IsLogAllowed(entry.Type))
                {
                    continue;
                }

                string _line = buildLogLine(entry);
                string _colored = colorSettings.ApplyColor(_line, entry.Type);
                consoleStringBuilder.AppendLine(_colored);
            }

            consoleText.text = consoleStringBuilder.ToString();

            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        private string buildLogLine(LogEntry _entry)
        {
            lineStringBuilder.Clear();

            if (logSettings.ShowLogType)
            {
                lineStringBuilder.Append('[').Append(_entry.Type.ToString()).Append("] ");
            }

            lineStringBuilder.Append(_entry.Message);

            if (logSettings.ShowStackTrace && !string.IsNullOrEmpty(_entry.StackTrace))
            {
                lineStringBuilder.AppendLine().Append(_entry.StackTrace);
            }

            return lineStringBuilder.ToString();
        }

        private void setDefaultUIValues()
        {
            if (fontSizeSlider != null)
            {
                fontSizeSlider.value = defaultFontSize;
            }

            applyDefaultToggleStates();

            if (consoleText != null)
            {
                consoleText.fontSize = defaultFontSize;
            }
        }

        private void applyDefaultToggleStates()
        {
            if (showLogTypeToggle != null)
            {
                showLogTypeToggle.isOn = logSettings.ShowLogType;
            }

            if (showStackTraceToggle != null)
            {
                showStackTraceToggle.isOn = logSettings.ShowStackTrace;
            }

            if (showInfoToggle != null)
            {
                showInfoToggle.isOn = logSettings.ShowLogs;
            }

            if (showWarningToggle != null)
            {
                showWarningToggle.isOn = logSettings.ShowWarnings;
            }

            if (showErrorToggle != null)
            {
                showErrorToggle.isOn = logSettings.ShowErrors;
            }
        }
    }
}
