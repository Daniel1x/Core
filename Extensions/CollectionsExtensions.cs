using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using UnityEngine;

public static class CollectionsExtensions
{
    public delegate bool FilterFunc<T>(T _filtr);
    private static System.Random random = new System.Random();

    public static bool SetActive(this UnityEngine.GameObject[] _array, bool _activeState)
    {
        bool _anyChanged = false;
        int _count = _array.Length;

        for (int i = 0; i < _count; i++)
        {
            UnityEngine.GameObject _go = _array[i];

            if (_go != null && _go.activeSelf != _activeState)
            {
                _go.SetActive(_activeState);
                _anyChanged = true;
            }
        }

        return _anyChanged;
    }

    public static bool SetActive(this List<UnityEngine.GameObject> _list, bool _activeState)
    {
        bool _anyChanged = false;
        int _count = _list.Count;

        for (int i = 0; i < _count; i++)
        {
            UnityEngine.GameObject _go = _list[i];

            if (_go != null && _go.activeSelf != _activeState)
            {
                _go.SetActive(_activeState);
                _anyChanged = true;
            }
        }

        return _anyChanged;
    }

    public static bool SetInteractable(this UnityEngine.UI.Selectable[] _array, bool _interactable)
    {
        bool _anyChanged = false;
        int _count = _array.Length;

        for (int i = 0; i < _count; i++)
        {
            UnityEngine.UI.Selectable _selectable = _array[i];

            if (_selectable != null && _selectable.interactable != _interactable)
            {
                _selectable.interactable = _interactable;
                _anyChanged = true;
            }
        }

        return _anyChanged;
    }

    public static bool SetInteractable(this List<UnityEngine.UI.Selectable> _list, bool _interactable)
    {
        bool _anyChanged = false;
        int _count = _list.Count;

        for (int i = 0; i < _count; i++)
        {
            UnityEngine.UI.Selectable _selectable = _list[i];

            if (_selectable != null && _selectable.interactable != _interactable)
            {
                _selectable.interactable = _interactable;
                _anyChanged = true;
            }
        }

        return _anyChanged;
    }

    public static bool Swap<T>(this List<T> _list, int _indexA, int _indexB, bool _showLogs = true)
    {
        if (_list.IsNullOrEmpty()
            || _indexA == _indexB
            || _indexA < 0
            || _indexB < 0
            || _indexA > _list.Count - 1
            || _indexB > _list.Count - 1)
        {
            return false;
        }

        T _temp = _list[_indexA];
        _list[_indexA] = _list[_indexB];
        _list[_indexB] = _temp;
        return true;
    }

    public static bool Swap<T>(this T[] _array, int _indexA, int _indexB)
    {
        if (_array.IsNullOrEmpty()
            || _indexA == _indexB
            || _indexA < 0
            || _indexB < 0
            || _indexA > _array.Length - 1
            || _indexB > _array.Length - 1)
        {
            return false;
        }

        T _temp = _array[_indexA];
        _array[_indexA] = _array[_indexB];
        _array[_indexB] = _temp;
        return true;
    }

    public static bool IsNullOrEmpty<T>(this List<T> _list)
    {
        return _list == null || _list.Count == 0;
    }

    public static bool IsNullOrEmpty<T>(this T[] _array)
    {
        return _array == null || _array.Length == 0;
    }

    public static bool IsNullOrEmpty<T1, T2>(this Dictionary<T1, T2> _dictionary)
    {
        return _dictionary == null || _dictionary.Count == 0;
    }

    public static bool IsIndexOutOfRange<T>(this List<T> _collection, int _index)
    {
        if (_index < 0 || _collection == null)
        {
            return true;
        }

        int _count = _collection.Count;
        return _count <= 0 || _index >= _count;
    }

    public static bool IsIndexOutOfRange<T>(this T[] _array, int _index)
    {
        if (_index < 0 || _array == null)
        {
            return true;
        }

        int _count = _array.Length;
        return _count <= 0 || _index >= _count;
    }

    public static T GetRandom<T>(this List<T> _list) => _list.GetRandom(out _);

    public static T GetRandom<T>(this List<T> _list, out int _selectedID)
    {
        int _count = _list.Count;

        if (_count > 0)
        {
            _selectedID = UnityEngine.Random.Range(0, _count);
            return _list.ElementAt(_selectedID);
        }

        _selectedID = -1;
        return default;
    }

