namespace DL.Localization
{
    public static class LocalizationManager
    {
        public static event System.Action OnLocalizationChanged;

        private static LocalizationSource localizationSource;
        private static Language currentLanguage = Language.English;

        public static bool IsInitialized => localizationSource != null;
        public static Language CurrentLanguage => currentLanguage;

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
                ? _data.GetLocalization(currentLanguage)
                : $"Key not found: {_key}";
        }

        public static void SetLocalizationSource(LocalizationSource _source)
        {
            if (_source == null || ReferenceEquals(localizationSource, _source))
            {
                return;
            }

            localizationSource = _source;
            localizationSource.Initialize();

            OnLocalizationChanged?.Invoke();
        }

        public static void SetLanguage(Language _language)
        {
            if (currentLanguage == _language)
            {
                return;
            }

            currentLanguage = _language;
            OnLocalizationChanged?.Invoke();
        }
    }
}
