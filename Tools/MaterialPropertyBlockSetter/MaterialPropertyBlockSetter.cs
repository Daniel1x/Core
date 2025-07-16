namespace DL.MaterialPropertyBlockSetter
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [DisallowMultipleComponent, ExecuteAlways]
    public class MaterialPropertyBlockSetter : MonoBehaviour
    {
        /// <summary> Editor only!</summary>
        [System.NonSerialized] public bool DisplayRenderers = false;
        /// <summary> Editor only!</summary>
        [System.NonSerialized] public MaterialData MaterialThatDisplaysEditorTools = null;

        public List<RendererData> Renderers = new List<RendererData>();

        [SerializeField] protected ExcludedRendererType excludedRendererType = ExcludedRendererType.None;
        [SerializeField] protected BlockUpdateMode updateMode = BlockUpdateMode.OnEnable;
        [SerializeField, Min(1f)] protected int updateInterval = 60;

        private int updateSeed = 0;

        public int UpdateInterval
        {
            get => updateInterval;
            set
            {
                updateMode = value > 0
                    ? updateMode.EnableMode(BlockUpdateMode.OnUpdate)
                    : updateMode.DisableMode(BlockUpdateMode.OnUpdate);

                updateInterval = value;
            }
        }

        private void Awake()
        {
            updateSeed = UnityEngine.Random.Range(0, 1000); //Random seed

            if (updateMode.ContainsMode(BlockUpdateMode.OnAwake))
            {
                UpdateMaterials();
            }
        }

        private void OnEnable()
        {
            if (updateMode.ContainsMode(BlockUpdateMode.OnEnable))
            {
                UpdateMaterials();
            }
        }

        private void Start()
        {
            if (updateMode.ContainsMode(BlockUpdateMode.OnStart))
            {
                UpdateMaterials();
            }
        }

        private void Update()
        {
            if (updateMode.ContainsMode(BlockUpdateMode.OnUpdate) == false || updateInterval <= 0)
            {
                return;
            }

            if (updateInterval == 1 || ((Time.frameCount + updateSeed) % updateInterval == 0))
            {
                UpdateMaterials();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            FindRenderers();
        }

        private void Reset()
        {
            FindRenderers();
        }
#endif

        public void EnableUpdateMode(BlockUpdateMode _mode, int _updateInterval = 1)
        {
            updateMode = updateMode.EnableMode(_mode);
            UpdateInterval = _updateInterval;
        }

        public void DisableUpdateMode(BlockUpdateMode _mode)
        {
            updateMode = updateMode.DisableMode(_mode);
        }

        public void UpdateMaterials(bool _createNewPropertyBlock = false)
        {
            for (int i = 0; i < Renderers.Count; i++)
            {
                Renderers[i].ApplyModifiersAndSetBlock(_createNewPropertyBlock);
            }
        }

        public void FindRenderers()
        {
            Renderers.RemoveNullReferences();

            Renderer[] _newRenderers = GetComponentsInChildren<Renderer>(true);

            for (int i = 0; i < _newRenderers.Length; i++)
            {
                if (IsRendererValid(_newRenderers[i], out int _id))
                {
                    if (IsRendererExcluded(_newRenderers[i]))
                    {
                        Renderers.RemoveAt(_id);
                    }
                    else
                    {
                        Renderers[_id].AddMissingMaterials();
                    }
                }
                else
                {
                    if (IsRendererExcluded(_newRenderers[i]) == false)
                    {
                        Renderers.Add(new RendererData(_newRenderers[i]));
                    }
                }
            }
        }

        public bool RemoveNullReferences()
        {
            return Renderers.RemoveNullReferences();
        }

        public void RemoveAllRenderers()
        {
            Renderers.Clear();
            Renderers.TrimExcess();
        }

        public void AddMaterialToEveryMaterialOfAllRenderers(Material _newMaterial)
        {
            int _count = Renderers.Count;

            for (int i = 0; i < _count; i++)
            {
                Renderers[i].AddMaterial(_newMaterial);
            }
        }

        public void RemoveMaterialFromEveryMaterialOfAllRenderers(Material _newMaterial)
        {
            int _count = Renderers.Count;

            for (int i = 0; i < _count; i++)
            {
                Renderers[i].RemoveMaterial(_newMaterial);
            }
        }

        public void AddModifierToEveryMaterialOfRenderer<T1, T2>(Renderer _renderer, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererValid(_renderer, out int _id))
            {
                AddModifierToEveryMaterialOfRenderer<T1, T2>(_id, _modifier);
            }
        }

        public void AddModifierToEveryMaterialOfRenderer<T1, T2>(int _rendererID, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererIDValid(_rendererID) == false)
            {
                return;
            }

            Renderers[_rendererID].AddModifierToAllMaterials<T1, T2>(_modifier);
        }

        public void AddModifierToEveryMaterialOfAllRenderers<T1, T2>(T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            int _count = Renderers.Count;

            for (int i = 0; i < _count; i++)
            {
                Renderers[i].AddModifierToAllMaterials<T1, T2>(_modifier);
            }
        }

        public void AddModifierToRendererMaterial<T1, T2>(Renderer _renderer, int _materialID, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererValid(_renderer, out int _rendererID))
            {
                AddModifierToRendererMaterial<T1, T2>(_rendererID, _materialID, _modifier);
            }
        }

        public void AddModifierToRendererMaterial<T1, T2>(int _rendererID, int _materialID, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererIDValid(_rendererID) == false)
            {
                return;
            }

            Renderers[_rendererID].AddModifierToMaterial<T1, T2>(_materialID, _modifier);
        }

        public void AddModifierToAllRendererMaterials<T1, T2>(Renderer _renderer, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererValid(_renderer, out int _rendererID))
            {
                AddModifierToAllRendererMaterials<T1, T2>(_rendererID, _modifier);
            }
        }

        public void AddModifierToAllRendererMaterials<T1, T2>(int _rendererID, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererIDValid(_rendererID) == false)
            {
                return;
            }

            Renderers[_rendererID].AddModifierToAllMaterials<T1, T2>(_modifier);
        }

        public void RemoveModifierFromEveryMaterialOfRenderer<T1, T2>(Renderer _renderer, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererValid(_renderer, out int _id))
            {
                RemoveModifierFromEveryMaterialOfRenderer<T1, T2>(_id, _modifier);
            }
        }

        public void RemoveModifierFromEveryMaterialOfRenderer<T1, T2>(int _rendererID, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererIDValid(_rendererID) == false)
            {
                return;
            }

            Renderers[_rendererID].RemoveModifierFromAllMaterials<T1, T2>(_modifier);
        }

        public void RemoveModifierFromEveryMaterialOfAllRenderers<T1, T2>(T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            int _count = Renderers.Count;

            for (int i = 0; i < _count; i++)
            {
                Renderers[i].RemoveModifierFromAllMaterials<T1, T2>(_modifier);
            }
        }

        public void RemoveModifierFromRendererMaterial<T1, T2>(Renderer _renderer, int _materialID, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererValid(_renderer, out int _rendererID))
            {
                RemoveModifierFromRendererMaterial<T1, T2>(_rendererID, _materialID, _modifier);
            }
        }

        public void RemoveModifierFromRendererMaterial<T1, T2>(int _rendererID, int _materialID, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererIDValid(_rendererID) == false)
            {
                return;
            }

            Renderers[_rendererID].RemoveModifierFromMaterial<T1, T2>(_materialID, _modifier);
        }

        public void RemoveModifierFromAllRendererMaterials<T1, T2>(Renderer _renderer, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererValid(_renderer, out int _rendererID))
            {
                RemoveModifierFromAllRendererMaterials<T1, T2>(_rendererID, _modifier);
            }
        }

        public void RemoveModifierFromAllRendererMaterials<T1, T2>(int _rendererID, T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (IsRendererIDValid(_rendererID) == false)
            {
                return;
            }

            Renderers[_rendererID].RemoveModifierFromAllMaterials<T1, T2>(_modifier);
        }

        public bool IsRendererIDValid(int _rendererID)
        {
            bool _valid = _rendererID >= 0 && _rendererID < Renderers.Count;

            if (_valid == false)
            {
                MyLog.Warning($"Material Property Block Setter :: Obj: {name} :: Received invalid renderer id: {_rendererID}!", Color.yellow);
            }

            return _valid;
        }

        public bool IsRendererValid(Renderer _renderer, out int _rendererID)
        {
            if (_renderer == null)
            {
                MyLog.Warning($"Material Property Block Setter :: Obj: {name} :: Received invalid renderer: NULL!", Color.yellow);
                _rendererID = -1;
                return false;
            }

            return Renderers.Contains(_renderer, out _rendererID);
        }

        public void PerformActionOnEveryCachedRendererOfType<T>(UnityAction<T> _action) where T : Renderer
        {
            int _rendererCount = Renderers.Count;

            for (int i = 0; i < _rendererCount; i++)
            {
                if (Renderers[i] == null || Renderers[i].Renderer == null || Renderers[i].Renderer is not T _rendererOfType)
                {
                    continue;
                }

                _action(_rendererOfType);
            }
        }

        public void PerformActionOnEveryCachedRenderer(UnityAction<RendererData> _action)
        {
            int _rendererCount = Renderers.Count;

            for (int i = 0; i < _rendererCount; i++)
            {
                if (Renderers[i] == null || Renderers[i].Renderer == null)
                {
                    continue;
                }

                _action(Renderers[i]);
            }
        }

        public void PerformActionOnEveryCachedMaterial(UnityAction<MaterialData> _action)
        {
            PerformActionOnEveryCachedRenderer(_actionPerRenderer);

            void _actionPerRenderer(RendererData _renderer)
            {
                List<MaterialData> _materials = _renderer.Materials;
                int _materialCount = _materials.Count;

                for (int j = 0; j < _materialCount; j++)
                {
                    if (_materials[j] == null || _materials[j].Material == null)
                    {
                        continue;
                    }

                    _action(_materials[j]);
                }
            }
        }

        public void EnableKeywordForEveryMaterialOfAllRenderers(string _keyword)
        {
            int _count = Renderers.Count;

            for (int i = 0; i < _count; i++)
            {
                Renderers[i].EnableKeywordOnAllMaterials(_keyword);
            }
        }

        public void DisableKeywordForEveryMaterialOfAllRenderers(string _keyword)
        {
            int _count = Renderers.Count;

            for (int i = 0; i < _count; i++)
            {
                Renderers[i].DisableKeywordOnAllMaterials(_keyword);
            }
        }

        public bool IsRendererExcluded<T>(T _renderer) where T : Renderer
        {
            return excludedRendererType != ExcludedRendererType.None && excludedRendererType.ContainsType(_renderer.GetExcludedRendererType());
        }

        public void SetExcludedRenderers(ExcludedRendererType _type, bool _findRenderers = true)
        {
            excludedRendererType = _type;

            if (_findRenderers)
            {
                FindRenderers();
            }
        }
    }
}
