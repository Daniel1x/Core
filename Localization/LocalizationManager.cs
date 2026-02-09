namespace DL.Localization
{
    public static class LocalizationManager
    {
        public static event System.Action OnLocalizationChanged;

        private static LocalizationSource localizationSource;

        public static bool IsInitialized => localizationSource != null;

        public static string GetTranslation(ILocalizationKey _key)
        {
            return _key != null ? GetTranslation(_key.LocalizationKey) : string.Empty;
        }

        public static string GetTranslation(string _key)
        {
            if (string.IsNullOrEmpty(_key) || localizationSource == null)
            {
                return string.Empty;
            }

            return localizationSource.HasKey(_key, out LocalizationData _data)
                ? _data.GetLocalization(localizationSource.LanguageList.SelectedLanguageIndex)
                : $"Key not found: {_key}";
        }

        public static void SetLocalizationSource(LocalizationSource _source)
        {
            if (ReferenceEquals(localizationSource, _source))
            {
                return;
            }

            clearCurrentSourceSubscriptions();

            localizationSource = _source;
            localizationSource.Initialize();
            localizationSource.LanguageList.OnLanguageChanged += onLanguageChanged;

            OnLocalizationChanged?.Invoke();
        }

        private static void clearCurrentSourceSubscriptions()
        {
            if (localizationSource != null)
            {
                localizationSource.LanguageList.OnLanguageChanged -= onLanguageChanged;
            }
        }

        private static void onLanguageChanged()
        {
            OnLocalizationChanged?.Invoke();
        }
    }
}
