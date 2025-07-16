namespace DL.MaterialPropertyBlockSetter
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class RendererData
    {
        public delegate bool MaterialComparer(Material _material);

        [System.NonSerialized] public bool VisibleInEditor = false;

        [SerializeField] private Renderer renderer = null;
        [SerializeField] private List<MaterialData> materials = new List<MaterialData>();

        public Renderer Renderer => renderer;
        public List<MaterialData> Materials => materials;

        public bool IsOverridingData => materials.AreAnyMaterialValuesOverwritten();

        public RendererData(Renderer _renderer)
        {
            renderer = _renderer;
            Initialize();
        }

        public void Initialize()
        {
            int _materialCount = renderer.sharedMaterials.Length;
            materials = new List<MaterialData>(_materialCount);

            for (int i = 0; i < _materialCount; i++)
            {
                materials.Add(new MaterialData(renderer, i, renderer.sharedMaterials[i]));
            }
        }

        public void AddMaterial(Material _newMaterial)
        {
            renderer.materials = renderer.materials.AddToArray(_newMaterial);
            materials.Add(new MaterialData(renderer, renderer.materials.Length - 1, _newMaterial));
        }

        public void RemoveMaterialByPartOfName(string _partOfName)
        {
            RemoveMaterial((_otherMaterial) => _otherMaterial.name.Contains(_partOfName));
        }

        public void RemoveMaterialByName(string _materialName)
        {
            RemoveMaterial((_otherMaterial) => _otherMaterial.name == _materialName);
        }

        public void RemoveMaterial(Material _material)
        {
            RemoveMaterial((_otherMaterial) => _material.name == _otherMaterial.name);
        }

        public void RemoveMaterial(MaterialComparer _comparer)
        {
            int _materialsCount = _countMaterials();

            if (_materialsCount <= 0)
            {
                return;
            }

            Material[] _materials = new Material[renderer.materials.Length - _materialsCount];
            int _materialID = 0;

            for (int i = 0; i < renderer.materials.Length && _materialID < _materials.Length; i++)
            {
                if (_comparer(renderer.materials[i]))
                {
                    continue;
                }

                _materials[_materialID] = renderer.materials[i];
                _materialID++;
            }

            renderer.materials = _materials;

            materials.Remove(materials.Find(_cm => _comparer(_cm.Material)));

            int _countMaterials()
            {
                int _materialsCount = 0;

                for (int i = 0; i < materials.Count; i++)
                {
                    if (_comparer(materials[i].Material))
                    {
                        _materialsCount++;
                    }
                }

                return _materialsCount;
            }
        }

        public void AddMissingMaterials()
        {
            int _materialCount = renderer.sharedMaterials.Length;

            for (int i = 0; i < _materialCount; i++)
            {
                if (materials.Contains(renderer.sharedMaterials[i]) == false)
                {
                    materials.Add(new MaterialData(renderer, i, renderer.sharedMaterials[i]));
                }
            }
        }

        public bool RemoveNotUsedMaterialsAndWrongModifiers()
        {
            bool _anyFieldModified = false;
            int _materialCount = materials.Count;

            for (int i = _materialCount - 1; i >= 0; i--)
            {
                if (materials[i] == null || materials[i].Material == null || renderer.sharedMaterials.Contains(materials[i].Material) == false)
                {
                    materials.RemoveAt(i);
                    _anyFieldModified = true;
                }
                else if (materials[i].RemoveUnassignedVariables())
                {
                    _anyFieldModified = true;
                }
            }

            return _anyFieldModified;
        }

        public void AddModifierToAllMaterials<T1, T2>(T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].AddModifier<T1, T2>(_modifier);
            }
        }

        public void AddModifierToMaterial<T1, T2>(int _materialID, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (isMaterialIDValid(_materialID))
            {
                materials[_materialID].AddModifier<T1, T2>(_modifier);
            }
        }

        public void RemoveModifierFromAllMaterials<T1, T2>(T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].RemoveModifier<T1, T2>(_modifier);
            }
        }

        public void RemoveModifierFromMaterial<T1, T2>(int _materialID, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (isMaterialIDValid(_materialID))
            {
                materials[_materialID].RemoveModifier<T1, T2>(_modifier);
            }
        }

        public void EnableKeywordOnAllMaterials(string _keyword)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].EnableKeyword(_keyword);
            }
        }

        public void DisableKeywordOnAllMaterials(string _keyword)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].DisableKeyword(_keyword);
            }
        }

        public void ApplyModifiersAndSetBlock(bool _createNewPropertyBlock = false)
        {
            if (renderer == null)
            {
                return;
            }

            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].ApplyModifiersAndSetBlock(renderer, _createNewPropertyBlock);
            }
        }

        private bool isMaterialIDValid(int _materialID)
        {
            return _materialID >= 0 && _materialID < materials.Count;
        }
    }
}
