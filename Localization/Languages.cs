namespace DL.Localization
{
    using UnityEngine;

    public static class Languages
    {
        public static readonly int LanguageCount = System.Enum.GetNames(typeof(Language)).Length;
        public static readonly string[] LanguageNames = System.Enum.GetNames(typeof(Language));

        public enum Language
        {
            English = 0,
            Polish = 1,
            German = 2,
            French = 3,
            Japanese = 4,
            Spanish = 5,
            Italian = 6,
            Chinese = 7,
            Portuguese = 8,
        }

        public static readonly string[] LanguageTerms = new string[]
        {
            ENGLISH_LANGUAGE,
            POLISH_LANGUAGE,
            GERMAN_LANGUAGE,
            FRENCH_LANGUAGE,
            JAPANESE_LANGUAGE,
            SPANISH_LANGUAGE,
            ITALIAN_LANGUAGE,
            CHINESE_LANGUAGE,
            PORTUGUESE_LANGUAGE,
        };

        public const string ENGLISH_LANGUAGE = "English";
        public const string POLISH_LANGUAGE = "Polish";
        public const string JAPANESE_LANGUAGE = "Japanese";
        public const string GERMAN_LANGUAGE = "German";
        public const string SPANISH_LANGUAGE = "Spanish";
        public const string ITALIAN_LANGUAGE = "Italian";
        public const string FRENCH_LANGUAGE = "French";
        public const string CHINESE_LANGUAGE = "Chinese";
        public const string PORTUGUESE_LANGUAGE = "Portuguese";

        public static Language GetSupportedLanguageFromSystem()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.English => Language.English,
                SystemLanguage.Polish => Language.Polish,
                SystemLanguage.Japanese => Language.Japanese,
                SystemLanguage.German => Language.German,
                SystemLanguage.Spanish => Language.Spanish,
                SystemLanguage.Italian => Language.Italian,
                SystemLanguage.French => Language.French,
                SystemLanguage.Chinese => Language.Chinese,
                SystemLanguage.ChineseSimplified => Language.Chinese,
                SystemLanguage.ChineseTraditional => Language.Chinese,
                SystemLanguage.Portuguese => Language.Portuguese,
                _ => Language.English
            };
        }

        public static string[] AdjustLanguageArraySize(this string[] _languages)
        {
            if (_languages == null || _languages.Length == 0)
            {
                return new string[LanguageCount]; // Return empty array with correct size
            }

            if (_languages.Length >= LanguageCount)
            {
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
