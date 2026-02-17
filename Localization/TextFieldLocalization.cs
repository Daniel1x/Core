namespace DL.Localization
{
    using System.Text;
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(TMP_Text))]
    public class TextFieldLocalization : MonoBehaviour
    {
        [SerializeField] private TMP_Text textField = null;
        [SerializeField, LocTerm] private string key = string.Empty;
        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private string suffix = string.Empty;

        private StringBuilder sb = new StringBuilder(256);

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

            sb.Clear();

            if (!string.IsNullOrEmpty(prefix))
            {
                sb.Append(prefix);
            }

            sb.Append(LocalizationManager.GetTranslation(key));

            if (!string.IsNullOrEmpty(suffix))
            {
                sb.Append(suffix);
            }

            textField.text = sb.ToString();
        }
    }
}
