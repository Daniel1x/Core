namespace DL.Localization
{
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(TMP_Text))]
    public class TextFieldLocalization : MonoBehaviour
    {
        [SerializeField] private TMP_Text textField = null;
        [SerializeField, LocTerm] private string key = string.Empty;

        private void OnEnable()
        {
            UpdateLocalizedText();
            LocalizationManager.OnLocalizationChanged += UpdateLocalizedText;
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocalizationChanged -= UpdateLocalizedText;
        }

        private void Reset()
        {
            textField = GetComponent<TMP_Text>();
        }

        public void UpdateLocalizedText()
        {
            if (textField == null)
            {
                textField = GetComponent<TMP_Text>();
            }

            textField.text = LocalizationManager.GetTranslation(key);
        }
    }
}
