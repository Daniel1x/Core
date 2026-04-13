namespace DL.Localization
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "LocalizationSource", menuName = "DL/Localization/Localization Source", order = 1)]
    public class LocalizationSource : ScriptableObject
    {
        [SerializeField] private List<LocalizationData> data = new List<LocalizationData>();

        private Dictionary<string, LocalizationData> dataDictionary = new Dictionary<string, LocalizationData>();
        private string[] keys = new string[] { };

        public string[] Keys => keys;

        public bool Initialized => dataDictionary != null && dataDictionary.Count > 0 && keys != null && keys.Length > 0 && data.Count == dataDictionary.Count;

        public void Initialize()
        {
            int _dataCount = data.Count;
            dataDictionary = new Dictionary<string, LocalizationData>(_dataCount);
            keys = new string[_dataCount];

            for (int i = 0; i < data.Count; i++)
            {
                LocalizationData _item = data[i];

                if (!dataDictionary.ContainsKey(_item.Key))
                {
                    dataDictionary.Add(_item.Key, _item);
                    keys[i] = _item.Key;
                }
                else
                {
                    Debug.LogWarning($"Duplicate localization key found: {_item.Key}. Skipping duplicate.");
                }
            }
        }

        public bool HasKey(string _key, out LocalizationData _data)
        {
            return dataDictionary.TryGetValue(_key, out _data);
        }

        public void SetKey(string _key, LocalizationData _data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].Key == _key)
                {
                    data[i] = _data;
                    dataDictionary[_key] = _data;
                    return;
                }
            }

            data.Add(_data);
            dataDictionary[_key] = _data;
            keys = dataDictionary.Keys.ToArray();
        }

        public void RemoveKey(string _key)
        {
            if (HasKey(_key, out _) == false)
            {
                return;
            }

            data.RemoveAll(d => d.Key == _key);
            dataDictionary.Remove(_key);
            keys = dataDictionary.Keys.ToArray();
        }

        [ContextMenu("Adjust Array Sizes To Language Count and Remove If Too Many")]
        public void AdjustArraySizeToLanguagesAndRemoveIfTooMany() => AdjustArraySizesToLanguageCount(true);

        [ContextMenu("Adjust Array Sizes To Language Count and Keep If Too Many")]
        public void AdjustArraySizeToLanguagesAndKeepIfTooMany() => AdjustArraySizesToLanguageCount(false);

        public void AdjustArraySizesToLanguageCount(bool _removeIfTooMany)
        {
#if UNITY_EDITOR
            bool _anyChanged = false;
            int _languageCount = Languages.LanguageCount;

            foreach (LocalizationData _item in data)
            {
                string[] _current = _item.Localization;

                if (_current.Length == _languageCount)
                {
                    continue;
                }

                _item.Localization = Languages.AdjustLanguageArraySize(_current, _removeIfTooMany);
                _anyChanged = true;
            }

            if (_anyChanged)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
                Debug.Log("Adjusted localization array sizes to match language count.");
            }
#endif
        }

        public HashSet<char> GetAllUniqueCharacters()
        {
            HashSet<char> _uniqueChars = new HashSet<char>();

            foreach (LocalizationData _item in data)
            {
                foreach (string _localization in _item.Localization)
                {
                    if (string.IsNullOrEmpty(_localization))
                    {
                        continue;
                    }
                    foreach (char _c in _localization)
                    {
                        _uniqueChars.Add(_c);
                    }
                }
            }

            return _uniqueChars;
        }
    }
}
