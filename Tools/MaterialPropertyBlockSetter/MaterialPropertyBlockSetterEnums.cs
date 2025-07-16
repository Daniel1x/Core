namespace DL.MaterialPropertyBlockSetter
{
    using UnityEngine;
    using UnityEngine.Rendering;

    [System.Flags]
    public enum BlockUpdateMode
    {
        None = 0,
        OnAwake = 1,
        OnEnable = 2,
        OnStart = 4,
        OnUpdate = 8,
        All = 15,
    }

    [System.Flags]
    public enum ShaderPropertyMode
    {
        Color = 1,
        Vector = 2,
        Float = 4,
        Range = 8,
        Texture = 16,
        Int = 32
    }

    [System.Flags]
    public enum ExcludedRendererType
    {
        None = 0,
        MeshRenderer = 1,
        SkinnedMeshRenderer = 2,
        LineRenderer = 4,
        ParticleSystemRenderer = 8,
        BillboardRenderer = 16,
        SpriteRenderer = 32,
        TrailRenderer = 64,
        All = 127,
    }

    public static class MaterialPropertyBlockSetterEnums
    {
        public static bool ContainsType(this ExcludedRendererType _types, ExcludedRendererType _type)
        {
            return (_types & _type) != 0;
        }

        public static ExcludedRendererType Add(this ExcludedRendererType _types, ExcludedRendererType _type)
        {
            return _types | _type;
        }

        public static ExcludedRendererType Remove(this ExcludedRendererType _types, ExcludedRendererType _type)
        {
            return _types & (~_type);
        }

        public static ExcludedRendererType GetExcludedRendererType<T>(this T _renderer) where T : Renderer
        {
            if (_renderer == null)
            {
                return ExcludedRendererType.None;
            }

            return _renderer switch
            {
                MeshRenderer => ExcludedRendererType.MeshRenderer,
                SkinnedMeshRenderer => ExcludedRendererType.SkinnedMeshRenderer,
                LineRenderer => ExcludedRendererType.LineRenderer,
                ParticleSystemRenderer => ExcludedRendererType.ParticleSystemRenderer,
                BillboardRenderer => ExcludedRendererType.BillboardRenderer,
                SpriteRenderer => ExcludedRendererType.SpriteRenderer,
                TrailRenderer => ExcludedRendererType.TrailRenderer,
                _ => ExcludedRendererType.None,
            };
        }

        public static bool ContainsMode(this BlockUpdateMode _mainMode, BlockUpdateMode _mode)
        {
            return (_mainMode & _mode) != 0;
        }

        public static BlockUpdateMode EnableMode(this BlockUpdateMode _mainMode, BlockUpdateMode _mode)
        {
            return _mainMode | _mode;
        }

        public static BlockUpdateMode DisableMode(this BlockUpdateMode _mainMode, BlockUpdateMode _mode)
        {
            return _mainMode & (~_mode);
        }

        public static bool ContainsType(this ShaderPropertyMode _mainMode, ShaderPropertyMode _mode)
        {
            return ((int)_mainMode & (int)_mode) != 0;
        }

        public static ShaderPropertyMode EnableType(this ShaderPropertyMode _mainMode, ShaderPropertyMode _type)
        {
            return (ShaderPropertyMode)((int)_mainMode | (int)_type);
        }

        public static ShaderPropertyMode DisableType(this ShaderPropertyMode _mainMode, ShaderPropertyMode _type)
        {
            return (ShaderPropertyMode)((int)_mainMode & (~(int)_type));
        }

        public static ShaderPropertyMode ToMode(this ShaderPropertyType _type)
        {
            return _type switch
            {
                ShaderPropertyType.Color => ShaderPropertyMode.Color,
                ShaderPropertyType.Vector => ShaderPropertyMode.Vector,
                ShaderPropertyType.Float => ShaderPropertyMode.Float,
                ShaderPropertyType.Range => ShaderPropertyMode.Range,
                ShaderPropertyType.Texture => ShaderPropertyMode.Texture,
                ShaderPropertyType.Int => ShaderPropertyMode.Int,
                _ => ShaderPropertyMode.Float
            };
        }

        public static bool ContainsType(this ShaderPropertyMode _mainMode, ShaderPropertyType _type)
        {
            return _mainMode.ContainsType(_type.ToMode());
        }

        public static ShaderPropertyMode EnableType(this ShaderPropertyMode _mainMode, ShaderPropertyType _type)
        {
            return _mainMode.EnableType(_type.ToMode());
        }

        public static ShaderPropertyMode DisableType(this ShaderPropertyMode _mainMode, ShaderPropertyType _type)
        {
            return _mainMode.DisableType(_type.ToMode());
        }

        public static ShaderPropertyType ToSingleType(this ShaderPropertyMode _mode)
        {
            if (_mode.ContainsType(ShaderPropertyType.Color))
            {
                return ShaderPropertyType.Color;
            }

            if (_mode.ContainsType(ShaderPropertyType.Vector))
            {
                return ShaderPropertyType.Vector;
            }

            if (_mode.ContainsType(ShaderPropertyType.Float))
            {
                return ShaderPropertyType.Float;
            }

            if (_mode.ContainsType(ShaderPropertyType.Range))
            {
                return ShaderPropertyType.Range;
            }

            if (_mode.ContainsType(ShaderPropertyType.Texture))
            {
                return ShaderPropertyType.Texture;
            }

            if (_mode.ContainsType(ShaderPropertyType.Int))
            {
                return ShaderPropertyType.Int;
            }

            return ShaderPropertyType.Float;
        }
    }
}
