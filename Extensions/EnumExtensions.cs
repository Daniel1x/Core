namespace DL.Enum
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine.Events;

    public static class EnumExtensions
    {
        public class EnumInfo<EnumType> where EnumType : System.Enum
        {
            public readonly EnumType[] Values = null;
            public readonly int[] IntValues = null;
            public readonly int Count = -1;
            public readonly bool ValueIsMatchingID = false;
            public readonly bool HasDefaultAtMinusOne = false;
            public readonly bool ValuesMatchIndexesWithDefaultAtMinusOne = false;

            public EnumType this[int _index] => Values[_index];

            public EnumInfo()
            {
                Values = GetEnumValues<EnumType>();
                IntValues = GetEnumIntValues<EnumType>();
                Count = Values.Length;
                ValueIsMatchingID = AreEnumIntValuesMatchingID<EnumType>(IntValues);
                HasDefaultAtMinusOne = EnumHasOnlyOneNegativeValueAtMinusOne<EnumType>(IntValues);
                ValuesMatchIndexesWithDefaultAtMinusOne = EnumIntValuesMatchIndexesWithDefaultAtMinusOne<EnumType>(IntValues);
            }

            public int GetEnumID(EnumType _enum)
            {
                int _enumIntValue = _enum.ToInt();

                if (ValueIsMatchingID)
                {
                    return _enumIntValue;
                }

                if (ValuesMatchIndexesWithDefaultAtMinusOne)
                {
                    if (_enumIntValue >= 0)
                    {
                        return _enumIntValue;
                    }

                    return Count - 1;
                }

                if (_enumIntValue >= 0)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (Values[i].Equals(_enum))
                        {
                            return i;
                        }
                    }
                }
                else
                {
                    if (HasDefaultAtMinusOne)
                    {
                        return Count - 1;
                    }

                    for (int i = Count - 1; i >= 0; i--)
                    {
                        if (Values[i].Equals(_enum))
                        {
                            return i;
                        }
                    }
                }

                return -1;
            }

            public bool IsPreviousValueAvailable(EnumType _currentValue)
            {
                return GetEnumID(_currentValue) > 0;
            }

            public bool IsNextValueAvailable(EnumType _currentValue)
            {
                return GetEnumID(_currentValue) < Count - 1;
            }

            public EnumType GetNextValue(EnumType _currentValue, bool _loop = true)
            {
                int _currentEnumID = GetEnumID(_currentValue);

                return _currentEnumID < Count - 1
                    ? Values[_currentEnumID + 1]
                    : _loop ? GetFirstValue() : _currentValue;
            }

            public EnumType GetPreviousValue(EnumType _currentValue, bool _loop = true)
            {
                int _currentEnumID = GetEnumID(_currentValue);

                return _currentEnumID > 0
                    ? Values[_currentEnumID - 1]
                    : _loop ? GetLastValue() : _currentValue;
            }

            public EnumType GetAdjacentValue(EnumType _currentValue, bool _next = true, bool _loop = true)
            {
                return _next ? GetNextValue(_currentValue, _loop) : GetPreviousValue(_currentValue, _loop);
            }

            public EnumType GetFirstValue()
            {
                return Values[0];
            }

            public EnumType GetLastValue()
            {
                return Values[Count - 1];
            }

            public void PerformActionForEachEnumValue(UnityAction<EnumType> _action)
            {
                for (int i = 0; i < Count; i++)
                {
                    _action(Values[i]);
                }
            }
        }

        public class EnumExtendedInfo<EnumType> : EnumInfo<EnumType> where EnumType : System.Enum
        {
            public readonly string EnumOrder = null;
            public readonly string[] EnumNames = null;

            public EnumExtendedInfo() : base()
            {
                EnumOrder = GetEnumOrder(Values);
                EnumNames = GetEnumNames<EnumType>();
            }
        }

        public class EnumFlagExtendedInfo<EnumType> : EnumExtendedInfo<EnumType> where EnumType : System.Enum
        {
            public readonly List<EnumType> UniqueFlags = null;
            public readonly int UniqueFlagsCount = -1;

            public EnumFlagExtendedInfo() : base()
            {
                UniqueFlags = GetUniqueFlags<EnumType>();
                UniqueFlagsCount = UniqueFlags.Count;
            }

            public bool IsUniqueFlag(EnumType _type)
            {
                int _flag = _type.ToInt();

                if (_flag == 0)
                {
                    return false;
                }

                int _shareCount = 0;

                for (int i = 0; i < Count; i++)
                {
                    int _testFlag = IntValues[i];

                    if (_testFlag <= 0)
                    {
                        continue;
                    }

                    if ((_flag & _testFlag) > 0 && _flag >= _testFlag)
                    {
                        _shareCount++;
                    }
                }

                return _shareCount == 1;
            }

            public List<EnumType> DisassembleFlag(EnumType _flag)
            {
                List<EnumType> _dissasembled = new List<EnumType>();

                for (int i = 0; i < UniqueFlagsCount; i++)
                {
                    if ((UniqueFlags[i].ToInt() & _flag.ToInt()) != 0)
                    {
                        _dissasembled.Add(UniqueFlags[i]);
                    }
                }

                return _dissasembled;
            }
        }

        public static System.Array GetEnumArray<EnumType>() where EnumType : System.Enum
        {
            return System.Enum.GetValues(typeof(EnumType));
        }

        public static int GetEnumCount<EnumType>() where EnumType : System.Enum
        {
            return GetEnumArray<EnumType>().Length;
        }

        public static EnumType[] GetEnumValues<EnumType>() where EnumType : System.Enum
        {
            return (EnumType[])GetEnumArray<EnumType>();
        }

        public static int[] GetEnumIntValues<EnumType>() where EnumType : System.Enum
        {
            return (int[])GetEnumArray<EnumType>();
        }

        public static string[] GetEnumNames<T>() where T : System.Enum
        {
            return System.Enum.GetNames(typeof(T));
        }

        public static bool EnumIntValuesMatchIndexes<EnumType>() where EnumType : System.Enum
        {
            return AreEnumIntValuesMatchingID<EnumType>(GetEnumIntValues<EnumType>());
        }

        public static bool AreEnumIntValuesMatchingID<EnumType>(int[] _values) where EnumType : System.Enum
        {
            int _count = _values.Length;

            for (int i = 0; i < _count; i++)
            {
                if (_values[i] != i)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool EnumIntValuesMatchIndexesWithDefaultAtMinusOne<EnumType>() where EnumType : System.Enum
        {
            return EnumIntValuesMatchIndexesWithDefaultAtMinusOne<EnumType>(GetEnumIntValues<EnumType>());
        }

        public static bool EnumIntValuesMatchIndexesWithDefaultAtMinusOne<EnumType>(int[] _values) where EnumType : System.Enum
        {
            int _count = _values.Length;

            for (int i = 0; i < _count; i++)
            {
                if (i == _count - 1)
                {
                    if (_values[i] == -1)
                    {
                        return true;
                    }
                }
                else if (_values[i] != i)
                {
                    return false;
                }
            }

            return false;
        }

        public static bool EnumHasOnlyOneNegativeValueAtMinusOne<EnumType>() where EnumType : System.Enum
        {
            return EnumHasOnlyOneNegativeValueAtMinusOne<EnumType>(GetEnumIntValues<EnumType>());
        }

        public static bool EnumHasOnlyOneNegativeValueAtMinusOne<EnumType>(int[] _values) where EnumType : System.Enum
        {
            int _count = _values.Length;
            int _negativeCount = 0;
            bool _minusOneFound = false;

            //Negatives are at the end of array
            for (int i = _count - 1; i >= 0; i--)
            {
                if (_values[i] < 0)
                {
                    _negativeCount++;

                    if (_negativeCount > 1)
                    {
                        return false;
                    }

                    if (_values[i] == -1)
                    {
                        _minusOneFound = true;
                    }
                }
                else
                {
                    return _minusOneFound && _negativeCount == 1;
                }
            }

            return _minusOneFound && _negativeCount == 1;
        }

        public static System.Array GetEnumArray<EnumType>(this EnumType _instance) where EnumType : System.Enum
        {
            return System.Enum.GetValues(typeof(EnumType));
        }

        public static int GetEnumCount<EnumType>(this EnumType _instance) where EnumType : System.Enum
        {
            return GetEnumArray<EnumType>().Length;
        }

        public static EnumType[] GetEnumValues<EnumType>(this EnumType _instance) where EnumType : System.Enum
        {
            return (EnumType[])GetEnumArray<EnumType>();
        }

        public static string[] GetEnumNames<EnumType>(this EnumType _instance) where EnumType : System.Enum
        {
            return GetEnumNames<EnumType>();
        }

        public static int[] GetEnumIntValues<EnumType>(this EnumType _instance) where EnumType : System.Enum
        {
            return (int[])GetEnumArray<EnumType>();
        }

        public static bool EnumIntValuesMatchIndexes<EnumType>(this EnumType _instance) where EnumType : System.Enum
        {
            return EnumIntValuesMatchIndexes<EnumType>();
        }

        public static EnumType[] GetPartOfEnumValues<EnumType>(params EnumType[] _excludedValues) where EnumType : System.Enum
        {
            List<EnumType> _list = GetEnumValues<EnumType>().ToList();
            int _excludedCount = _excludedValues.Length;

            for (int i = 0; i < _excludedCount; i++)
            {
                _list.Remove(_excludedValues[i]);
            }

            return _list.ToArray();
        }

        public static string GetEnumOrder<EnumType>() where EnumType : System.Enum
        {
            return GetEnumOrder(GetEnumValues<EnumType>());
        }

        public static string GetEnumOrder<EnumType>(EnumType[] _values) where EnumType : System.Enum
        {
            int _count = _values.Length;

            if (_count <= 0)
            {
                return "";
            }

            string _text = _values[0].ToString();

            for (int i = 1; i < _count; i++)
            {
                _text += $", {_values[i]}";
            }

            return _text;
        }

        public static void PerformActionForEachEnumValue<EnumType>(UnityAction<EnumType> _action) where EnumType : System.Enum
        {
            EnumType[] _values = GetEnumValues<EnumType>();
            int _count = _values.Length;

            for (int i = 0; i < _count; i++)
            {
                _action(_values[i]);
            }
        }

        public static void PerformActionForEachEnumValue<E1, E2>(UnityAction<E1, E2> _action)
            where E1 : System.Enum
            where E2 : System.Enum
        {
            PerformActionForEachEnumValue<E1>((_t1) => PerformActionForEachEnumValue<E2>((_t2) => _action(_t1, _t2)));
        }

        public static void PerformActionForEachEnumValue<E1, E2, E3>(UnityAction<E1, E2, E3> _action)
            where E1 : System.Enum
            where E2 : System.Enum
            where E3 : System.Enum
        {
            PerformActionForEachEnumValue<E1, E2>((_t1, _t2) => PerformActionForEachEnumValue<E3>((_t3) => _action(_t1, _t2, _t3)));
        }

        public static void PerformActionForEachEnumValue<E1, E2, E3, E4>(UnityAction<E1, E2, E3, E4> _action)
            where E1 : System.Enum
            where E2 : System.Enum
            where E3 : System.Enum
            where E4 : System.Enum
        {
            PerformActionForEachEnumValue<E1, E2, E3>((_t1, _t2, _t3) => PerformActionForEachEnumValue<E4>((_t4) => _action(_t1, _t2, _t3, _t4)));
        }

        public static void PerformActionForEachEnumValue<E1, E2, E3, E4, E5>(System.Action<E1, E2, E3, E4, E5> _action)
            where E1 : System.Enum
            where E2 : System.Enum
            where E3 : System.Enum
            where E4 : System.Enum
            where E5 : System.Enum
        {
            PerformActionForEachEnumValue<E1, E2, E3, E4>((_t1, _t2, _t3, _t4) => PerformActionForEachEnumValue<E5>((_t5) => _action(_t1, _t2, _t3, _t4, _t5)));
        }

        public static void PerformActionForEachEnumValue<E1, E2, E3, E4, E5, E6>(System.Action<E1, E2, E3, E4, E5, E6> _action)
            where E1 : System.Enum
            where E2 : System.Enum
            where E3 : System.Enum
            where E4 : System.Enum
            where E5 : System.Enum
            where E6 : System.Enum
        {
            PerformActionForEachEnumValue<E1, E2, E3, E4, E5>((_t1, _t2, _t3, _t4, _t5) => PerformActionForEachEnumValue<E6>((_t6) => _action(_t1, _t2, _t3, _t4, _t5, _t6)));
        }

        public static void PerformActionForEachEnumValue<E1, E2, E3, E4, E5, E6, E7>(System.Action<E1, E2, E3, E4, E5, E6, E7> _action)
            where E1 : System.Enum
            where E2 : System.Enum
            where E3 : System.Enum
            where E4 : System.Enum
            where E5 : System.Enum
            where E6 : System.Enum
            where E7 : System.Enum
        {
            PerformActionForEachEnumValue<E1, E2, E3, E4, E5, E6>((_t1, _t2, _t3, _t4, _t5, _t6) => PerformActionForEachEnumValue<E7>((_t7) => _action(_t1, _t2, _t3, _t4, _t5, _t6, _t7)));
        }

        public static void PerformActionForEachEnumValue<E1, E2, E3, E4, E5, E6, E7, E8>(System.Action<E1, E2, E3, E4, E5, E6, E7, E8> _action)
            where E1 : System.Enum
            where E2 : System.Enum
            where E3 : System.Enum
            where E4 : System.Enum
            where E5 : System.Enum
            where E6 : System.Enum
            where E7 : System.Enum
            where E8 : System.Enum
        {
            PerformActionForEachEnumValue<E1, E2, E3, E4, E5, E6, E7>((_t1, _t2, _t3, _t4, _t5, _t6, _t7) => PerformActionForEachEnumValue<E8>((_t8) => _action(_t1, _t2, _t3, _t4, _t5, _t6, _t7, _t8)));
        }

        public static EnumType GetNextValue<EnumType>(this EnumType _type, bool _loop = true) where EnumType : System.Enum
        {
            return new EnumInfo<EnumType>().GetNextValue(_type, _loop);
        }

        public static EnumType GetPreviousValue<EnumType>(this EnumType _type, bool _loop = true) where EnumType : System.Enum
        {
            return new EnumInfo<EnumType>().GetPreviousValue(_type, _loop);
        }

        public static EnumType GetAdjacentValue<EnumType>(this EnumType _type, bool _next, bool _loop = true) where EnumType : System.Enum
        {
            return new EnumInfo<EnumType>().GetAdjacentValue(_type, _next, _loop);
        }

        public static int ToInt(this System.Enum _value)
        {
            return System.Convert.ToInt32(_value);
        }

        public static uint ToUint(this System.Enum _value)
        {
            return System.Convert.ToUInt32(_value);
        }

        public static T ToEnum<T>(this int _value) where T : System.Enum
        {
            T[] _enumValues = GetEnumValues<T>();

            for (int i = 0; i < _enumValues.Length; i++)
            {
                if (_enumValues[i].ToInt() == _value)
                {
                    return _enumValues[i];
                }
            }

            return _enumValues[0];
        }

        public static int AddFlag<EnumType>(this EnumType _flag, EnumType _flagToAdd) where EnumType : System.Enum
        {
            return _flag.ToInt() | _flagToAdd.ToInt();
        }

        public static int RemoveFlag<EnumType>(this EnumType _flag, EnumType _flagToRemove) where EnumType : System.Enum
        {
            return _flag.ToInt() & (~_flagToRemove.ToInt());
        }

        public static bool ContainsFlag<EnumType>(this EnumType _flag, EnumType _flagToCheck) where EnumType : System.Enum
        {
            return (_flag.ToInt() & _flagToCheck.ToInt()) != 0;
        }

        public static int NegateFlag<EnumType>(this EnumType _flag) where EnumType : System.Enum
        {
            return ~(_flag.ToInt());
        }

        public static bool IsFlag<EnumType>() where EnumType : System.Enum
        {
            return typeof(EnumType).GetCustomAttributes(typeof(System.FlagsAttribute), true).Length > 0;
        }

        public static List<EnumType> DisassembleFlag<EnumType>(this EnumType _flag) where EnumType : System.Enum
        {
            List<EnumType> _dissasembled = new List<EnumType>();
            List<EnumType> _uniqueFlags = GetUniqueFlags<EnumType>();
            int _count = _uniqueFlags.Count;

            for (int i = 0; i < _count; i++)
            {
                if ((_uniqueFlags[i].ToInt() & _flag.ToInt()) != 0)
                {
                    _dissasembled.Add(_uniqueFlags[i]);
                }
            }

            return _dissasembled;
        }

        public static List<EnumType> GetUniqueFlags<EnumType>() where EnumType : System.Enum
        {
            List<EnumType> _uniqueFlags = new List<EnumType>();
            EnumType[] _values = GetEnumValues<EnumType>();
            int _count = _values.Length;

            for (int i = 0; i < _count; i++)
            {
                if (IsUniqueFlag(_values[i]))
                {
                    _uniqueFlags.Add(_values[i]);
                }
            }

            return _uniqueFlags;
        }

        public static bool IsUniqueFlag<EnumType>(EnumType _type) where EnumType : System.Enum
        {
            int _flag = _type.ToInt();

            if (_flag == 0)
            {
                return false;
            }

            int[] _intValues = _type.GetEnumIntValues();
            int _count = _intValues.Length;
            int _shareCount = 0;

            for (int i = 0; i < _count; i++)
            {
                int _testFlag = _intValues[i];

                if (_testFlag <= 0)
                {
                    continue;
                }

                if ((_flag & _testFlag) > 0 && _flag >= _testFlag)
                {
                    _shareCount++;
                }
            }

            return _shareCount == 1;
        }
    }
}
