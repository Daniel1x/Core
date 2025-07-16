namespace DL.MaterialPropertyBlockSetter
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;

    public static class MaterialExtensions
    {
        public static bool GetAllProperties(this Material _material, out string[] _properties, out int _propertyCount)
        {
            if (_material == null)
            {
                _properties = null;
                _propertyCount = 0;
                return false;
            }

            return _material.shader.GetAllProperties(out _properties, out _propertyCount);
        }

        public static bool GetAllProperties(this Shader _shader, out string[] _properties, out int _propertyCount)
        {
            if (_shader == null)
            {
                _properties = null;
                _propertyCount = 0;
                return false;
            }

            _propertyCount = _shader.GetPropertyCount();

            if (_propertyCount <= 0)
            {
                _properties = null;
                return false;
            }

            _properties = new string[_propertyCount];

            for (int i = 0; i < _propertyCount; i++)
            {
                _properties[i] = _shader.GetPropertyName(i);
            }

            return true;
        }

        public static bool GetAllPropertiesWithTypes(this Material _material, out ShaderProperty[] _properties, out int _propertyCount)
        {
            if (_material == null)
            {
                _properties = null;
                _propertyCount = 0;
                return false;
            }

            return _material.shader.GetAllPropertiesWithTypes(out _properties, out _propertyCount);
        }

        public static bool GetAllPropertiesWithTypes(this Shader _shader, out ShaderProperty[] _properties, out int _propertyCount)
        {
            if (_shader == null)
            {
                _properties = null;
                _propertyCount = 0;
                return false;
            }

            _propertyCount = _shader.GetPropertyCount();

            if (_propertyCount <= 0)
            {
                _properties = null;
                return false;
            }

            _properties = new ShaderProperty[_propertyCount];

            for (int i = 0; i < _propertyCount; i++)
            {
                _properties[i] = new ShaderProperty(_shader.GetPropertyType(i), _shader.GetPropertyName(i));
            }

            return true;
        }

        public static bool GetNameOfPropertyOfType(this Material _material, UnityEngine.Rendering.ShaderPropertyType _type, int _propertyID, out string _propertyName)
        {
            if (_propertyID >= 0 && _material.GetPropertiesOfType(_type, out string[] _properties, out int _count) && _count > 0 && _propertyID < _count)
            {
                _propertyName = _properties[_propertyID];
                return true;
            }

            _propertyName = null;
            return false;
        }

        public static bool GetPropertiesOfType(this Material _material, UnityEngine.Rendering.ShaderPropertyType _type, out string[] _properties, out int _propertyCount)
        {
            if (_material == null)
            {
                _properties = null;
                _propertyCount = 0;
                return false;
            }

            return _material.shader.GetPropertiesOfType(_type, out _properties, out _propertyCount);
        }

        public static bool GetPropertiesOfType(this Shader _shader, ShaderPropertyType _type, out string[] _properties, out int _propertyCount)
        {
            if (_shader == null)
            {
                _properties = null;
                _propertyCount = 0;
                return false;
            }

            int _allPropertyCount = _shader.GetPropertyCount();

            if (_allPropertyCount <= 0)
            {
                _properties = null;
                _propertyCount = 0;
                return false;
            }

            List<string> _propertiesOfType = new List<string>();

            for (int i = 0; i < _allPropertyCount; i++)
            {
                if (_shader.GetPropertyType(i) == _type)
                {
                    _propertiesOfType.Add(_shader.GetPropertyName(i));
                }
            }

            _propertyCount = _propertiesOfType.Count;

            if (_propertyCount <= 0)
            {
                _properties = null;
                return false;
            }

            _properties = _propertiesOfType.ToArray();
            return true;
        }

        public static string[] GetKeywords(this Shader _shader)
        {
            return _shader.keywordSpace.keywordNames;
        }

        public static List<string> GetOverridableKeywords(this Shader _shader)
        {
            List<string> _keywordNames = new List<string>();
            LocalKeyword[] _keywords = _shader.keywordSpace.keywords;

            for (int i = 0; i < _keywords.Length; i++)
            {
                if (_keywords[i].isOverridable)
                {
                    _keywordNames.Add(_keywords[i].name);
                }
            }

            return _keywordNames;
        }

        public static List<string> GetValidAndOverridableKeywords(this Shader _shader)
        {
            List<string> _keywordNames = new List<string>();
            LocalKeyword[] _keywords = _shader.keywordSpace.keywords;

            for (int i = 0; i < _keywords.Length; i++)
            {
                if (_keywords[i].isValid && _keywords[i].isOverridable)
                {
                    _keywordNames.Add(_keywords[i].name);
                }
            }

            return _keywordNames;
        }

        public static List<string> GetFilteredKeywords(this Shader _shader, string _filter, System.StringComparison _comparsion = System.StringComparison.OrdinalIgnoreCase)
        {
            List<string> _keywordNames = new List<string>();
            LocalKeyword[] _keywords = _shader.keywordSpace.keywords;

            for (int i = 0; i < _keywords.Length; i++)
            {
                if (_keywords[i].name.Contains(_filter, _comparsion))
                {
                    _keywordNames.Add(_keywords[i].name);
                }
            }

            return _keywordNames;
        }

        public static List<string> GetFilteredOverridableKeywords(this Shader _shader, string _filter, System.StringComparison _comparsion = System.StringComparison.OrdinalIgnoreCase)
        {
            List<string> _keywordNames = new List<string>();
            LocalKeyword[] _keywords = _shader.keywordSpace.keywords;

            for (int i = 0; i < _keywords.Length; i++)
            {
                if (_keywords[i].isOverridable && _keywords[i].name.Contains(_filter, _comparsion))
                {
                    _keywordNames.Add(_keywords[i].name);
                }
            }

            return _keywordNames;
        }

        public static List<string> GetFilteredValidAndOverridableKeywords(this Shader _shader, string _filter, System.StringComparison _comparsion = System.StringComparison.OrdinalIgnoreCase)
        {
            List<string> _keywordNames = new List<string>();
            LocalKeyword[] _keywords = _shader.keywordSpace.keywords;

            for (int i = 0; i < _keywords.Length; i++)
            {
                if (_keywords[i].isValid && _keywords[i].isOverridable && _keywords[i].name.Contains(_filter, _comparsion))
                {
                    _keywordNames.Add(_keywords[i].name);
                }
            }

            return _keywordNames;
        }

        public static List<string> GetGlobalKeywords()
        {
            List<string> _keywords = new List<string>();

            foreach (var _keyword in Shader.globalKeywords)
            {
                _keywords.Add(_keyword.name);
            }

            return _keywords;
        }

        public static List<string> GetFilteredGlobalKeywords(string _filter, System.StringComparison _comparsion = System.StringComparison.OrdinalIgnoreCase)
        {
            List<string> _keywords = new List<string>();

            foreach (GlobalKeyword _keyword in Shader.globalKeywords)
            {
                if (_keyword.name.Contains(_filter, _comparsion))
                {
                    _keywords.Add(_keyword.name);
                }
            }

            return _keywords;
        }
    }
}
