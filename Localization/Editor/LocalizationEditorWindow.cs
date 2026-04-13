namespace DL.Localization.Editor
{
    using DL.Localization;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public class LocalizationEditorWindow : EditorWindow
    {
        private const float k_ToolbarHeight = 22f;
        private const float k_LanguageToggleWidth = 90f;
        private const float k_KeyColumnMinWidth = 200f;
        private const float k_TranslationColumnMinWidth = 200f;
        private const float k_RowHeight = 20f;
        private const float k_Spacing = 4f;
        private const float k_SectionSpacing = 8f;

        private static readonly Color s_HeaderColor = new Color(0.22f, 0.22f, 0.22f, 1f);
        private static readonly Color s_RowEvenColor = new Color(0.25f, 0.25f, 0.25f, 0.3f);
        private static readonly Color s_RowOddColor = new Color(0.2f, 0.2f, 0.2f, 0.3f);
        private static readonly Color s_SelectedLanguageColor = new Color(0.3f, 0.5f, 0.9f, 0.25f);
        private static readonly Color s_EmptyTranslationColor = new Color(1f, 0.4f, 0.3f, 0.2f);

        [SerializeField] private LocalizationSource source;
        [SerializeField] private bool[] visibleLanguages;
        [SerializeField] private string filterText = string.Empty;
        [SerializeField] private bool filterByTerm = true;
        [SerializeField] private bool filterByTranslations = false;
        [SerializeField] private string newKeyName = string.Empty;

        private Vector2 scrollPosition;
        private string[] cachedKeys;
        private string[] filteredKeys;
        private bool needsFilterRefresh = true;

        [MenuItem("DL/Localization/LocalizationPreview")]
        public static void ShowWindow()
        {
            var _window = GetWindow<LocalizationEditorWindow>("Localization Preview");
            _window.minSize = new Vector2(600f, 400f);
            _window.Show();
        }

        private void OnEnable()
        {
            initializeLanguageToggles();
            refreshCachedData();
        }

        private void OnFocus()
        {
            if (source != null && source.Initialized == false)
            {
                refreshCachedData();
            }
        }

        public void OnGUI()
        {
            drawSourceField();

            if (source == null)
            {
                EditorGUILayout.HelpBox("Assign a LocalizationSource to begin editing translations.", MessageType.Info);
                return;
            }

            drawFilterSection();
            drawLanguageToggles();

            EditorGUILayout.Space(k_SectionSpacing);

            drawTranslationTable();

            EditorGUILayout.Space(k_SectionSpacing);

            drawNewKeySection();
        }

        // ─────────────────────────────────────────────────────────────
        //  Source Field & Refresh
        // ─────────────────────────────────────────────────────────────

        private void drawSourceField()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                source = (LocalizationSource)EditorGUILayout.ObjectField("Source", source, typeof(LocalizationSource), false);

                if (_check.changed)
                {
                    initializeLanguageToggles();
                    refreshCachedData();
                }
            }

            if (source != null && GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60f)))
            {
                refreshCachedData();
            }

            if (source != null && GUILayout.Button("Export CSV", EditorStyles.toolbarButton, GUILayout.Width(75f)))
            {
                exportToCsv();
            }

            if (source != null && GUILayout.Button("Import CSV", EditorStyles.toolbarButton, GUILayout.Width(78f)))
            {
                importFromCsv();
            }

            EditorGUILayout.EndHorizontal();
        }

        // ─────────────────────────────────────────────────────────────
        //  Filter Section
        // ─────────────────────────────────────────────────────────────

        private void drawFilterSection()
        {
            EditorGUILayout.Space(k_Spacing);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Filter", GUILayout.Width(40f));

            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                filterText = EditorGUILayout.TextField(filterText);

                if (_check.changed)
                {
                    needsFilterRefresh = true;
                }
            }

            if (GUILayout.Button("✕", GUILayout.Width(22f)))
            {
                filterText = string.Empty;
                needsFilterRefresh = true;
                GUI.FocusControl(null);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(44f);

            using (var _check = new EditorGUI.ChangeCheckScope())
            {
                filterByTerm = GUILayout.Toggle(filterByTerm, "Match Term", EditorStyles.miniButtonLeft, GUILayout.Width(100f));
                filterByTranslations = GUILayout.Toggle(filterByTranslations, "Match Translations", EditorStyles.miniButtonRight, GUILayout.Width(130f));

                if (_check.changed)
                {
                    needsFilterRefresh = true;
                }
            }

            GUILayout.FlexibleSpace();

            int _visibleCount = filteredKeys != null ? filteredKeys.Length : 0;
            int _totalCount = cachedKeys != null ? cachedKeys.Length : 0;
            EditorGUILayout.LabelField($"{_visibleCount} / {_totalCount}", EditorStyles.miniLabel, GUILayout.Width(80f));

            EditorGUILayout.EndHorizontal();
        }

        // ─────────────────────────────────────────────────────────────
        //  Language Toggles
        // ─────────────────────────────────────────────────────────────

        private void drawLanguageToggles()
        {
            EditorGUILayout.Space(k_Spacing);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Languages", EditorStyles.boldLabel, GUILayout.Width(80f));

            if (GUILayout.Button("All", EditorStyles.miniButtonLeft, GUILayout.Width(36f)))
            {
                setAllLanguagesVisible(true);
            }

            if (GUILayout.Button("None", EditorStyles.miniButtonRight, GUILayout.Width(42f)))
            {
                setAllLanguagesVisible(false);
            }

            GUILayout.Space(k_SectionSpacing);

            for (int i = 0; i < Languages.LanguageCount; i++)
            {
                bool _wasVisible = visibleLanguages[i];

                Color _bgColor = GUI.backgroundColor;

                if (_wasVisible)
                {
                    GUI.backgroundColor = new Color(0.4f, 0.7f, 1f, 1f);
                }

                visibleLanguages[i] = GUILayout.Toggle(visibleLanguages[i], Languages.LanguageNames[i], EditorStyles.miniButton, GUILayout.Width(k_LanguageToggleWidth));
                GUI.backgroundColor = _bgColor;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        // ─────────────────────────────────────────────────────────────
        //  Translation Table
        // ─────────────────────────────────────────────────────────────

        private void drawTranslationTable()
        {
            if (needsFilterRefresh)
            {
                rebuildFilteredKeys();
                needsFilterRefresh = false;
            }

            if (filteredKeys == null || filteredKeys.Length == 0)
            {
                EditorGUILayout.HelpBox("No keys match the current filter.", MessageType.Info);
                return;
            }

            int _visibleLanguageCount = getVisibleLanguageCount();

            if (_visibleLanguageCount == 0)
            {
                EditorGUILayout.HelpBox("Select at least one language to display translations.", MessageType.Info);
                return;
            }

            drawTableHeader(_visibleLanguageCount);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < filteredKeys.Length; i++)
            {
                drawTranslationRow(filteredKeys[i], i, _visibleLanguageCount);
            }

            EditorGUILayout.EndScrollView();
        }

        private void drawTableHeader(int _visibleLanguageCount)
        {
            Rect _headerRect = EditorGUILayout.GetControlRect(false, k_ToolbarHeight);
            EditorGUI.DrawRect(_headerRect, s_HeaderColor);

            float _x = _headerRect.x;
            float _keyWidth = Mathf.Max(k_KeyColumnMinWidth, _headerRect.width * 0.25f);
            float _remainingWidth = _headerRect.width - _keyWidth - 26f;
            float _langWidth = Mathf.Max(k_TranslationColumnMinWidth, _remainingWidth / _visibleLanguageCount);

            EditorGUI.LabelField(new Rect(_x, _headerRect.y, _keyWidth, _headerRect.height), "Key", EditorStyles.boldLabel);
            _x += _keyWidth;

            for (int i = 0; i < Languages.LanguageCount; i++)
            {
                if (visibleLanguages[i] == false)
                {
                    continue;
                }

                Rect _langHeaderRect = new Rect(_x, _headerRect.y, _langWidth, _headerRect.height);
                Rect _highlightRect = new Rect(_x, _headerRect.y, _langWidth, _headerRect.height);
                EditorGUI.DrawRect(_highlightRect, s_SelectedLanguageColor);
                EditorGUI.LabelField(_langHeaderRect, Languages.LanguageNames[i], EditorStyles.boldLabel);
                _x += _langWidth;
            }
        }

        private void drawTranslationRow(string _key, int _rowIndex, int _visibleLanguageCount)
        {
            if (source.HasKey(_key, out LocalizationData _data) == false)
            {
                return;
            }

            Rect _rowRect = EditorGUILayout.GetControlRect(false, k_RowHeight);
            Color _rowColor = _rowIndex % 2 == 0 ? s_RowEvenColor : s_RowOddColor;
            EditorGUI.DrawRect(_rowRect, _rowColor);

            float _x = _rowRect.x;
            float _keyWidth = Mathf.Max(k_KeyColumnMinWidth, _rowRect.width * 0.25f);
            float _remainingWidth = _rowRect.width - _keyWidth - 26f;
            float _langWidth = Mathf.Max(k_TranslationColumnMinWidth, _remainingWidth / _visibleLanguageCount);

            EditorGUI.LabelField(new Rect(_x, _rowRect.y, _keyWidth - k_Spacing, _rowRect.height), _key, EditorStyles.miniLabel);
            _x += _keyWidth;

            for (int i = 0; i < Languages.LanguageCount; i++)
            {
                if (visibleLanguages[i] == false)
                {
                    continue;
                }

                Rect _fieldRect = new Rect(_x, _rowRect.y, _langWidth - k_Spacing, _rowRect.height);
                string _current = _data.GetLocalization(i);

                if (string.IsNullOrEmpty(_current))
                {
                    EditorGUI.DrawRect(_fieldRect, s_EmptyTranslationColor);
                }

                using (var _check = new EditorGUI.ChangeCheckScope())
                {
                    string _newValue = EditorGUI.TextField(_fieldRect, _current);

                    if (_check.changed)
                    {
                        Undo.RecordObject(source, "Edit Localization");
                        _data.SetLocalization(i, _newValue);
                        source.SaveSource();
                    }
                }

                _x += _langWidth;
            }

            // Delete button
            Rect _deleteRect = new Rect(_rowRect.xMax - 22f, _rowRect.y, 20f, _rowRect.height);

            if (GUI.Button(_deleteRect, "✕", EditorStyles.miniButton))
            {
                if (EditorUtility.DisplayDialog("Delete Key", $"Delete localization key '{_key}'?", "Delete", "Cancel"))
                {
                    Undo.RecordObject(source, "Delete Localization Key");
                    source.RemoveKey(_key);
                    source.SaveSource();
                    refreshCachedData();
                    GUIUtility.ExitGUI();
                }
            }
        }

        // ─────────────────────────────────────────────────────────────
        //  New Key Section
        // ─────────────────────────────────────────────────────────────

        private void drawNewKeySection()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("New Key", GUILayout.Width(55f));
            newKeyName = EditorGUILayout.TextField(newKeyName);

            bool _canCreate = string.IsNullOrWhiteSpace(newKeyName) == false && source.HasKey(newKeyName, out _) == false;
            EditorGUI.BeginDisabledGroup(_canCreate == false);

            if (GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(55f)))
            {
                Undo.RecordObject(source, "Create Localization Key");
                source.CreateNewKey(newKeyName);
                refreshCachedData();
                newKeyName = string.Empty;
                GUI.FocusControl(null);
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        // ─────────────────────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────────────────────

        private void initializeLanguageToggles()
        {
            if (visibleLanguages == null || visibleLanguages.Length != Languages.LanguageCount)
            {
                visibleLanguages = new bool[Languages.LanguageCount];

                for (int i = 0; i < visibleLanguages.Length; i++)
                {
                    visibleLanguages[i] = true;
                }
            }
        }

        internal void refreshCachedData()
        {
            if (source == null)
            {
                cachedKeys = new string[0];
                filteredKeys = new string[0];
                return;
            }

            source.Initialize();
            cachedKeys = source.Keys;
            needsFilterRefresh = true;
            Repaint();
        }

        private void rebuildFilteredKeys()
        {
            if (cachedKeys == null)
            {
                filteredKeys = new string[0];
                return;
            }

            if (string.IsNullOrEmpty(filterText) || (filterByTerm == false && filterByTranslations == false))
            {
                filteredKeys = cachedKeys;
                return;
            }

            var _result = new System.Collections.Generic.List<string>(cachedKeys.Length);

            for (int i = 0; i < cachedKeys.Length; i++)
            {
                string _key = cachedKeys[i];

                if (_key == null)
                {
                    continue;
                }

                if (filterByTerm && _key.IndexOf(filterText, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _result.Add(_key);
                    continue;
                }

                if (filterByTranslations && source.HasKey(_key, out LocalizationData _data))
                {
                    bool _found = false;

                    for (int j = 0; j < _data.Localization.Length; j++)
                    {
                        string _translation = _data.Localization[j];

                        if (_translation != null && _translation.IndexOf(filterText, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            _found = true;
                            break;
                        }
                    }

                    if (_found)
                    {
                        _result.Add(_key);
                    }
                }
            }

            filteredKeys = _result.ToArray();
        }

        private int getVisibleLanguageCount()
        {
            int _count = 0;

            for (int i = 0; i < visibleLanguages.Length; i++)
            {
                if (visibleLanguages[i])
                {
                    _count++;
                }
            }

            return _count;
        }

        private void setAllLanguagesVisible(bool _visible)
        {
            for (int i = 0; i < visibleLanguages.Length; i++)
            {
                visibleLanguages[i] = _visible;
            }
        }

        // ─────────────────────────────────────────────────────────────
        //  CSV Export
        // ─────────────────────────────────────────────────────────────

        private void exportToCsv()
        {
            if (source == null || cachedKeys == null || cachedKeys.Length == 0)
            {
                EditorUtility.DisplayDialog("Export CSV", "No localization data to export.", "OK");
                return;
            }

            string _defaultName = source.name + "_localization";
            string _path = EditorUtility.SaveFilePanel("Export Localization to CSV", "", _defaultName, "csv");

            if (string.IsNullOrEmpty(_path))
            {
                return;
            }

            var _sb = new StringBuilder();

            // Header row
            _sb.Append(escapeCsvField("Key"));

            for (int i = 0; i < Languages.LanguageCount; i++)
            {
                _sb.Append(',');
                _sb.Append(escapeCsvField(Languages.LanguageNames[i]));
            }

            _sb.AppendLine();

            // Data rows
            for (int i = 0; i < cachedKeys.Length; i++)
            {
                string _key = cachedKeys[i];

                if (_key == null || source.HasKey(_key, out LocalizationData _data) == false)
                {
                    continue;
                }

                _sb.Append(escapeCsvField(_key));

                for (int j = 0; j < Languages.LanguageCount; j++)
                {
                    _sb.Append(',');
                    _sb.Append(escapeCsvField(_data.GetLocalization(j)));
                }

                _sb.AppendLine();
            }

            File.WriteAllText(_path, _sb.ToString(), Encoding.UTF8);
            Debug.Log($"Localization exported to: {_path}");
            EditorUtility.DisplayDialog("Export CSV", $"Exported {cachedKeys.Length} keys to:\n{_path}", "OK");
        }

        private static string escapeCsvField(string _field)
        {
            if (string.IsNullOrEmpty(_field))
            {
                return string.Empty;
            }

            if (_field.IndexOfAny(new char[] { ',', '"', '\n', '\r' }) >= 0)
            {
                return '"' + _field.Replace("\"", "\"\"") + '"';
            }

            return _field;
        }

        // ─────────────────────────────────────────────────────────────
        //  CSV Import
        // ─────────────────────────────────────────────────────────────

        private void importFromCsv()
        {
            string _path = EditorUtility.OpenFilePanel("Import Localization from CSV", "", "csv");

            if (string.IsNullOrEmpty(_path))
            {
                return;
            }

            string[] _lines = File.ReadAllLines(_path, Encoding.UTF8);

            if (_lines.Length < 2)
            {
                EditorUtility.DisplayDialog("Import CSV", "CSV file is empty or contains only a header row.", "OK");
                return;
            }

            var _headerFields = parseCsvLine(_lines[0]);

            if (_headerFields.Count < 2)
            {
                EditorUtility.DisplayDialog("Import CSV", "CSV header must contain at least a Key column and one language column.", "OK");
                return;
            }

            int _keyColumnIndex = -1;
            int[] _languageColumnMap = new int[_headerFields.Count];

            for (int i = 0; i < _languageColumnMap.Length; i++)
            {
                _languageColumnMap[i] = -1;
            }

            for (int i = 0; i < _headerFields.Count; i++)
            {
                string _header = _headerFields[i].Trim();

                if (string.Equals(_header, "Key", System.StringComparison.OrdinalIgnoreCase))
                {
                    _keyColumnIndex = i;
                    continue;
                }

                for (int j = 0; j < Languages.LanguageCount; j++)
                {
                    if (string.Equals(_header, Languages.LanguageNames[j], System.StringComparison.OrdinalIgnoreCase))
                    {
                        _languageColumnMap[i] = j;
                        break;
                    }
                }
            }

            if (_keyColumnIndex < 0)
            {
                EditorUtility.DisplayDialog("Import CSV", "CSV header is missing a 'Key' column.", "OK");
                return;
            }

            var _entries = new List<CsvImportPreviewWindow.ImportEntry>();

            for (int i = 1; i < _lines.Length; i++)
            {
                string _line = _lines[i];

                if (string.IsNullOrWhiteSpace(_line))
                {
                    continue;
                }

                var _fields = parseCsvLine(_line);

                if (_fields.Count <= _keyColumnIndex)
                {
                    continue;
                }

                string _key = _fields[_keyColumnIndex].Trim();

                if (string.IsNullOrEmpty(_key))
                {
                    continue;
                }

                bool _isNew = source.HasKey(_key, out LocalizationData _existing) == false;
                string[] _oldValues = new string[Languages.LanguageCount];
                string[] _newValues = new string[Languages.LanguageCount];

                for (int j = 0; j < Languages.LanguageCount; j++)
                {
                    _oldValues[j] = _isNew ? string.Empty : _existing.GetLocalization(j) ?? string.Empty;
                    _newValues[j] = _oldValues[j];
                }

                for (int col = 0; col < _fields.Count; col++)
                {
                    if (col >= _languageColumnMap.Length)
                    {
                        break;
                    }

                    int _langIndex = _languageColumnMap[col];

                    if (_langIndex >= 0)
                    {
                        _newValues[_langIndex] = _fields[col];
                    }
                }

                bool _hasAnyChange = _isNew;

                if (_hasAnyChange == false)
                {
                    for (int j = 0; j < Languages.LanguageCount; j++)
                    {
                        if (_oldValues[j] != _newValues[j])
                        {
                            _hasAnyChange = true;
                            break;
                        }
                    }
                }

                if (_hasAnyChange)
                {
                    _entries.Add(new CsvImportPreviewWindow.ImportEntry
                    {
                        Key = _key,
                        IsNew = _isNew,
                        OldValues = _oldValues,
                        NewValues = _newValues,
                    });
                }
            }

            if (_entries.Count == 0)
            {
                EditorUtility.DisplayDialog("Import CSV", "No changes detected. The CSV matches the current localization data.", "OK");
                return;
            }

            CsvImportPreviewWindow.Show(source, _entries, this);
        }

        private static System.Collections.Generic.List<string> parseCsvLine(string _line)
        {
            var _fields = new System.Collections.Generic.List<string>();
            int _length = _line.Length;
            int _pos = 0;

            while (_pos <= _length)
            {
                if (_pos == _length)
                {
                    _fields.Add(string.Empty);
                    break;
                }

                if (_line[_pos] == '"')
                {
                    // Quoted field
                    var _sb = new StringBuilder();
                    _pos++;

                    while (_pos < _length)
                    {
                        if (_line[_pos] == '"')
                        {
                            if (_pos + 1 < _length && _line[_pos + 1] == '"')
                            {
                                _sb.Append('"');
                                _pos += 2;
                            }
                            else
                            {
                                _pos++;
                                break;
                            }
                        }
                        else
                        {
                            _sb.Append(_line[_pos]);
                            _pos++;
                        }
                    }

                    _fields.Add(_sb.ToString());

                    if (_pos < _length && _line[_pos] == ',')
                    {
                        _pos++;
                    }
                }
                else
                {
                    // Unquoted field
                    int _start = _pos;

                    while (_pos < _length && _line[_pos] != ',')
                    {
                        _pos++;
                    }

                    _fields.Add(_line.Substring(_start, _pos - _start));

                    if (_pos < _length)
                    {
                        _pos++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return _fields;
        }
    }

    public class CsvImportPreviewWindow : EditorWindow
    {
        private const float k_RowHeight = 20f;
        private const float k_Spacing = 2f;

        private static readonly Color s_NewKeyColor = new Color(0.3f, 0.8f, 0.4f, 0.25f);
        private static readonly Color s_ModifiedColor = new Color(1f, 0.8f, 0.2f, 0.2f);
        private static readonly Color s_OldValueColor = new Color(1f, 0.4f, 0.4f, 0.15f);
        private static readonly Color s_NewValueColor = new Color(0.4f, 1f, 0.5f, 0.15f);
        private static readonly Color s_HeaderColor = new Color(0.22f, 0.22f, 0.22f, 1f);
        private static readonly Color s_RowEvenColor = new Color(0.25f, 0.25f, 0.25f, 0.3f);
        private static readonly Color s_RowOddColor = new Color(0.2f, 0.2f, 0.2f, 0.3f);

        public struct ImportEntry
        {
            public string Key;
            public bool IsNew;
            public string[] OldValues;
            public string[] NewValues;
        }

        private LocalizationSource source;
        private List<ImportEntry> entries;
        private LocalizationEditorWindow parentWindow;
        private Vector2 scrollPosition;
        private bool showUnchangedLanguages = false;

        public static void Show(LocalizationSource _source, List<ImportEntry> _entries, LocalizationEditorWindow _parent)
        {
            var _window = GetWindow<CsvImportPreviewWindow>(true, "Import Preview", true);
            _window.source = _source;
            _window.entries = _entries;
            _window.parentWindow = _parent;
            _window.minSize = new Vector2(700f, 400f);
            _window.ShowUtility();
        }

        private void OnGUI()
        {
            if (source == null || entries == null)
            {
                EditorGUILayout.HelpBox("No import data available.", MessageType.Warning);
                return;
            }

            drawSummary();
            drawOptions();
            drawChangesTable();
            drawActionButtons();
        }

        private void drawSummary()
        {
            int _newCount = 0;
            int _modifiedCount = 0;

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].IsNew)
                {
                    _newCount++;
                }
                else
                {
                    _modifiedCount++;
                }
            }

            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField("Import Preview", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"New keys: {_newCount}   |   Modified keys: {_modifiedCount}   |   Total changes: {entries.Count}");
            EditorGUILayout.Space(4f);
        }

        private void drawOptions()
        {
            showUnchangedLanguages = EditorGUILayout.Toggle("Show unchanged languages", showUnchangedLanguages);
            EditorGUILayout.Space(2f);
        }

        private void drawChangesTable()
        {
            float _keyWidth = 200f;
            float _langLabelWidth = 80f;
            float _valueWidth = Mathf.Max(180f, (position.width - _keyWidth - _langLabelWidth - 40f) * 0.5f);

            // Header
            Rect _headerRect = EditorGUILayout.GetControlRect(false, k_RowHeight);
            EditorGUI.DrawRect(_headerRect, s_HeaderColor);

            float _x = _headerRect.x;
            EditorGUI.LabelField(new Rect(_x, _headerRect.y, _keyWidth, _headerRect.height), "Key", EditorStyles.boldLabel);
            _x += _keyWidth;
            EditorGUI.LabelField(new Rect(_x, _headerRect.y, _langLabelWidth, _headerRect.height), "Language", EditorStyles.boldLabel);
            _x += _langLabelWidth;
            EditorGUI.LabelField(new Rect(_x, _headerRect.y, _valueWidth, _headerRect.height), "Current Value", EditorStyles.boldLabel);
            _x += _valueWidth;
            EditorGUI.LabelField(new Rect(_x, _headerRect.y, _valueWidth, _headerRect.height), "New Value", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            int _rowIndex = 0;

            for (int i = 0; i < entries.Count; i++)
            {
                var _entry = entries[i];

                bool _drewKeyLabel = false;

                for (int j = 0; j < Languages.LanguageCount; j++)
                {
                    bool _changed = _entry.OldValues[j] != _entry.NewValues[j];

                    if (_changed == false && showUnchangedLanguages == false)
                    {
                        continue;
                    }

                    Rect _rowRect = EditorGUILayout.GetControlRect(false, k_RowHeight);

                    Color _bgColor = _rowIndex % 2 == 0 ? s_RowEvenColor : s_RowOddColor;
                    EditorGUI.DrawRect(_rowRect, _bgColor);

                    if (_entry.IsNew)
                    {
                        EditorGUI.DrawRect(_rowRect, s_NewKeyColor);
                    }
                    else if (_changed)
                    {
                        EditorGUI.DrawRect(_rowRect, s_ModifiedColor);
                    }

                    _x = _rowRect.x;

                    if (_drewKeyLabel == false)
                    {
                        string _keyLabel = _entry.IsNew ? $"{_entry.Key}  [NEW]" : _entry.Key;
                        EditorGUI.LabelField(new Rect(_x, _rowRect.y, _keyWidth - k_Spacing, _rowRect.height), _keyLabel, EditorStyles.miniLabel);
                        _drewKeyLabel = true;
                    }

                    _x += _keyWidth;

                    EditorGUI.LabelField(new Rect(_x, _rowRect.y, _langLabelWidth - k_Spacing, _rowRect.height), Languages.LanguageNames[j], EditorStyles.miniLabel);
                    _x += _langLabelWidth;

                    Rect _oldRect = new Rect(_x, _rowRect.y, _valueWidth - k_Spacing, _rowRect.height);

                    if (_changed)
                    {
                        EditorGUI.DrawRect(_oldRect, s_OldValueColor);
                    }

                    EditorGUI.SelectableLabel(_oldRect, _entry.OldValues[j], EditorStyles.miniLabel);
                    _x += _valueWidth;

                    Rect _newRect = new Rect(_x, _rowRect.y, _valueWidth - k_Spacing, _rowRect.height);

                    if (_changed)
                    {
                        EditorGUI.DrawRect(_newRect, s_NewValueColor);
                    }

                    EditorGUI.SelectableLabel(_newRect, _entry.NewValues[j], EditorStyles.miniLabel);

                    _rowIndex++;
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void drawActionButtons()
        {
            EditorGUILayout.Space(6f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(120f), GUILayout.Height(28f)))
            {
                Close();
            }

            GUILayout.Space(8f);

            Color _prevBg = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f, 1f);

            if (GUILayout.Button($"Apply {entries.Count} Changes", GUILayout.Width(180f), GUILayout.Height(28f)))
            {
                applyImport();
            }

            GUI.backgroundColor = _prevBg;

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4f);
        }

        private void applyImport()
        {
            if (source == null)
            {
                Close();
                return;
            }

            Undo.RecordObject(source, "Import Localization CSV");

            int _createdCount = 0;

            for (int i = 0; i < entries.Count; i++)
            {
                var _entry = entries[i];
                bool _isNew = source.HasKey(_entry.Key, out LocalizationData _data) == false;

                if (_isNew)
                {
                    _data = new LocalizationData(_entry.Key, Languages.LanguageCount);
                    _createdCount++;
                }

                for (int j = 0; j < Languages.LanguageCount; j++)
                {
                    _data.SetLocalization(j, _entry.NewValues[j]);
                }

                source.SetKey(_entry.Key, _data);
            }

            source.SaveSource();

            Debug.Log($"Localization imported: {entries.Count} keys ({_createdCount} new)");
            EditorUtility.DisplayDialog("Import CSV", $"Applied {entries.Count} changes ({_createdCount} new keys).", "OK");

            if (parentWindow != null)
            {
                parentWindow.refreshCachedData();
            }

            Close();
        }
    }
}
