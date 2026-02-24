namespace DL.Core.Maze
{
    using System;
    using System.Collections.Generic;

    public interface ISelectionGroup<TValue>
    {
        public event Action OnSelectionChanged;
        public TValue GetSelected();
    }

    public interface ISelectionKeyContainer<TKey>
    {
        public TKey Key { get; }
    }

    public interface ISelectionGroup<TKey, TValue> : ISelectionGroup<TValue> where TValue : ISelectionKeyContainer<TKey>
    {
        public event Action OnItemListChanged;

        public TKey SelectedKey { get; set; }

        public IReadOnlyList<TKey> GetAvailableKeys();
        public TValue Get(TKey _key);
        public void SelectRandom();
        public void SetNext(bool _loop = true);
        public void SetPrevious(bool _loop = true);

        public void Register(TValue _item, bool _setAsSelected = false);
    }

    [Serializable]
    public abstract class DefaultSelectionGroup<TKey, TValue> : ISelectionGroup<TKey, TValue>
        where TValue : ISelectionKeyContainer<TKey>
    {
        public event Action OnSelectionChanged;
        public event Action OnItemListChanged;

        protected readonly Dictionary<TKey, TValue> dictionary;
        protected readonly List<TKey> keys;
        protected readonly ISelectionGroup<TKey> selector;
        protected TKey selectedKey;

        public TKey SelectedKey
        {
            get => selectedKey;
            set
            {
                if (value == null || value.Equals(selectedKey))
                {
                    return;
                }

                if (dictionary.ContainsKey(value))
                {
                    selectedKey = value;
                    OnSelectionChanged?.Invoke();
                }
            }
        }

        public DefaultSelectionGroup(int _initialCapacity, ISelectionGroup<TKey> _selector = null)
        {
            dictionary = new Dictionary<TKey, TValue>(_initialCapacity);
            keys = new List<TKey>(_initialCapacity);
            selector = _selector;

            if (selector != null)
            {
                selector.OnSelectionChanged += onExternalSelectionChanged;
            }
        }

        ~DefaultSelectionGroup()
        {
            if (selector != null)
            {
                selector.OnSelectionChanged -= onExternalSelectionChanged;
            }
        }

        private void onExternalSelectionChanged()
        {
            if (selector != null)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Log($"Selector: Detected external selection change. Updating selected key to match external selector's selected key: {selector.GetSelected()}");
#endif
                SelectedKey = selector.GetSelected();
            }
        }

        public TValue Get(TKey _key)
        {
            if (dictionary.TryGetValue(_key, out TValue _value))
            {
                return _value;
            }

            return default;
        }

        public TValue GetSelected()
        {
            if (dictionary.TryGetValue(selectedKey, out TValue _value))
            {
                return _value;
            }

            return default;
        }

        public IReadOnlyList<TKey> GetAvailableKeys()
        {
            return keys;
        }

        public void SelectRandom()
        {
            if (keys.Count > 0)
            {
                SelectedKey = keys.GetRandom();
            }
        }

        public void SetNext(bool _loop = true)
        {
            int _currentIndex = keys.IndexOf(selectedKey);

            if (_currentIndex == -1)
            {
                return;
            }

            int _nextIndex = _currentIndex + 1;

            if (_nextIndex >= keys.Count)
            {
                if (!_loop)
                {
                    return;
                }

                _nextIndex = 0;
            }

            SelectedKey = keys[_nextIndex];
        }

        public void SetPrevious(bool _loop = true)
        {
            int _currentIndex = keys.IndexOf(selectedKey);

            if (_currentIndex == -1)
            {
                return;
            }

            int _previousIndex = _currentIndex - 1;

            if (_previousIndex < 0)
            {
                if (!_loop)
                {
                    return;
                }

                _previousIndex = keys.Count - 1;
            }

            SelectedKey = keys[_previousIndex];
        }

        public void Register(TValue _object, bool _setAsSelected = false)
        {
            if (_object == null)
            {
                throw new ArgumentNullException(nameof(_object), "Cannot register a null object.");
            }

            TKey _key = _object.Key;

            if (_key == null || dictionary.ContainsKey(_key))
            {
                throw new ArgumentException($"An object with the key '{_key}' is already registered.", nameof(_object));
            }

            keys.Add(_key);
            dictionary[_key] = _object;

            OnItemListChanged?.Invoke();

            if (_setAsSelected || selectedKey == null)
            {
                SelectedKey = _key;
            }
        }
    }
}
