namespace DL.Localization.Editor
{
    using DL.Localization;
    using UnityEditor;
    using UnityEngine;

    public static class LocalizationEditorExtensions
    {
        private static LocalizationSource localizationSource;

        public static LocalizationSource GetSource()
        {
            if (localizationSource == null)
            {
                localizationSource = FindScriptableOfType<LocalizationSource>("Loc");

                if (LocalizationManager.IsInitialized == false)
                {
                    LocalizationManager.SetLocalizationSource(localizationSource);
                }
            }

            return localizationSource;
        }

        public static void CreateNewKey(this LocalizationSource _source, string _key, params string[] _translations)
        {
            if (_source == null || string.IsNullOrEmpty(_key))
            {
                return;
            }

            LocalizationData _newData = new LocalizationData(_key, Languages.LanguageCount);

            if (_translations != null)
            {
                for (int i = 0; i < Languages.LanguageCount && i < _translations.Length; i++)
                {
                    _newData.SetLocalization(i, _translations[i]);
                }
            }
            else
            {
                string _defaultTranslation = _key.Split('/', '\\')[^1];

                for (int i = 0; i < Languages.LanguageCount; i++)
                {
                    _newData.SetLocalization(i, _defaultTranslation);
                }
            }

            _source.SetKey(_key, _newData);
            _source.SaveSource();
        }

        public static void SaveSource(this LocalizationSource _source)
        {
            if (_source != null)
            {
                EditorUtility.SetDirty(_source);
            }

            AssetDatabase.SaveAssets();
        }

        public static T FindScriptableOfType<T>(string _filter = "Loc") where T : ScriptableObject
        {
            string[] _guids = AssetDatabase.FindAssets("t:" + typeof(T));

            if (_guids.Length > 0)
            {
                string _assetPath = string.Empty;

                for (int i = 0; i < _guids.Length; i++)
                {
                    _assetPath = AssetDatabase.GUIDToAssetPath(_guids[i]);

                    if (_assetPath.Contains(_filter, System.StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }

                return AssetDatabase.LoadAssetAtPath<T>(_assetPath);
            }

            return null;
        }
    }
}