    public static T GetRandom<T>(this T[] _array) => _array.GetRandom(out _);

    public static T GetRandom<T>(this T[] _array, out int _selectedID)
    {
        int _count = _array.Length;

        if (_count > 0)
        {
            _selectedID = UnityEngine.Random.Range(0, _count);
            return _array.ElementAt(_selectedID);
        }

        _selectedID = -1;
        return default;
    }

    public static bool ClearNullReferences<T>(this List<T> _list) where T : class
    {
        int _count = _list.Count;
        bool _anyRemoved = false;

        for (int i = _count - 1; i >= 0; i--)
        {
            if (_list[i] == null)
            {
                _list.RemoveAt(i);
                _anyRemoved = true;
            }
        }

        return _anyRemoved;
    }

    public static bool ClearNullReferences<T>(this List<T> _list, System.Predicate<T> _isNullCheck) where T : class
    {
        int _count = _list.Count;
        bool _anyRemoved = false;

        for (int i = _count - 1; i >= 0; i--)
        {
            if (_isNullCheck(_list[i]))
            {
                _list.RemoveAt(i);
                _anyRemoved = true;
            }
        }

        return _anyRemoved;
    }

    public static bool WithoutNullReferences<T>(this T[] _array, out T[] _arrayWithoutNulls) where T : class
    {
        int _count = _array.Length;
        int _nullCount = 0;

        for (int i = 0; i < _count; i++)
        {
            if (_array[i] == null)
            {
                _nullCount++;
            }
        }

        if (_nullCount == 0)
        {
            _arrayWithoutNulls = null;
            return false;
        }

        _arrayWithoutNulls = new T[_count - _nullCount];

        for (int i = 0, j = 0; i < _count; i++)
        {
            if (_array[i] == null)
            {
                continue;
            }

            _arrayWithoutNulls[j] = _array[i];
            j++;
        }

        return true;
    }

    public static bool AddIfNotNull<T>(this List<T> _list, T _item) where T : class
    {
        if (_item != null)
        {
            _list.Add(_item);
            return true;
        }

        return false;
    }

    public static bool AddIfNotContains<T>(this List<T> _list, T _element)
    {
        if (_list.Contains(_element) == false)
        {
            _list.Add(_element);
            return true;
        }

        return false;
    }

    public static bool AddRangeIfNotContains<T>(this List<T> _list, List<T> _elements)
    {
        if (_elements == null)
        {
            return false;
        }

        int _count = _elements.Count;

        if (_count <= 0)
        {
            return false;
        }

        bool _anyAdded = false;

        for (int i = 0; i < _count; i++)
        {
            if (_list.AddIfNotContains(_elements[i]))
            {
                _anyAdded = true;
            }
        }

        return _anyAdded;
    }

    public static bool RemoveRange<T>(this List<T> _list, List<T> _elements)
    {
        if (_elements == null)
        {
            return false;
        }

        int _count = _elements.Count;
        bool _anyRemoved = false;

        for (int i = 0; i < _count; i++)
        {
            if (_list.Remove(_elements[i]))
            {
                _anyRemoved = true;
            }
        }

        return _anyRemoved;
    }

    public static bool Remove<T>(this List<T> _list, System.Predicate<T> _predicate)
    {
        if (_predicate == null)
        {
            return false;
        }

        bool _anyRemoved = false;

        for (int i = _list.Count - 1; i >= 0; i--)
        {
            if (_predicate(_list[i]))
            {
                _list.RemoveAt(i);
                _anyRemoved = true;
            }
        }

        return _anyRemoved;
    }

