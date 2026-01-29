namespace DL.Localization
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu(fileName = "LocalizationSource", menuName = "DL/Localization/Localization Source", order = 1)]
    public class LocalizationSource : ScriptableObject
    {
        [SerializeField] private LanguageList languages = new LanguageList();
        [SerializeField] private LocalizationData[] data = new LocalizationData[] { };

        private Dictionary<string, LocalizationData> dataDictionary = new Dictionary<string, LocalizationData>();
        private string[] keys = new string[] { };

        public LanguageList LanguageList => languages;
        public string[] Keys => keys;

        public bool Initialized => dataDictionary != null && dataDictionary.Count > 0 && keys != null && keys.Length > 0 && data.Length == dataDictionary.Count;

        public void Initialize()
        {
            int _dataCount = data.Length;
            dataDictionary = new Dictionary<string, LocalizationData>(_dataCount);
            keys = new string[_dataCount];

            for (int i = 0; i < data.Length; i++)
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
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Key == _key)
                {
                    data[i] = _data;
                    dataDictionary[_key] = _data;
                    return;
                }
            }

            data = Enumerable.Append(data, _data).ToArray();
            dataDictionary[_key] = _data;
            keys = dataDictionary.Keys.ToArray();
        }

        public void RemoveKey(string _key)
        {
            if (HasKey(_key, out _) == false)
            {
                return;
            }

            List<LocalizationData> _dataList = data.ToList();
            _dataList.RemoveAll(item => item.Key == _key);
            data = _dataList.ToArray();
            dataDictionary.Remove(_key);
            keys = dataDictionary.Keys.ToArray();
        }

        [ContextMenu("Adjust Array Sizes To Language Count")]
        public void AdjustArraySizesToLanguageCount()
        {
            bool _anyChanged = false;
            int _languageCount = Languages.LanguageCount;

            foreach (LocalizationData _item in data)
            {
                string[] _current = _item.Localization;

                if (_current.Length == _languageCount)
                {
                    continue;
                }

                _item.Localization = Languages.AdjustLanguageArraySize(_current);
                _anyChanged = true;
            }

#if UNITY_EDITOR
            if (_anyChanged)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssets();
                Debug.Log("Adjusted localization array sizes to match language count.");
            }
#endif
        }
    }
}
