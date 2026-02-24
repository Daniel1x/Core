namespace DL.PlayerPrefs
{
    using UnityEngine;

    public interface IPlayerPrefsProperty<T> : IProperty<T>
    {
        public string PrefsKey { get; }
        public T Default { get; }
    }

    public interface IProperty<T>
    {
        public T Value { get; set; }
    }

    public interface IPropertyChangeEvent<T>
    {
        public event System.Action<T> OnValueChanged;
    }

    public class PlayerPrefsFloatProperty : PlayerPrefsProperty<float>
    {
        public PlayerPrefsFloatProperty(string _prefsKey, float _defaultValue) : base(_prefsKey, _defaultValue) { }
        protected sealed override void saveValue(string _key, float _value) => PlayerPrefs.SetFloat(_key, _value);
        protected sealed override float loadValue(string _key, float _defaultValue) => PlayerPrefs.GetFloat(_key, _defaultValue);
    }

    public class PlayerPrefsBoolProperty : PlayerPrefsProperty<bool>
    {
        public PlayerPrefsBoolProperty(string _prefsKey, bool _defaultValue) : base(_prefsKey, _defaultValue) { }
        protected sealed override void saveValue(string _key, bool _value) => PlayerPrefs.SetInt(_key, parse(_value));
        protected sealed override bool loadValue(string _key, bool _defaultValue) => parse(PlayerPrefs.GetInt(_key, parse(_defaultValue)));
        private int parse(bool _value) => _value ? 1 : 0;
        private bool parse(int _value) => _value == 1;
    }

    public class PlayerPrefsIntProperty : PlayerPrefsProperty<int>
    {
        public PlayerPrefsIntProperty(string _prefsKey, int _defaultValue) : base(_prefsKey, _defaultValue) { }
        protected sealed override void saveValue(string _key, int _value) => PlayerPrefs.SetInt(_key, _value);
        protected sealed override int loadValue(string _key, int _defaultValue) => PlayerPrefs.GetInt(_key, _defaultValue);
    }

    public class PlayerPrefsStringProperty : PlayerPrefsProperty<string>
    {
        public PlayerPrefsStringProperty(string _prefsKey, string _defaultValue) : base(_prefsKey, _defaultValue) { }
        protected sealed override void saveValue(string _key, string _value) => PlayerPrefs.SetString(_key, _value);
        protected sealed override string loadValue(string _key, string _defaultValue) => PlayerPrefs.GetString(_key, _defaultValue);
    }

    public abstract class PlayerPrefsProperty<T> : IPlayerPrefsProperty<T>, IPropertyChangeEvent<T>
    {
        public event System.Action<T> OnValueChanged;

        private readonly string prefsKey;
        private readonly T defaultValue;

        private T value;

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                saveValue(prefsKey, value);
                OnValueChanged?.Invoke(value);
            }
        }

        public string PrefsKey => prefsKey;
        public T Default => defaultValue;

        public PlayerPrefsProperty(string _prefsKey, T _defaultValue)
        {
            prefsKey = _prefsKey;
            defaultValue = _defaultValue;
            value = loadValue(prefsKey, defaultValue);
        }

        protected abstract void saveValue(string _key, T _value);
        protected abstract T loadValue(string _key, T _defaultValue);
    }
}
