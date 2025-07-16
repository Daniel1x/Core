using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;

public static class RenderTextureExtensions
{
    public const string GENERIC_RT_NAME = "rt_Generic";

    [System.Serializable]
    public class RenderTextureLerpController : System.IDisposable
    {
        protected const string GENERIC_RT_NAME = "generic_NewLerpTexture";
        protected const int KERNEL_ID = 0;
        protected const int THREAD_COUNT = 8;

        public event UnityAction<RenderTexture> OnRenderTextureCreated = null;
        ///<summary>(RenderTexture, LerpAnimationProgress, LerpProgress)</summary>
        public event UnityAction<RenderTexture, float, float> OnRenderTextureUpdate = null;

        public ComputeShader LerpShader = null;
        public Texture StartTexture = null;
        public Texture EndTexture = null;
        public TextureWrapMode TextureWrapMode = TextureWrapMode.Repeat;
        public GraphicsFormat GraphicsFormat = GraphicsFormat.R32G32B32A32_UInt;
        public AnimationCurve LerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public RenderTexture GenericRenderTexture { get; private set; } = null;

        protected List<RenderTexture> createdRenderTextures = new List<RenderTexture>();
        protected float lerpAnimationProgress = 0f;
        protected Vector2Int size = default;

        public float LerpAnimationProgress
        {
            get => lerpAnimationProgress;
            private set
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false || GenericRenderTexture == null)
                {
                    return;
                }
#endif

                lerpAnimationProgress = value;
                float _evaluatedLerpProgress = LerpProgress;
                LerpShader.SetFloat("Progress", _evaluatedLerpProgress);
                LerpShader.Dispatch(KERNEL_ID, size.x / THREAD_COUNT, size.y / THREAD_COUNT, 1);

