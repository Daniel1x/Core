namespace DL.Core.Maze
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PolygonMaterialVariants
    {
        private const string k_DefaultKeyword = "_DEFAULT_KEY";

        private readonly Dictionary<string, Material> materials;

        public Dictionary<string, Material> Materials => materials;

        public PolygonMaterialVariants(Material _baseMaterial, string[] _keywords)
        {
            if (_baseMaterial == null)
            {
                throw new System.ArgumentNullException(nameof(_baseMaterial), "Base material cannot be null.");
            }

            if (_keywords == null || _keywords.Length == 0)
            {
                throw new System.ArgumentException("At least one keyword must be provided.", nameof(_keywords));
            }

            int _count = _keywords.Length + 1;
            materials = new Dictionary<string, Material>(_count);
            materials[k_DefaultKeyword] = _baseMaterial;

            for (int i = 0; i < _keywords.Length; i++)
            {
                if (string.IsNullOrEmpty(_keywords[i]))
                {
                    Debug.LogWarning($"Keyword at index {i} is null or empty. Skipping this keyword.");
                    continue;
                }

                Material _newMaterialWithKeyword = new Material(_baseMaterial);
                _newMaterialWithKeyword.EnableKeyword(_keywords[i]);
                _newMaterialWithKeyword.name += $"_{_keywords[i]}";
                materials[_keywords[i]] = _newMaterialWithKeyword;
            }
        }

        public Material GetMaterial(string _keyword)
        {
            if (string.IsNullOrEmpty(_keyword) == false && materials.TryGetValue(_keyword, out var _material))
            {
                return _material;
            }

            if (materials.TryGetValue(k_DefaultKeyword, out var _defaultMaterial))
            {
                return _defaultMaterial;
            }

            throw new KeyNotFoundException($"No material found for keyword '{_keyword}' and no default material is available.");
        }
    }
}
