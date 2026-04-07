namespace DL.Core.Maze
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class PolygonPermutationService : IPolygonPermutationDataProvider
    {
        private readonly Dictionary<int, PolygonMaterialPermutation> permutationsByVertex = null;

        public PolygonPermutationService(Material[] _materials, string _vertexCountProperty = "_VertexCount", string _wallMaskProperty = "_WallMask")
        {
            if (_materials == null || _materials.Length == 0)
            {
                throw new System.ArgumentException("At least one material data entry must be provided.", nameof(_materials));
            }

            if (string.IsNullOrEmpty(_wallMaskProperty))
            {
                throw new System.ArgumentException("Wall mask property name cannot be null or empty.", nameof(_wallMaskProperty));
            }

            int _initialCapacity = _materials.Length;
            permutationsByVertex = new Dictionary<int, PolygonMaterialPermutation>(_initialCapacity);

            foreach (Material _material in _materials)
            {
                if (_material == null)
                {
                    Debug.LogWarning("Null material entry detected. Skipping null material.");
                    continue;
                }

                if (!_material.HasProperty(_vertexCountProperty))
                {
                    Debug.LogWarning($"Material '{_material.name}' does not have the required vertex count property '{_vertexCountProperty}'. Skipping this material.");
                    continue;
                }

                if (!_material.HasProperty(_wallMaskProperty))
                {
                    Debug.LogWarning($"Material '{_material.name}' does not have the required wall mask property '{_wallMaskProperty}'. Skipping this material.");
                    continue;
                }

                var _vertexCount = Mathf.RoundToInt(_material.GetFloat(_vertexCountProperty));

                if (_vertexCount < 3)
                {
                    Debug.LogWarning($"Skipping material for vertex count {_vertexCount} as it is less than 3.");
                    continue;
                }

                if (permutationsByVertex.ContainsKey(_vertexCount))
                {
                    Debug.LogWarning($"Duplicate material entry for vertex count {_vertexCount} detected. Skipping duplicate.");
                    continue;
                }

                permutationsByVertex[_vertexCount] = new PolygonMaterialPermutation(_material, _vertexCount, _wallMaskProperty);
            }
        }

        public PolygonPermutationData GetPermutation<T>(T _polygon) where T : IPolygonShape, IPolygonWallMaskProvider
        {
            if (_polygon == null)
            {
                throw new System.ArgumentNullException(nameof(_polygon), "Polygon cannot be null.");
            }

            int _vertexCount = _polygon.VertexCount;

            if (_vertexCount < 3)
            {
                throw new System.ArgumentException("Polygon must have at least 3 vertices.", nameof(_polygon));
            }

            if (!permutationsByVertex.TryGetValue(_vertexCount, out PolygonMaterialPermutation _permutation))
            {
                throw new KeyNotFoundException($"No material permutation found for vertex count {_vertexCount}. Ensure that a material is provided for this vertex count.");
            }

            return _permutation.GetPermutation(_polygon.GetWallMask());
        }

        private class PolygonMaterialPermutation
        {
            private readonly Material templateMaterial;
            private readonly Dictionary<int, PolygonPermutationData> permutationsByWallMask;

            public PolygonMaterialPermutation(Material _material, int _vertexCount, string _wallMaskProperty)
            {
                // Create a new material instance to serve as the template for this vertex count
                templateMaterial = _material;
                templateMaterial.SetFloat(_wallMaskProperty, 0f);

                // Initialize the dictionary to hold permutations for each wall mask
                permutationsByWallMask = new Dictionary<int, PolygonPermutationData>(_vertexCount);

                // Add default permutation for no walls
                int _defaultMask = 0;
                PolygonModifier _defaultModifier = new PolygonModifier(_defaultMask, 0f);
                permutationsByWallMask[_defaultMask] = new PolygonPermutationData(templateMaterial, _defaultModifier);

                // Generate permutations for all possible wall masks
                int _maxMask = (1 << _vertexCount) - 1;
                float _angleStep = 360f / _vertexCount;

                for (int _baseMask = 1; _baseMask <= _maxMask; _baseMask++)
                {
                    for (int i = 0; i < _vertexCount; i++)
                    {
                        int _rotatedMask = (_baseMask << i) | (_baseMask >> (_vertexCount - i));
                        _rotatedMask &= _maxMask; // Ensure we only keep the meaningful bits

                        if (permutationsByWallMask.ContainsKey(_rotatedMask))
                        {
                            continue;
                        }

                        PolygonModifier _modifier = new PolygonModifier(_baseMask, i * _angleStep);

                        if (permutationsByWallMask.TryGetValue(_baseMask, out PolygonPermutationData _existingPermutation))
                        {
                            permutationsByWallMask[_rotatedMask] = new PolygonPermutationData(_existingPermutation.Variants, _modifier);
                        }
                        else
                        {
                            Material _newMaterial = new Material(templateMaterial);
                            _newMaterial.name += $"_Mask_{Convert.ToString(_baseMask, 2)}";
                            _newMaterial.SetFloat(_wallMaskProperty, _baseMask);

                            permutationsByWallMask[_rotatedMask] = new PolygonPermutationData(_newMaterial, _modifier);
                        }
                    }
                }
            }

            public PolygonPermutationData GetPermutation(int _wallMask)
            {
                if (permutationsByWallMask.TryGetValue(_wallMask, out PolygonPermutationData _permutation))
                {
                    return _permutation;
                }

                throw new KeyNotFoundException($"No material permutation found for wall mask {_wallMask}. Ensure that the wall mask is valid for this vertex count.");
            }
        }
    }
}
