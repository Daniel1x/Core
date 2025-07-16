namespace DL.MaterialPropertyBlockSetter
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class MaterialPropertyBlockSetterExtensions
    {
        public static bool ContainsProperty<T1, T2>(this List<T1> _list, string _property, out int _foundAtID) where T1 : MaterialPropertyModifier<T2>
        {
            int _count = _list.Count;

            for (int i = 0; i < _count; i++)
            {
                if (_list[i].Property == _property)
                {
                    _foundAtID = i;
                    return true;
                }
            }

            _foundAtID = -1;
            return false;
        }

        public static void InsertModifier<T1, T2>(this List<T1> _list, T1 _propertyModifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (_list.TryToRemoveModifier<T1, T2>(_propertyModifier, out T1 _removed))
            {
                _propertyModifier.OverrideDefaultValue(_removed);
            }

            _list.Add(_propertyModifier);
        }

        public static bool TryToRemoveModifier<T1, T2>(this List<T1> _list, T1 _propertyModifier, out T1 _removed) where T1 : MaterialPropertyModifier<T2>
        {
            if (_list.ContainsProperty<T1, T2>(_propertyModifier.Property, out int _id))
            {
                _removed = _list[_id];
                _list.RemoveAt(_id);
                return true;
            }

            _removed = null;
            return false;
        }

        public static void ApplyModifiersToBlock<T1, T2>(this List<T1> _list, Material _material, MaterialPropertyBlock _targetBlock) where T1 : MaterialPropertyModifier<T2>
        {
            int _count = _list.Count;

            for (int i = 0; i < _count; i++)
            {
                _list[i].ApplyModifier(_material, _targetBlock);
            }
        }

        public static bool Contains(this List<RendererData> _list, Renderer _renderer, out int _id)
        {
            if (_renderer == null)
            {
                _id = -1;
                return false;
            }

            int _listSize = _list.Count;

            for (int i = 0; i < _listSize; i++)
            {
                if (_list[i] != null && _list[i].Renderer == _renderer)
                {
                    _id = i;
                    return true;
                }
            }

            _id = -1;
            return false;
        }

        public static bool Contains(this List<MaterialData> _list, Material _material)
        {
            if (_material == null)
            {
                return false;
            }

            int _listSize = _list.Count;

            for (int i = 0; i < _listSize; i++)
            {
                if (_list[i] != null && _list[i].Material == _material)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool RemoveNullReferences(this List<RendererData> _list)
        {
            bool _anyFieldModified = false;
            int _renderersCount = _list.Count;

            for (int i = _renderersCount - 1; i >= 0; i--)
            {
                RendererData _renderer = _list[i];

                if (_renderer == null || _renderer.Renderer == null)
                {
                    _list.RemoveAt(i);
                    _markAnyFieldModified(true);
                }
                else
                {
                    _markAnyFieldModified(_renderer.RemoveNotUsedMaterialsAndWrongModifiers());
                }
            }

            return _anyFieldModified;

            void _markAnyFieldModified(bool _modified)
            {
                if (_anyFieldModified == false)
                {
                    _anyFieldModified = _modified;
                }
            }
        }

        public static void RemoveRendererThatIsNotOverridingAnyData(this List<RendererData> _list)
        {
            int _renderersCount = _list.Count;

            for (int i = _renderersCount - 1; i >= 0; i--)
            {
                if (_list[i] == null || _list[i].Renderer == null || _list[i].IsOverridingData == false)
                {
                    _list.RemoveAt(i);
                }
            }
        }

        public static bool AreAnyMaterialValuesOverwritten(this List<MaterialData> _list)
        {
            if (_list == null)
            {
                return false;
            }

            int _count = _list.Count;

            if (_count <= 0)
            {
                return false;
            }

            for (int i = 0; i < _count; i++)
            {
                if (_list[i].OverridedTypes != 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ModifiesTheSamePropertyMultipleTimes<T1, T2>(this List<T1> _modifiers, out List<string> _duplicatedTypes, out int _duplicatedTypesCount) where T1 : MaterialPropertyModifier<T2>
        {
            if (_modifiers == null)
            {
                _duplicatedTypes = null;
                _duplicatedTypesCount = -1;
                return false;
            }

            int _count = _modifiers.Count;

            if (_count <= 0)
            {
                _duplicatedTypes = null;
                _duplicatedTypesCount = -1;
                return false;
            }

            List<string> _usedTypes = new List<string>();
            _duplicatedTypes = new List<string>();

            for (int i = 0; i < _count; i++)
            {
                if (_usedTypes.AddIfNotContains(_modifiers[i].Property) == false)
                {
                    _duplicatedTypes.AddIfNotContains(_modifiers[i].Property);
                }
            }

            _duplicatedTypesCount = _duplicatedTypes.Count;
            return _duplicatedTypesCount > 0;
        }
    }
}
