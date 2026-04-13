namespace DL.Localization
{
    using System.Linq;
    using System.Text;
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(TMP_Text))]
    public class LocalizationTestTextField : MonoBehaviour
    {
        [SerializeField] private LocalizationSource source = null;

        private TMP_Text textField;

        [ContextMenu("Set Unique Characters")]
        public void SetUniqueCharacters()
        {
            if (source == null)
            {
                Debug.LogError("Localization source is not set.");
                return;
            }

            if (textField == null)
            {
                textField = GetComponent<TMP_Text>();
            }

            var _uniqueChars = source.GetAllUniqueCharacters().ToList();
            _uniqueChars.Sort((a, b) => a.CompareTo(b));
            var _sb = new StringBuilder(_uniqueChars.Count);

            foreach (char _c in _uniqueChars)
            {
                _sb.Append(_c);
            }

            textField.text = _sb.ToString();
            _sb.Clear();

            Debug.Log($"Set text field to unique characters from localization source. Total unique characters: {_uniqueChars.Count}\nCharacters: {textField.text}");
        }
    }
}
