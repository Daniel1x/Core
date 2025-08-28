namespace DL.TextureSaver
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class TextureSaver : MonoBehaviour
    {
        private static readonly List<IMaterialModifier> materialModifiers = new List<IMaterialModifier>();

        public static void SaveTexture(GameObject _objectWithImage, string _directoryPath = "Assets", string _filename = "t_New", TextureFormat _format = TextureFormat.RGBA32, bool _optimizeFor9Slice = false, bool _pingObject = true)
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

            if (_materialInstance != null)
            {
                Graphics.Blit(_textureToDraw, _renderTexture, _materialInstance);
            }
            else
            {
                Graphics.Blit(_textureToDraw, _renderTexture);
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
    }
}
