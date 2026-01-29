namespace DL.Localization
{
    [System.Serializable]
    public class LocalizationData
    {
        public string Key;
        public string[] Localization;

        public LocalizationData(string _key, int _languageCount)
        {
            Key = _key;
            Localization = new string[_languageCount];
        }

        public void SetLocalization(int _languageIndex, string _localizedValue)
        {
            if (_languageIndex < 0 || _languageIndex >= Localization.Length)
            {
                UnityEngine.Debug.LogError($"Language index {_languageIndex} is out of range 0-{Localization.Length} for Key: {Key}.");
                return;
            }

            Localization[_languageIndex] = _localizedValue;
        }

        public string GetLocalization(int _languageIndex)
        {
            if (_languageIndex < 0 || _languageIndex >= Localization.Length)
            {
                UnityEngine.Debug.LogError($"Language index {_languageIndex} is out of range 0-{Localization.Length} for Key: {Key}.");
                return string.Empty;
            }

            return Localization[_languageIndex];
        }
    }
}
