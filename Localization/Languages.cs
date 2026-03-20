namespace DL.Localization
{
    using System;
    using UnityEngine;

    public static class Languages
    {
        public static readonly int LanguageCount = System.Enum.GetNames(typeof(Language)).Length;
        public static readonly string[] LanguageNames = System.Enum.GetNames(typeof(Language));

        public enum Language
        {
            English = 0,
            Polish = 1,
        }

        public static readonly string[] LanguageTerms = new string[]
        {
            ENGLISH_LANGUAGE,
            POLISH_LANGUAGE,
        };

        public const string ENGLISH_LANGUAGE = "English";
        public const string POLISH_LANGUAGE = "Polish";

        public static Language GetSupportedLanguageFromSystem()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.English => Language.English,
                SystemLanguage.Polish => Language.Polish,
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