                OnRenderTextureUpdate?.Invoke(GenericRenderTexture, lerpAnimationProgress, _evaluatedLerpProgress);
            }
        }

        public float LerpProgress => LerpCurve.Evaluate(LerpAnimationProgress);

        protected virtual float DeltaTime => Time.deltaTime;

        public void Initialize(Texture _startTexture, Texture _endTexture, float _initialProgress = 0f, UnityAction<RenderTexture> _onRenderTextureCreated = null)
        {
            StartTexture = _startTexture;
            EndTexture = _endTexture;
            Initialize(_initialProgress, _onRenderTextureCreated);
        }

        public void Initialize(float _initialProgress = 0f, UnityAction<RenderTexture> _onRenderTextureCreated = null)
        {
            if (StartTexture != null == false && EndTexture != null == false)
            {
                MyLog.Log($"RENDER TEXTURE LERP CONTROLLER :: Invalid textures!");
                return;
            }

            size = StartTexture.Size();

            if (size != EndTexture.Size())
            {
                MyLog.Log($"RENDER TEXTURE LERP CONTROLLER :: Cannot lerp textures with different sizes!");
                return;
            }

            GenericRenderTexture = new RenderTexture(size.x, size.y, 0, GraphicsFormat).WithName("generic_NewLerpTexture");
            GenericRenderTexture.enableRandomWrite = true;
            GenericRenderTexture.wrapMode = TextureWrapMode;
            GenericRenderTexture.Create();

            createdRenderTextures.AddIfNotContains(GenericRenderTexture);

            _onRenderTextureCreated?.Invoke(GenericRenderTexture);
            OnRenderTextureCreated?.Invoke(GenericRenderTexture);

            LerpShader.SetTexture(0, "Result", GenericRenderTexture);
            LerpShader.SetTexture(0, "StartTexture", StartTexture);
            LerpShader.SetTexture(0, "EndTexture", EndTexture);

            LerpAnimationProgress = _initialProgress;

            tryToClearOldRenderTextures();
        }

        public IEnumerator Lerp(float _duration, UnityAction _onComplete = null)
        {
            if (_duration <= 0)
            {
                _onComplete?.Invoke();
                yield break;
            }

            float _timeMultiplier = 1f / _duration;
            float _internalProgress = 0f;

            LerpAnimationProgress = _internalProgress;

            while (_internalProgress < 1f)
            {
                yield return null;
                _internalProgress += DeltaTime * _timeMultiplier;
                LerpAnimationProgress = _internalProgress.ClampMax(1f);
            }

            _onComplete?.Invoke();
        }

        public void Dispose()
        {
            tryToClearAllGeneratedTextures();
            lerpAnimationProgress = 0f;
        }

        protected void tryToClearOldRenderTextures()
        {
            int _count = createdRenderTextures.Count;

            for (int i = 0; i < _count; i++)
            {
                RenderTexture _rt = createdRenderTextures[i];

                if (_rt != GenericRenderTexture
                    && _rt != StartTexture
                    && _rt != EndTexture)
                {
                    _rt.ClearSpawnedRenderTexture(GENERIC_RT_NAME);
                }
            }
        }

        protected void tryToClearAllGeneratedTextures()
        {
            int _count = createdRenderTextures.Count;

            for (int i = 0; i < _count; i++)
            {
                createdRenderTextures[i].ClearSpawnedRenderTexture(GENERIC_RT_NAME);
            }
        }
    }

    [System.Serializable]
    public class RenderTextureLerpControllerRealtime : RenderTextureLerpController
    {
        protected override float DeltaTime => Time.unscaledDeltaTime;
    }

    public static void GetUVMinMax(this Sprite _sprite, out Vector2 _minUV, out Vector2 _maxUV)
    {
        _minUV = new Vector2(float.MaxValue, float.MaxValue);
        _maxUV = new Vector2(float.MinValue, float.MinValue);

        int _uvCount = _sprite.uv.Length;

        for (int i = 0; i < _uvCount; i++)
        {
            Vector2 _uv = _sprite.uv[i];

            if (_uv.x < _minUV.x)
            {
                _minUV.x = _uv.x;
            }
            else if (_uv.x > _maxUV.x)
            {
                _maxUV.x = _uv.x;
            }

            if (_uv.y < _minUV.y)
            {
                _minUV.y = _uv.y;
            }
            else if (_uv.y > _maxUV.y)
            {
                _maxUV.y = _uv.y;
            }
        }
    }

    public static void GetUVScaleAndOffset(this Sprite _sprite, out Vector2 _scale, out Vector2 _offset)
    {
        _sprite.GetUVMinMax(out _offset, out Vector2 _maxUV);
        _scale = _maxUV - _offset;
    }

    public static RenderTexture GetRenderTextureWithMaterial(this Sprite _sprite, Material _material, GraphicsFormat _format = GraphicsFormat.R8G8B8A8_UNorm, string _rtName = GENERIC_RT_NAME)
    {
        Vector2Int _size = _sprite.rect.size.ToInt();
        _sprite.GetUVScaleAndOffset(out Vector2 _scale, out Vector2 _offset);

        RenderTexture _newTexture = new RenderTexture(_size.x, _size.y, 0, _format).WithName(_rtName);
        RenderTexture _temp = RenderTexture.GetTemporary(_size.x, _size.y, 0, _format);

        Graphics.Blit(_sprite.texture, _temp, _scale, _offset);
        Graphics.Blit(_temp, _newTexture, _material);

        RenderTexture.ReleaseTemporary(_temp);
        return _newTexture;
    }

    public static RenderTexture GetUpdatedRenderTextureWithMaterial(this Sprite _sprite, RenderTexture _renderTextureToUpdate, Material _material, out bool _newTextureCreated)
    {
        if (_renderTextureToUpdate == null)
        {
            _newTextureCreated = true;
            return _sprite.GetRenderTextureWithMaterial(_material);
        }

        Vector2Int _size = _sprite.rect.size.ToInt();

        if (_renderTextureToUpdate.Size() != _size)
        {
            MyLog.Log($"Sprite size({_size}) is different than size of the render texture({_renderTextureToUpdate.Size()})! " +
                $"Setting size of already created render texture is not supported! " +
                $"New render texture will be created! Old render texture should be removed manually!");

            _newTextureCreated = true;
            return _sprite.GetRenderTextureWithMaterial(_material);
        }

        RenderTexture _temp = RenderTexture.GetTemporary(_size.x, _size.y, 0, GraphicsFormat.R8G8B8A8_UNorm);

        _sprite.GetUVScaleAndOffset(out Vector2 _scale, out Vector2 _offset);
        Graphics.Blit(_sprite.texture, _temp, _scale, _offset);
        Graphics.Blit(_temp, _renderTextureToUpdate, _material);

        RenderTexture.ReleaseTemporary(_temp);

        _newTextureCreated = false;
        return _renderTextureToUpdate;
    }

    /// <returns>Size of the Texture in pixels.</returns>
    public static Vector2Int Size(this Texture _texture)
    {
        return new Vector2Int(_texture.width, _texture.height);
    }

    public static void ClearSpawnedRenderTexture(this RenderTexture _renderTexture, string _renderTextureName = GENERIC_RT_NAME)
    {
        if (_renderTexture == null)
        {
            return;
        }

        if (RenderTexture.active == _renderTexture)
        {
            RenderTexture.active = null;
        }

        _renderTexture.DiscardContents();
        _renderTexture.Release();

#if UNITY_EDITOR
        if (_renderTexture.name == _renderTextureName)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(_renderTexture);
            }
            else
            {
                Object.DestroyImmediate(_renderTexture);
            }
        }
#else
            Object.Destroy(_renderTexture);
#endif
    }

    public static void DestroySpawnedRenderTexture(this RenderTexture _renderTexture)
    {
        if (_renderTexture == null)
        {
            return;
        }

        if (RenderTexture.active == _renderTexture)
        {
            RenderTexture.active = null;
        }

        if (_renderTexture.IsCreated())
        {
            _renderTexture.DiscardContents();
            _renderTexture.Release();
        }

#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            Object.Destroy(_renderTexture);
        }
        else
        {
            Object.DestroyImmediate(_renderTexture);
        }
