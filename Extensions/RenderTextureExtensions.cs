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
        protected const string SHADER_KERNEL_NAME = "RenderTextureLerpMain";
        protected const string GENERIC_RT_NAME = "generic_NewLerpTexture";
        protected const int THREAD_COUNT = 8;

        public event UnityAction<RenderTexture> OnRenderTextureCreated = null;
        ///<summary>(RenderTexture, LerpAnimationProgress, LerpProgress)</summary>
        public event UnityAction<RenderTexture, float, float> OnRenderTextureUpdate = null;

        [Header("Settings")]
        public ComputeShader LerpShader = null;
        public Texture StartTexture = null;
        public Texture EndTexture = null;
        public TextureWrapMode TextureWrapMode = TextureWrapMode.Repeat;
        public GraphicsFormat GraphicsFormat = GraphicsFormat.R32G32B32A32_SFloat;
        public AnimationCurve LerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public RenderTexture GenericRenderTexture { get; private set; } = null;

        protected readonly List<RenderTexture> createdRenderTextures = new List<RenderTexture>();
        protected float lerpAnimationProgress = 0f;
        protected Vector2Int size = default;
        protected int kernelId = -1;

        public float LerpAnimationProgress
        {
            get => lerpAnimationProgress;
            private set
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || GenericRenderTexture == null)
                {
                    return;
                }