    public static bool ContainsDuplicates<T>(this List<T> _collection)
    {
        int _count = _collection.Count;

        for (int i = 0; i < _count; i++)
        {
            for (int j = i + 1; j < _count; j++)
            {
                if (_collection[i].Equals(_collection[j]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool ContainsDuplicates<T>(this T[] _array)
    {
        int _count = _array.Count();

        for (int i = 0; i < _count; i++)
        {
            for (int j = i + 1; j < _count; j++)
            {
                if (_array[i].Equals(_array[j]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static T[] WithoutDuplicates<T>(this T[] _array)
    {
        return _array.Distinct().ToArray();
    }

    public static List<T> WithoutDuplicates<T>(this List<T> _list)
    {
        return _list.Distinct().ToList();
    }

    public static bool Contains<T>(this T[] _array, T _value)
    {
        int _count = _array.Length;

        for (int i = 0; i < _count; i++)
        {
            if (_array[i] != null && _array[i].Equals(_value))
            {
                return true;
            }
        }

        return false;
    }

    public static bool ContainsNull<T>(this T[] _array)
    {
        int _count = _array.Length;

        for (int i = 0; i < _count; i++)
        {
            if (_array[i] == null)
            {
                return true;
            }
        }

        return false;
    }

    public static bool ContainsNull<T>(this List<T> _list)
    {
        int _count = _list.Count;

        for (int i = 0; i < _count; i++)
        {
            if (_list[i] == null)
            {
                return true;
            }
        }

        return false;
    }

    public static T GetFirst<T>(this IEnumerable<T> _collection)
    {
        if (_collection == null || _collection.Count() <= 0)
        {
            return default;
        }

        return _collection.First();
    }

    public static T GetLast<T>(this IEnumerable<T> _collection)
    {
        if (_collection == null || _collection.Count() <= 0)
        {
            return default;
        }

        return _collection.Last();
    }

    public static int GetClampedIndex(int _newIndex, int _collectionSize, bool _loop = true)
    {
        if (_loop == false)
        {
            return Mathf.Clamp(_newIndex, 0, _collectionSize - 1);
        }

        if (_newIndex < 0)
        {
            return _collectionSize - 1;
        }
        else if (_newIndex > _collectionSize - 1)
        {
            return 0;
        }
        else
        {
            return _newIndex;
        }
    }

    public static int GetClampedIndex<T>(this List<T> _list, int _newIndex, bool _loop = true)
    {
        return GetClampedIndex(_newIndex, _list.Count, _loop);
    }

    public static int GetClampedIndex<T>(this T[] _array, int _newIndex, bool _loop = true)
    {
        return GetClampedIndex(_newIndex, _array.Length, _loop);
    }

    public static void InsertItemAndSlideRestDown<T>(this T[] _array, T _item, int _id, out T _itemRemovedFromEnd)
    {
        if (_array == null)
        {
            _itemRemovedFromEnd = default;
            return;
        }

        int _length = _array.Length;

        if (_length <= 0 || _id >= _length || _id < 0)
        {
            _itemRemovedFromEnd = default;
            return;
        }

        _itemRemovedFromEnd = _array[_length - 1];

        for (int i = _length - 1; i > _id; i--)
        {
            _array[i] = _array[i - 1];
        }

        _array[_id] = _item;
    }

    public static void RemoveNullReferences<T>(this List<T> _list) where T : Object
    {
        if (_list == null)
        {
            return;
        }

        for (int i = _list.Count - 1; i >= 0; i--)
        {
            if (_list[i] == null)
            {
                _list.RemoveAt(i);
            }
        }
    }

    public static void RemoveNullReferences(this List<object> _list)
    {
        if (_list == null)
        {
            return;
        }

        for (int i = _list.Count - 1; i >= 0; i--)
        {
            if (_list[i] == null)
            {
                _list.RemoveAt(i);
            }
        }
    }

    public static void RemoveNullOrDisabled(this List<GameObject> _list)
    {
        if (_list == null)
        {
            return;
        }

        for (int i = _list.Count - 1; i >= 0; i--)
        {
            if (_list[i] == null || _list[i].activeSelf == false)
            {
                _list.RemoveAt(i);
            }
        }
    }

    public static void RemoveNullOrDisabledInHierarchy(this List<GameObject> _list)
    {
        if (_list == null)
        {
            return;
        }

        for (int i = _list.Count - 1; i >= 0; i--)
        {
            if (_list[i] == null || _list[i].activeInHierarchy == false)
            {
                _list.RemoveAt(i);
            }
        }
    }

    public static int IndexOf<T>(this T[] _array, T _item)
    {
        for (int i = 0; i < _array.Length; i++)
        {
            if (_array[i].Equals(_item))
            {
                return i;
            }
        }

        return -1;
    }

    public static T[] AddToArray<T>(this T[] _array, T _item)
    {
        T[] _tmpArray = new T[_array.Length + 1];

        for (int i = 0; i < _array.Length; i++)
        {
            _tmpArray[i] = _array[i];
        }

        _tmpArray[_tmpArray.Length - 1] = _item;

        return _tmpArray;
    }

    public static T[] AddToArray<T>(this T[] _array, params T[] _items)
    {
        int _itemsCount = _items != null ? _items.Length : 0;

        if (_itemsCount == 0)
        {
            return _array;
        }

        int _currentLength = _array.Length;
        T[] _tmpArray = new T[_currentLength + _itemsCount];

        for (int i = 0; i < _currentLength; i++)
        {
            _tmpArray[i] = _array[i];
        }

        for (int i = 0; i < _itemsCount; i++)
        {
            _tmpArray[_currentLength + i] = _items[i];
        }

        return _tmpArray;
    }

    public static T[] AddToArray<T>(this T[] _array, List<T> _listToAdd)
    {
        int _itemsCount = _listToAdd != null ? _listToAdd.Count : 0;

        if (_itemsCount == 0)
        {
            return _array;
        }

        int _currentLength = _array.Length;
        T[] _tmpArray = new T[_currentLength + _itemsCount];

        for (int i = 0; i < _currentLength; i++)
        {
            _tmpArray[i] = _array[i];
        }

        for (int i = 0; i < _itemsCount; i++)
        {
            _tmpArray[_currentLength + i] = _listToAdd[i];
        }

        return _tmpArray;
    }

    public static T[] Extend<T>(this T[] _array, int _slots, T _defaultValue = default)
    {
        if (_slots <= 0)
        {
            return _array;
        }

        int _currentLength = _array.Length;
        T[] _tmpArray = new T[_currentLength + _slots];

        for (int i = 0; i < _currentLength; i++)
        {
            _tmpArray[i] = _array[i];
        }

        for (int i = 0; i < _slots; i++)
        {
            _tmpArray[_currentLength + i] = _defaultValue;
        }

        return _tmpArray;
    }

    public static List<T> Extend<T>(this List<T> _list, int _slots, T _defaultValue = default)
    {
        _list.Capacity += _slots;

        for (int i = 0; i < _slots; i++)
        {
            _list.Add(_defaultValue);
        }

        return _list;
    }

    public static List<T> Extend<T>(this List<T> _collection, IEnumerable<T> _toAdd)
    {
        _collection.AddRange(_toAdd);
        return _collection;
    }

    public static T[] RemoveFromArray<T>(this T[] _array, T _item, bool _safe = false)
    {
        if (_safe)
        {
            bool _foundMatch = false;

            for (int i = 0; i < _array.Length; i++)
            {
                if (_array[i].Equals(_item))
                {
                    _foundMatch = true;
                    break;
                }
            }

            if (_foundMatch == false)
            {
                return _array;
            }
        }

        T[] _tmpArray = new T[_array.Length - 1];
        int _addingIndex = 0;

        for (int i = 0; i < _array.Length; i++)
        {
            if (_array[i].Equals(_item))
            {
                continue;
            }

            _tmpArray[_addingIndex] = _array[i];
            _addingIndex++;
        }

        return _tmpArray;
    }

    public static T[] RemoveAtIndex<T>(this T[] _array, int _index)
    {
        T[] _tmpArray = new T[_array.Length - 1];
        int _originalIndex = 0;

        for (int i = 0; i < _array.Length; i++)
        {
            if (i == _index)
            {
                continue;
            }

            _tmpArray[_originalIndex] = _array[i];
            _originalIndex++;
        }

        return _tmpArray;
    }

    public static T[] RemoveLast<T>(this T[] _array)
    {
        int _length = _array.Length;

        if (_length > 0)
        {
            return _array.RemoveAtIndex(_length - 1);
        }

        return _array;
    }

    public static T[] RemoveLast<T>(this T[] _array, int _count)
    {
        if (_count <= 0)
        {
            return _array;
        }

        int _length = _array.Length;
        int _newLength = _length - _count;

        if (_newLength <= 0)
        {
            return new T[] { };
        }

        T[] _newArray = new T[_newLength];

        for (int i = 0; i < _newLength; i++)
        {
            _newArray[i] = _array[i];
        }

        return _newArray;
    }

    public static void InsertIntoSpotAndMoveItemsUp<T>(this T[] _array, T _item, int _id, out T _itemRemovedFromTop)
    {
        if (_array == null)
        {
            _itemRemovedFromTop = default;
            return;
        }

        int _length = _array.Length;

        if (_length <= 0 || _id >= _length || _id < 0)
        {
            _itemRemovedFromTop = default;
            return;
        }

        _itemRemovedFromTop = _array[0];

        for (int i = 0; i < _id; i++)
        {
            _array[i] = _array[i + 1];
        }

        _array[_id] = _item;
    }

    public static void InsertIntoSpotAndMoveItemsUp(this NativeArray<Vector3> _array, Vector3 _item, int _id, out Vector3 _itemRemovedFromTop)
    {
        if (_array == null)
        {
            _itemRemovedFromTop = default;
            return;
        }

        int _length = _array.Length;

        if (_length <= 0 || _id >= _length || _id < 0)
        {
            _itemRemovedFromTop = default;
            return;
        }

        _itemRemovedFromTop = _array[0];

        for (int i = 0; i < _id; i++)
        {
            _array[i] = _array[i + 1];
        }

        _array[_id] = _item;
    }

    public static void Shuffle<T>(this IList<T> _list)
    {
        int _count = _list.Count;

        while (_count > 1)
        {
            int _rand = random.Next(_count--);

            if (_rand != _count)
            {
                T _temp = _list[_rand];
                _list[_rand] = _list[_count];
                _list[_count] = _temp;
            }
        }
    }

    public static void ShuffleRange<T>(this List<T> _list, int _startID, int _endID)
    {
        if (_list == null)
        {
            return;
        }

        int _count = _list.Count;

        if (_count <= 1 || _startID == _endID) //Nothing to suffle
        {
            return;
        }

        if (_startID > _endID)
        {
            int _temp = _startID;
            _startID = _endID;
            _endID = _temp;
        }

        if (_startID < 0)
        {
            _startID = 0;
        }

        if (_endID >= _count)
        {
            _endID = _count - 1;
        }

        int _valuesToShuffleCount = (_endID - _startID) + 1;

        if (_valuesToShuffleCount == 2) //Do not shuffle, just swap?
        {
            if (random.Next(0, 1) == 0) //Should swap?
            {
                _list.Swap(_startID, _endID, false);
            }

            return;
        }

        List<int> _shuffledIDs = new List<int>(_valuesToShuffleCount);

        for (int i = 0; i < _valuesToShuffleCount; i++)
        {
            _shuffledIDs.Add(_startID + i);
        }

        _shuffledIDs.Shuffle();

        for (int i = 0; i < _valuesToShuffleCount; i++)
        {
            _list.Swap(_startID + i, _shuffledIDs[i], false);
        }
    }

    public static string ToStringByElements<T>(this IEnumerable<T> _collection, System.Func<T, string> _elementInfoCreator = null, string _separator = ", ", bool _printIDs = false, bool _simple = true)
    {
        if (_elementInfoCreator == null)
        {
            _elementInfoCreator = _obj => _obj.ToString();
        }

        StringBuilder _builder = new StringBuilder();

        if (_simple == false)
        {
            _builder.Append($"Collection of type {typeof(T).Name} ");
        }

        int _count = _collection.Count();

        if (_count > 0)
        {
            if (_simple == false)
            {
                _builder.Append($", Items({_count}): ");
            }

            for (int i = 0; i < _count; i++)
            {
                T _item = _collection.ElementAt(i);

                if (_item == null)
                {
                    continue;
                }

                if (_printIDs)
                {
                    _builder.Append($"[{i}] ");
                }

                _builder.Append(_elementInfoCreator(_item));

                if (i < _count - 1)
                {
                    _builder.Append(_separator);
                }
            }
        }

        return _builder.ToString();
    }

    public static T[] Filter<T>(this T[] _collection, FilterFunc<T> _filter)
    {
        List<T> _list = new();

        for (int i = 0; i < _collection.Length; i++)
        {
            if (_filter(_collection[i]))
            {
                _list.Add(_collection[i]);
            }
        }

        return _list.ToArray();
    }

    public static List<T> Filter<T>(this List<T> _collection, FilterFunc<T> _filter)
    {
        List<T> _list = new();

        for (int i = 0; i < _collection.Count; i++)
        {
            if (_filter(_collection[i]))
            {
                _list.Add(_collection[i]);
            }
        }

        return _list;
    }

    public static bool AnyMatch<T>(this List<T> _list, System.Predicate<T> _predicate)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (_predicate(_list[i]))
            {
                return true;
            }
        }

        return false;
    }

    public static bool AnyMatch<T>(this List<T> _list, System.Predicate<T> _predicate, out T _match)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (_predicate(_list[i]))
            {
                _match = _list[i];
                return true;
            }
        }

        _match = default;
        return false;
    }
}
