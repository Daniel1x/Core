namespace DL.TextureSaver
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class TextureSaver : MonoBehaviour
    {
        private static readonly List<IMaterialModifier> materialModifiers = new List<IMaterialModifier>();

        public void SaveTexture(string _directoryPath, string _filename, TextureFormat _format = TextureFormat.RGBA32) => SaveTexture(gameObject, _directoryPath, _filename, _format);

        [MenuItem("CONTEXT/Image/Save Image Texture")]
        private static void SaveImageTextureToPNG(MenuCommand _command)
        {
            if (_command.context is Image _img)
            {
                SaveTexture(_img.gameObject, _filename: "t_" + _img.gameObject.name.RemoveSpaces());
            }
        }

        public static void SaveTexture(GameObject _objectWithImage, string _directoryPath = "Assets", string _filename = "t_New", TextureFormat _format = TextureFormat.RGBA32)
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
                _materialInstance.SetTexture("_MainTex", _spriteTexture);
            }

            int _width = (int)_rectTransform.rect.width;
            int _height = (int)_rectTransform.rect.height;

            RenderTexture _renderTexture = new RenderTexture(_width, _height, 0);
            RenderTexture _previousRT = RenderTexture.active;
            RenderTexture.active = _renderTexture;

            GL.Clear(true, true, Color.clear);
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, _width, _height, 0);
            Graphics.DrawTexture(new Rect(0, 0, _width, _height), _spriteTexture != null ? _spriteTexture : Texture2D.whiteTexture, _materialInstance);
            GL.PopMatrix();

            Texture2D _texture = new Texture2D(_width, _height, _format, false);
            _texture.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
            _texture.Apply();

            RenderTexture.active = _previousRT;
            _renderTexture.Release();

            Texture2D _optimizedTexture = _texture.OptimalizeTextureFor9Slicing(out Vector4 _border);

            if (_optimizedTexture != _texture)
            {
                //Destroy the old texture to free memory
                Object.DestroyImmediate(_texture);

                _texture = _optimizedTexture;
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
                TextureImporter _importer = TextureImporter.GetAtPath(_path) as TextureImporter;

                if (_importer != null)
                {
                    _importer.textureType = TextureImporterType.Sprite;
                    _importer.spriteImportMode = SpriteImportMode.Single;
                    _importer.alphaIsTransparency = true;

                    if (_border != default)
                    {
                        _importer.spriteBorder = _border;
                    }

                    _importer.SaveAndReimport();
                }
            }
            else
            {
                Debug.LogError("Failed to encode texture to PNG.");
            }

            DestroyImmediate(_materialInstance);
            DestroyImmediate(_texture);
            DestroyImmediate(_renderTexture);
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
#endif
        }
    }
}
