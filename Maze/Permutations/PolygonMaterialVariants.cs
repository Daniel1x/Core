namespace DL.Core.Maze
{
    using System.Collections.Generic;
    using UnityEngine;

    public class PolygonMaterialVariants
    {
        private readonly Dictionary<PolygonVisualizationState, Material> materials;

        public Dictionary<PolygonVisualizationState, Material> Materials => materials;

        public PolygonMaterialVariants(Material _baseMaterial)
        {
            if (_baseMaterial == null)
            {
                throw new System.ArgumentNullException(nameof(_baseMaterial), "Base material cannot be null.");
            }

            PolygonVisualizationState[] _states = PolygonStateVisualizationExtensions.States;
            int _count = _states.Length;

            materials = new Dictionary<PolygonVisualizationState, Material>(_count);

            for (int i = 0; i < _states.Length; i++)
            {
                PolygonVisualizationState _state = _states[i];
                Material _newMaterialWithKeyword = new Material(_baseMaterial);
                _newMaterialWithKeyword.SetFloat(PolygonStateVisualizationExtensions.k_StatePropertyID, (float)_state);
                _newMaterialWithKeyword.name += $"_{_state}";

                materials[_state] = _newMaterialWithKeyword;
            }
        }

        public Material GetMaterial(PolygonVisualizationState _state)
        {
            if (!materials.TryGetValue(_state, out Material _material))
            {
                throw new KeyNotFoundException($"Material for state {_state} not found. Ensure that the state was included during initialization.");
            }

            return _material;
        }
    }
}
