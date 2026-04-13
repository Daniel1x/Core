namespace DL.Localization
{
    using System;
    using UnityEngine;

    public interface ILanguageSettingsProvider : ILanguageContainer
    {
        event Action<Language> OnLanguageChanged;
    }

    public interface ILanguageSettingsContainer : ILanguageContainer
    {
        bool LanguageFromSystemInitialized { get; set; }
    }

    public interface ILanguageContainer
    {
        Language CurrentLanguage { get; set; }
    }

    [System.Serializable]
    public class LanguageProvider : ILanguageSettingsProvider
    {
        public event Action<Language> OnLanguageChanged;

        private readonly ILanguageSettingsContainer settingsContainer;

        public LanguageProvider(ILanguageSettingsContainer _settingsContainer)
        {
            settingsContainer = _settingsContainer;

            if (settingsContainer == null)
            {
                throw new ArgumentNullException(nameof(_settingsContainer), "Language settings container cannot be null.");
            }
        }

        public Language CurrentLanguage
        {
            get => settingsContainer.CurrentLanguage;
            set
            {
                if (settingsContainer.CurrentLanguage != value)
                {
                    settingsContainer.CurrentLanguage = value;
                    OnLanguageChanged?.Invoke(value);
                }
            }
        }
    }

    public static class Languages
    {
        public static readonly int LanguageCount = System.Enum.GetNames(typeof(Language)).Length;
        public static readonly string[] LanguageNames = System.Enum.GetNames(typeof(Language));

        public static readonly string[] LanguageTerms = new string[]
        {
            ENGLISH_LANGUAGE,
            POLISH_LANGUAGE,
            GERMAN_LANGUAGE,
            SPANISH_LANGUAGE,
            FRENCH_LANGUAGE,
            ITALIAN_LANGUAGE,
            PORTUGUESE_LANGUAGE,
        };

        public const string ENGLISH_LANGUAGE = "English";
        public const string POLISH_LANGUAGE = "Polish";
        public const string GERMAN_LANGUAGE = "German";
        public const string SPANISH_LANGUAGE = "Spanish";
        public const string FRENCH_LANGUAGE = "French";
        public const string ITALIAN_LANGUAGE = "Italian";
        public const string PORTUGUESE_LANGUAGE = "Portuguese";

        public static int ToIndex(this Language _language) => (int)_language;

        public static Language GetSupportedLanguageFromSystem()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.English => Language.English,
                SystemLanguage.Polish => Language.Polish,
                SystemLanguage.German => Language.German,
                SystemLanguage.Spanish => Language.Spanish,
                SystemLanguage.French => Language.French,
                SystemLanguage.Italian => Language.Italian,
                SystemLanguage.Portuguese => Language.Portuguese,
                _ => Language.English
            };
        }

        public static string[] AdjustLanguageArraySize(this string[] _languages, bool _removeIfTooMany)
        {
            if (_languages == null || _languages.Length == 0)
            {
                return new string[LanguageCount]; // Return empty array with correct size
            }

            if (_languages.Length >= LanguageCount)
            {
                if (_removeIfTooMany)
                {
                    string[] _trimmedLanguages = new string[LanguageCount];
                    Array.Copy(_languages, _trimmedLanguages, LanguageCount);
                    return _trimmedLanguages; // Trim to the correct size
                }

                return _languages; // No adjustment needed, do not remove existing languages
            }

            string[] _adjustedLanguages = new string[LanguageCount];

            for (int i = 0; i < LanguageCount; i++)
            {
                if (i < _languages.Length)
                {
                    _adjustedLanguages[i] = _languages[i];
                }
                else
                {
                    _adjustedLanguages[i] = _languages[0]; // Fill missing entries with the first language
                }
            }

            return _adjustedLanguages;
        }
    }
}
