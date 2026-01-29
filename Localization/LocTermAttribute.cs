namespace DL.Localization
{
    using UnityEngine;

    public class LocTermAttribute : PropertyAttribute
    {
        public string Filter { get; private set; }

        public LocTermAttribute(string _filter = "")
        {
            Filter = _filter;
        }
    }
}