#endif
                value = Mathf.Clamp01(value);

                if (!Mathf.Approximately(lerpAnimationProgress, value))
                {
                    lerpAnimationProgress = value;
                }

                dispatchCompute();
            }
        }

        public float LerpProgress => LerpCurve.Evaluate(lerpAnimationProgress);
        protected virtual float DeltaTime => Time.deltaTime;

        #region Public API

        public void Initialize(Texture _startTexture, Texture _endTexture, float _initialProgress = 0f, UnityAction<RenderTexture> _onRenderTextureCreated = null)
        {
            StartTexture = _startTexture;
            EndTexture = _endTexture;
            Initialize(_initialProgress, _onRenderTextureCreated);
        }

        public void Initialize(float _initialProgress = 0f, UnityAction<RenderTexture> _onRenderTextureCreated = null)
        {
            if (validateInput() == false)
            {
                return;
            }

            obtainKernel();

            if (kernelId < 0)
            {
                MyLog.Log("RENDER TEXTURE LERP CONTROLLER :: Kernel 'CSMain' not found!", Color.yellow);
                return;
            }

            allocateRenderTexture(_onRenderTextureCreated);

            if (GenericRenderTexture == null)
            {
                MyLog.Log("RENDER TEXTURE LERP CONTROLLER :: Failed to create RenderTexture!", Color.red);
                return;
            }

            bindResources();

            LerpAnimationProgress = _initialProgress;
            tryToClearOldRenderTextures();
        }

        public IEnumerator Lerp(float _duration, UnityAction _onComplete = null)
        {
            if (_duration <= 0f)
            {
                SetProgress(1f);
                _onComplete?.Invoke();
                yield break;
            }

            float _timeMultiplier = 1f / _duration;
            float _internalProgress = 0f;
            SetProgress(0f);

            while (_internalProgress < 1f)
            {
                yield return null;
                _internalProgress += DeltaTime * _timeMultiplier;
                SetProgress(_internalProgress);
            }

            SetProgress(1f);
            _onComplete?.Invoke();
        }

        public void SetProgress(float _value)
        {
            LerpAnimationProgress = _value;
        }

        public void ForceUpdate()
        {
            dispatchCompute();
        }

        public void Dispose()
        {
            tryToClearAllGeneratedTextures();
            lerpAnimationProgress = 0f;
            GenericRenderTexture = null;
        }

        #endregion

        #region Internal

        private bool validateInput()
        {
            if (LerpShader == null)
            {
                MyLog.Log("RENDER TEXTURE LERP CONTROLLER :: Missing ComputeShader!", Color.red);
                return false;
            }

            if (StartTexture == null || EndTexture == null)
            {
                MyLog.Log("RENDER TEXTURE LERP CONTROLLER :: Start or End texture is null!", Color.red);
                return false;
            }

            size = StartTexture.Size();

            if (size.x <= 0 || size.y <= 0)
            {
                MyLog.Log("RENDER TEXTURE LERP CONTROLLER :: Invalid texture size!", Color.red);
                return false;
            }

            if (size != EndTexture.Size())
            {
                MyLog.Log("RENDER TEXTURE LERP CONTROLLER :: Cannot lerp textures with different sizes!", Color.red);
                return false;
            }

            return true;
        }

        private void obtainKernel()
        {
            if (LerpShader == null)
            {
                kernelId = -1;
                return;
            }

            try
            {
                kernelId = LerpShader.FindKernel(SHADER_KERNEL_NAME);
            }
            catch
            {
                kernelId = -1;
            }
        }

        private void allocateRenderTexture(UnityAction<RenderTexture> _onRenderTextureCreated = null)
        {
            if (GenericRenderTexture != null)
            {
                if (GenericRenderTexture.IsCreated())
                {
                    GenericRenderTexture.Release();
                }

                GenericRenderTexture.ClearSpawnedRenderTexture(GENERIC_RT_NAME);
            }

            GenericRenderTexture = new RenderTexture(size.x, size.y, 0)
            {
                enableRandomWrite = true,
                graphicsFormat = GraphicsFormat,
                wrapMode = TextureWrapMode,
                name = GENERIC_RT_NAME
            };

            GenericRenderTexture.Create();
            createdRenderTextures.AddIfNotContains(GenericRenderTexture);

            _onRenderTextureCreated?.Invoke(GenericRenderTexture);
            OnRenderTextureCreated?.Invoke(GenericRenderTexture);
        }

        private void bindResources()
        {
            if (LerpShader == null || kernelId < 0)
            {
                return;
            }

            LerpShader.SetTexture(kernelId, "Result", GenericRenderTexture);
            LerpShader.SetTexture(kernelId, "StartTexture", StartTexture);
            LerpShader.SetTexture(kernelId, "EndTexture", EndTexture);
        }

        private void dispatchCompute()
        {
            if (LerpShader == null || GenericRenderTexture == null || kernelId < 0)
            {
                return;
            }

            float _evaluated = Mathf.Clamp01(LerpProgress);
            float _inverseEvaluated = 1f - _evaluated;

            LerpShader.SetFloat("Progress", _evaluated);
            LerpShader.SetFloat("InverseProgress", _inverseEvaluated);
            LerpShader.SetInts("TextureSize", size.x, size.y);

            int _groupsX = (size.x + THREAD_COUNT - 1) / THREAD_COUNT;
            int _groupsY = (size.y + THREAD_COUNT - 1) / THREAD_COUNT;

            LerpShader.Dispatch(kernelId, _groupsX, _groupsY, 1);

            OnRenderTextureUpdate?.Invoke(GenericRenderTexture, lerpAnimationProgress, _evaluated);
        }

        protected void tryToClearOldRenderTextures()
        {
            int _count = createdRenderTextures.Count;

            for (int i = _count - 1; i >= 0; i--)
            {
                RenderTexture _rt = createdRenderTextures[i];

                if (_rt == null)
                {
                    continue;
                }

                if (_rt != GenericRenderTexture
                    && _rt != StartTexture
                    && _rt != EndTexture)
                {
                    _rt.ClearSpawnedRenderTexture(GENERIC_RT_NAME);
                }

                createdRenderTextures.RemoveAt(i);
            }
        }

        protected void tryToClearAllGeneratedTextures()
        {
            int _count = createdRenderTextures.Count;

            for (int i = 0; i < _count; i++)
            {
                RenderTexture _rt = createdRenderTextures[i];

                if (_rt == null)
                {
                    continue;
                }

                _rt.ClearSpawnedRenderTexture(GENERIC_RT_NAME);
            }

            createdRenderTextures.Clear();
        }

        #endregion
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

    public static Texture2D OptimalizeTextureFor9Slicing(this Texture2D _texture, out Vector4 _border)
    {
        if (_texture == null)
        {
            _border = default;
            return _texture; //No texture to optimize
        }

        int _width = _texture.width;
        int _height = _texture.height;

        if (_width <= 2 || _height <= 2)
        {
            _border = default;
            return _texture; //Too small to optimize
        }

        Color[] _pixels = _texture.GetPixels();
        int _centerX = _width / 2;
        int _centerY = _height / 2;

        // Track contiguous removable duplicate rows / columns around the center.
        int _removeUp = _removeInDirection(_centerY, _height - 1, _areRowsSame);       // rows above center (greater y) identical to previous
        int _removeDown = _removeInDirection(_centerY, 0, _areRowsSame);               // rows below center (smaller y) identical to next
        int _removeRight = _removeInDirection(_centerX, _width - 1, _areColumnsSame);  // columns to the right of center identical
        int _removeLeft = _removeInDirection(_centerX, 0, _areColumnsSame);             // columns to the left of center identical

        int _newWidth = _width - (_removeLeft + _removeRight);
        int _newHeight = _height - (_removeUp + _removeDown);

        // No change
        if (_newWidth == _width && _newHeight == _height)
        {
            _border = default;
            return _texture;
        }

        Vector2Int _newCenterPosition = new Vector2Int(_centerX - _removeLeft, _centerY - _removeDown);
        int _leftBorder = _newCenterPosition.x;
        int _rightBorder = _newWidth - _newCenterPosition.x - 1;
        int _bottomBorder = _newCenterPosition.y;
        int _topBorder = _newHeight - _newCenterPosition.y - 1;

        _border = new Vector4(_leftBorder, _bottomBorder, _rightBorder, _topBorder);

        Debug.Log($"Optimized texture from {_width}x{_height} to {_newWidth}x{_newHeight}, New Center: {_newCenterPosition}, Border: L:{_leftBorder} R:{_rightBorder} B:{_bottomBorder} T:{_topBorder}");

        Texture2D _newTexture = new Texture2D(_newWidth, _newHeight, _texture.format, false);
        Color[] _newPixels = new Color[_newWidth * _newHeight];

        // Ranges of removed rows / columns
        int _removedTopEnd = _centerY + _removeUp;
        int _removedBottomStart = _centerY - _removeDown;
        int _removedRightEnd = _centerX + _removeRight;
        int _removedLeftStart = _centerX - _removeLeft;

        int _newY = 0;

        for (int y = 0; y < _height; y++)
        {
            if (_rowRemoved(y))
            {
                continue;
            }

            int _newX = 0;

            for (int x = 0; x < _width; x++)
            {
                if (_columnRemoved(x))
                {
                    continue;
                }

                _newPixels[_newY * _newWidth + _newX] = _pixels[_getPixelIndex(x, y)];
                _newX++;
            }

            _newY++;
        }

        _newTexture.SetPixels(_newPixels);
        _newTexture.Apply();
        return _newTexture;

        // Check if a row is removed
        bool _rowRemoved(int y) => (y > _centerY && y <= _removedTopEnd) || (y < _centerY && y >= _removedBottomStart);

        // Check if a column is removed
        bool _columnRemoved(int x) => (x > _centerX && x <= _removedRightEnd) || (x < _centerX && x >= _removedLeftStart);

        // Count how many rows / columns can be removed in a direction
        int _removeInDirection(int _start, int _end, System.Func<int, int, bool> _areSame)
        {
            int _removeCount = 0;

            for (int i = _start; i != _end; i += (i < _end ? 1 : -1))
            {
                if (_areSame(i, i + (i < _end ? 1 : -1)))
                {
                    _removeCount++;
                }
                else
                {
                    break;
                }
            }

            return _removeCount;
        }

        // Check if two rows are identical
        bool _areRowsSame(int y1, int y2)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_pixels[_getPixelIndex(x, y1)] != _pixels[_getPixelIndex(x, y2)])
                {
                    return false;
                }
            }
            return true;
        }

        // Check if two columns are identical
        bool _areColumnsSame(int x1, int x2)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_pixels[_getPixelIndex(x1, y)] != _pixels[_getPixelIndex(x2, y)])
                {
                    return false;
                }
            }

            return true;
        }

        // Converts 2D coordinates to 1D index
        int _getPixelIndex(int x, int y) => y * _width + x;
    }
}