#else
        UnityEngine.Object.Destroy(_rt);
#endif
    }

    public static void UpdateSpawnedRenderTextureSize(ref RenderTexture _texture, string _name, int _width, int _height, string _logPrefix = "", UnityAction<RenderTexture> _onNewCreated = null, int _depth = 0, GraphicsFormat _format = GraphicsFormat.R32G32B32A32_SFloat, GraphicsFormat _depthFormat = GraphicsFormat.D32_SFloat, bool _logCondition = true)
    {
        _width = _width.ClampMin(1);
        _height = _height.ClampMin(1);

        if (_texture != null)
        {
            if (_texture.width == _width && _texture.height == _height)
            {
                MyLog.Log(_logPrefix + $"Render Texture with size: {_width}x{_height} already exist! There is no need to create new one!", _condition: _logCondition);
                return;
            }
            else
            {
                _texture.DestroySpawnedRenderTexture();
            }
        }

        MyLog.Log(_logPrefix + $"Creating new Render Texture with size: {_width}x{_height}", _condition: _logCondition);

        _texture = new RenderTexture(_width, _height, _format, _depthFormat);
        _texture.name = _name;
        _texture.Create();

        _onNewCreated?.Invoke(_texture);
    }

    public static Color GetAverageColor(this Texture _texture)
    {
        return _texture.GetAverageColor(_texture != null ? _texture.Size() : default);
    }

    public static Color GetAverageColor(this Texture _texture, Vector2Int _tempRTSize)
    {
        return _texture.GetAverageColor(_tempRTSize.x, _tempRTSize.y);
    }

    public static Color GetAverageColor(this Texture _texture, int _tempRTWidth = 64, int _tempRTHeight = 64)
    {
        if (_texture == null)
        {
            return Color.clear;
        }

        RenderTexture _tempRT = (QualitySettings.activeColorSpace != ColorSpace.Linear)
            ? RenderTexture.GetTemporary(_tempRTWidth, _tempRTHeight, 0, RenderTextureFormat.ARGB32)
            : RenderTexture.GetTemporary(_tempRTWidth, _tempRTHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

        _tempRT.filterMode = FilterMode.Point;
        _tempRT.Create();

        Graphics.Blit(_texture, _tempRT);

        Texture2D _tempTexture2D = null;
        _tempRT.ToTexture2D(ref _tempTexture2D);

        Color[] _pixels = _tempTexture2D.GetPixels();
        Color _result = new Color(0f, 0f, 0f, 0f);

        for (int i = 0; i < _pixels.Length; i++)
        {
            _result += _pixels[i];
        }

        _result /= (float)_pixels.Length;

        RenderTexture.ReleaseTemporary(_tempRT);
        UnityEngine.Object.DestroyImmediate(_tempTexture2D);

        return _result;
    }

    public static void ToTexture2D(this RenderTexture _rt, ref Texture2D _texture2D)
    {
        if (_rt == null)
        {
            return;
        }

        RenderTexture _previousActiveRT = RenderTexture.active;
        RenderTexture.active = _rt;

        Vector2Int _rtSize = _rt.Size();

        if (_texture2D == null)
        {
            _texture2D = CreateTexture2D(_rtSize);
            _texture2D.wrapMode = _rt.wrapMode;
            _texture2D.name = _rt.name;
        }
        else if (_texture2D.Size() != _rtSize)
        {
            _texture2D.Reinitialize(_rtSize.x, _rtSize.y, TextureFormat.ARGB32, hasMipMap: true);
        }

        _texture2D.ReadPixels(new Rect(0f, 0f, _rt.width, _rt.height), 0, 0);
        _texture2D.Apply();

        RenderTexture.active = Application.isPlaying ? _previousActiveRT : null;
    }

    public static Texture2D CreateTexture2D(Vector2Int _size, bool _mipmaps = false)
    {
        return CreateTexture2D(_size.x, _size.y, _mipmaps);
    }

    public static Texture2D CreateTexture2D(int _width, int _height, bool _mipmaps = false)
    {
        return QualitySettings.activeColorSpace == ColorSpace.Linear
            ? new Texture2D(_width, _height, TextureFormat.ARGB32, _mipmaps, linear: true)
            : new Texture2D(_width, _height, TextureFormat.ARGB32, _mipmaps);
    }

    public static Texture2D CreateTexture2D(int _width, int _height, Color _color)
    {
        Color[] _pixels = new Color[_width * _height];

        for (int i = 0; i < _pixels.Length; i++)
        {
            _pixels[i] = _color;
        }

        Texture2D _result = new Texture2D(_width, _height);
        _result.SetPixels(_pixels);
        _result.Apply();

        return _result;
    }
}
