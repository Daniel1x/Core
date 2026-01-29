namespace DL.Localization
{
    using UnityEngine;

    [System.Serializable]
    public class LanguageList
    {
        public event System.Action OnLanguageChanged;

        [SerializeField] private int selectedLanguageIndex = 0;
        [SerializeField] private string[] languageTerms = Languages.LanguageNames;

        public int SelectedLanguageIndex
        {
            get => selectedLanguageIndex;
            set
            {
                value = Mathf.Clamp(value, 0, languageTerms.Length - 1);

                if (selectedLanguageIndex != value)
                {
                    OnLanguageChanged?.Invoke();
                }
            }
        }

        public string SelectedLanguage
        {
            get => languageTerms[selectedLanguageIndex];
            set
            {
                int _index = System.Array.IndexOf(languageTerms, value);

                if (_index >= 0)
                {
                    SelectedLanguageIndex = _index;
                }
            }
        }

        public string[] LanguageTerms => languageTerms;
    }
}
