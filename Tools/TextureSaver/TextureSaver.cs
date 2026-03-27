namespace DL.TextureSaver
{
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class TextureSaver : MonoBehaviour
    {
        public enum GrayScaleAdjustments
        {
            None = 0,
            Average = 1,
            Luminosity = 2,
        }

        private static readonly List<IMaterialModifier> materialModifiers = new List<IMaterialModifier>();
        private static readonly MethodInfo onPopulateMeshMethod = typeof(Image).GetMethod("OnPopulateMesh", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(VertexHelper) }, null);

        public static void SaveTexture(GameObject _objectWithImage, string _directoryPath = "Assets", string _filename = "t_New", TextureFormat _format = TextureFormat.RGBA32, bool _optimizeFor9Slice = false, bool _pingObject = true, GrayScaleAdjustments _grayScale = GrayScaleAdjustments.None)
        {
#if !UNITY_EDITOR
            Debug.LogError("Texture saving is only supported in the Unity Editor.");
            return;
#else
            if (_objectWithImage == null)
            {
                Debug.LogError("No GameObject provided.");
                return;
            }

            RectTransform _rectTransform = _objectWithImage.GetComponent<RectTransform>();
            Image _image = _objectWithImage.GetComponent<Image>();

            if (_rectTransform == null || _image == null)
            {
                Debug.LogError("The provided GameObject must have both RectTransform and Image components.");
                return;
            }

            if (_image.sprite == null && _image.material == null)
            {
                Debug.LogError("No sprite or material found on the Image component.");
                return;
            }

            //Clone material to avoid modifying the original
            Material _materialInstance = _image.material != null ? Instantiate(_image.material) : null;
            Texture2D _spriteTexture = _image.sprite != null ? _image.sprite.texture : null;

            if (_materialInstance != null)
            {
                materialModifiers.Clear();
                _image.GetComponents(materialModifiers);

                foreach (IMaterialModifier _modifier in materialModifiers)
                {
                    Material _modifiedMaterialInstance = _modifier.GetModifiedMaterial(_materialInstance);

                    if (_modifiedMaterialInstance != null && _modifiedMaterialInstance != _materialInstance)
                    {
                        DestroyImmediate(_materialInstance);
                        _materialInstance = _modifiedMaterialInstance;
                    }
                }

                //If the material is using the default UI shader, we need to set the main texture to the sprite's texture
                if (_materialInstance.HasTexture("_MainTex"))
                {
                    _materialInstance.SetTexture("_MainTex", _spriteTexture);
                }
            }

            int _width = (int)_rectTransform.rect.width;
            int _height = (int)_rectTransform.rect.height;

            calculateOptimalResolution(ref _width, ref _height, _spriteTexture);

            Texture2D _textureToDraw = _spriteTexture != null ? _spriteTexture : Texture2D.whiteTexture;
            RenderTexture _renderTexture = new RenderTexture(_width, _height, 0)
            {
                useMipMap = false,
                autoGenerateMips = false,
                antiAliasing = 1,
            };

            RenderTexture _previousActiveRT = RenderTexture.active;
            _renderTexture.Create();
            RenderTexture.active = _renderTexture;

            bool _isSimple = _image.type == Image.Type.Simple;

            if (_isSimple)
            {
                // Simple mode: standard blit works correctly
                if (_materialInstance != null)
                {
                    Graphics.Blit(_textureToDraw, _renderTexture, _materialInstance);
                }
                else
                {
                    Graphics.Blit(_textureToDraw, _renderTexture);
                }
            }
            else
            {
                // Sliced/Tiled/Filled: generate mesh via reflection and render it manually
                renderImageMesh(_image, _renderTexture, _materialInstance, _textureToDraw, _width, _height);
            }

            Texture2D _texture = new Texture2D(_width, _height, _format, false);

            // Read pixels from RenderTexture
            _texture.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
            _texture.Apply();

            RenderTexture.active = _previousActiveRT;
            _renderTexture.Release();

            Vector4 _border = default;

            if (_optimizeFor9Slice)
            {
                Texture2D _optimizedTexture = _texture.OptimalizeTextureFor9Slicing(out _border);

                if (_optimizedTexture != _texture)
                {
                    //Destroy the old texture to free memory
                    Object.DestroyImmediate(_texture);
                    _texture = _optimizedTexture;

                    _width = _texture.width;
                    _height = _texture.height;
                }
            }

            if (_grayScale is not GrayScaleAdjustments.None)
            {
                Color32[] _pixels = _texture.GetPixels32();
                bool _useLuminosity = _grayScale is GrayScaleAdjustments.Luminosity;

                for (int i = 0; i < _pixels.Length; i++)
                {
                    byte _gray = _pixels[i].GetGrayScale(_useLuminosity);
                    _pixels[i] = new Color32(_gray, _gray, _gray, _pixels[i].a);
                }

                _texture.SetPixels32(_pixels);
                _texture.Apply();
            }

            byte[] _pngData = _texture.EncodeToPNG();

            if (_pngData != null)
            {
                string _path = System.IO.Path.Combine(_directoryPath, _filename + ".png");
                System.IO.File.WriteAllBytes(_path, _pngData);
                Debug.Log($"Texture saved to {_path}");

                //Refresh the AssetDatabase to show the saved texture in the Project window
                UnityEditor.AssetDatabase.Refresh();

                //Update texture import settings to make it readable and set the correct format
                var _importer = UnityEditor.AssetImporter.GetAtPath(_path) as UnityEditor.TextureImporter;

                if (_importer != null)
                {
                    int _maxSize = Mathf.Max(_width, _height);

                    _importer.textureType = UnityEditor.TextureImporterType.Sprite;
                    _importer.spriteImportMode = UnityEditor.SpriteImportMode.Single;
                    _importer.alphaIsTransparency = true;
                    _importer.maxTextureSize = Mathf.NextPowerOfTwo(_maxSize);

                    if (_border != default)
                    {
                        _importer.spriteBorder = _border;
                    }

                    _importer.SaveAndReimport();
                }

                if (_pingObject)
                {
                    UnityEditor.EditorGUIUtility.PingObject(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(_path));
                }
            }
            else
            {
                Debug.LogError("Failed to encode texture to PNG.");
            }

            DestroyImmediate(_materialInstance);
            DestroyImmediate(_texture);
            DestroyImmediate(_renderTexture);

            //Clear references
            _materialInstance = null;
            _texture = null;
            _renderTexture = null;

            //Force garbage collection to free memory
            Resources.UnloadUnusedAssets();
            System.GC.Collect(0, System.GCCollectionMode.Forced, true, true);
#endif
        }

        /// <summary>
        /// Calculates the optimal output resolution by comparing the RectTransform size with the sprite texture size.
        /// The aspect ratio from the RectTransform is always preserved.
        /// If the sprite has a higher resolution than the rect, the output is scaled up to match the sprite's
        /// largest dimension while maintaining the rect's aspect ratio.
        /// </summary>
        private static void calculateOptimalResolution(ref int _width, ref int _height, Texture2D _spriteTexture)
        {
            if (_width <= 0 || _height <= 0)
            {
                return;
            }

            if (_spriteTexture == null)
            {
                return;
            }

            int _spriteWidth = _spriteTexture.width;
            int _spriteHeight = _spriteTexture.height;
            float _rectAspect = (float)_width / _height;

            // Check if sprite resolution is higher than rect in at least one dimension
            if (_spriteWidth <= _width && _spriteHeight <= _height)
            {
                return;
            }

            // Scale up to fit the sprite's largest useful dimension while keeping rect's aspect ratio
            // Try fitting by width first, then by height, and pick whichever gives the larger result
            // without exceeding both sprite dimensions unnecessarily
            int _fitByWidthW = _spriteWidth;
            int _fitByWidthH = Mathf.RoundToInt(_spriteWidth / _rectAspect);

            int _fitByHeightH = _spriteHeight;
            int _fitByHeightW = Mathf.RoundToInt(_spriteHeight * _rectAspect);

            // Pick the fit that results in the largest area without going below original rect size
            long _areaByWidth = (long)_fitByWidthW * _fitByWidthH;
            long _areaByHeight = (long)_fitByHeightW * _fitByHeightH;

            if (_areaByWidth >= _areaByHeight)
            {
                _width = _fitByWidthW;
                _height = _fitByWidthH;
            }
            else
            {
                _width = _fitByHeightW;
                _height = _fitByHeightH;
            }

            // Ensure minimum of 1 pixel in each dimension
            _width = Mathf.Max(_width, 1);
            _height = Mathf.Max(_height, 1);
        }

        /// <summary>
        /// Renders the Image mesh (Sliced/Tiled/Filled) onto the RenderTexture using GL commands.
        /// Uses reflection to call the protected <c>OnPopulateMesh</c> method, which generates
        /// correct vertices and UVs for all Image.type modes.
        /// </summary>
        private static void renderImageMesh(Image _image, RenderTexture _renderTexture, Material _material, Texture2D _texture, int _width, int _height)
        {
            if (onPopulateMeshMethod == null)
            {
                Debug.LogError("Failed to find OnPopulateMesh method via reflection. Falling back to simple Blit.");

                if (_material != null)
                {
                    Graphics.Blit(_texture, _renderTexture, _material);
                }
                else
                {
                    Graphics.Blit(_texture, _renderTexture);
                }

                return;
            }

            // Generate mesh data via Image's internal OnPopulateMesh
            using (VertexHelper _vertexHelper = new VertexHelper())
            {
                onPopulateMeshMethod.Invoke(_image, new object[] { _vertexHelper });

                Mesh _mesh = new Mesh();
                _vertexHelper.FillMesh(_mesh);

                // Remap vertices from RectTransform local space to pixel coordinates
                Rect _rect = _image.rectTransform.rect;
                Vector3[] _vertices = _mesh.vertices;

                for (int i = 0; i < _vertices.Length; i++)
                {
                    float _normalizedX = (_vertices[i].x - _rect.x) / _rect.width;
                    float _normalizedY = (_vertices[i].y - _rect.y) / _rect.height;

                    _vertices[i] = new Vector3(_normalizedX * _width, _normalizedY * _height, 0f);
                }

                _mesh.vertices = _vertices;

                // Clear to transparent
                GL.Clear(true, true, Color.clear);

                // Setup orthographic pixel-perfect projection
                GL.PushMatrix();
                GL.LoadPixelMatrix(0, _width, 0, _height);

                // Use the material or a default UI material
                Material _drawMaterial = _material != null ? _material : new Material(Shader.Find("UI/Default"));

                if (_drawMaterial.HasTexture("_MainTex"))
                {
                    _drawMaterial.SetTexture("_MainTex", _texture != null ? _texture : Texture2D.whiteTexture);
                }

                _drawMaterial.SetPass(0);
                Graphics.DrawMeshNow(_mesh, Matrix4x4.identity);

                GL.PopMatrix();

                DestroyImmediate(_mesh);

                if (_material == null)
                {
                    DestroyImmediate(_drawMaterial);
                }
            }
        }
    }
}
