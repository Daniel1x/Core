namespace DL.SpriteUtilities.Editor
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public sealed class SpriteEditionPopup : PopupWindowContent
    {
        private const int k_MinimalCompressionSizeStep = 4;

        private readonly Texture2D texture;
        private readonly string assetPath;
        private readonly Vector2Int originalSize;

        private Vector2 scroll;
        private int trimLeft;
        private int trimRight;
        private int trimTop;
        private int trimBottom;

        private bool keepToMultipleOf4 = true;

        public static void Open(Texture2D _texture, Vector2 _screenPosition)
        {
            if (_texture == null)
            {
                EditorUtility.DisplayDialog("Trim Texture", "No texture selected.", "OK");
                return;
            }

            string _path = AssetDatabase.GetAssetPath(_texture);

            if (string.IsNullOrWhiteSpace(_path))
            {
                EditorUtility.DisplayDialog("Trim Texture", "Could not determine asset path.", "OK");
                return;
            }

            var _popup = new SpriteEditionPopup(_texture, _path);
            PopupWindow.Show(new Rect(_screenPosition, Vector2.zero), _popup);
        }

        private SpriteEditionPopup(Texture2D _texture, string _assetPath)
        {
            assetPath = _assetPath;
            originalSize = new Vector2Int(_texture.width, _texture.height);
            texture = _texture;

            Vector2Int _toTrim = calculatePixelsToTrimToMultipleOf(originalSize, k_MinimalCompressionSizeStep);

            trimLeft = 0;
            trimRight = _toTrim.x;
            trimTop = 0;
            trimBottom = _toTrim.y;
        }

        public override Vector2 GetWindowSize() => new Vector2(500, 400f);

        public override void OnGUI(Rect _rect)
        {
            using (var _scope = new EditorGUILayout.ScrollViewScope(scroll))
            {
                scroll = _scope.scrollPosition;

                EditorGUILayout.LabelField("Trim Texture", EditorStyles.boldLabel);
                EditorGUILayout.Space(4);

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField("Asset", assetPath);
                    EditorGUILayout.LabelField("Size", $"{originalSize.x} x {originalSize.y}");
                }

                EditorGUILayout.Space(4);

                keepToMultipleOf4 = EditorGUILayout.ToggleLeft("Keep size a multiple of 4 (better compression)", keepToMultipleOf4);

                Vector2Int _suggested = calculatePixelsToTrimToMultipleOf(originalSize, k_MinimalCompressionSizeStep);
                Vector2Int _newSize = new Vector2Int(
                    originalSize.x - (trimLeft + trimRight),
                    originalSize.y - (trimTop + trimBottom));

                EditorGUILayout.Space(2);

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField("Suggested (to reach a multiple of 4)", $"{_suggested.x} px (X), {_suggested.y} px (Y)");
                    EditorGUILayout.LabelField("New size", $"{Mathf.Max(0, _newSize.x)} x {Mathf.Max(0, _newSize.y)}");
                }

                EditorGUILayout.Space(6);
                EditorGUILayout.LabelField("Trim (pixels)", EditorStyles.boldLabel);

                using (new EditorGUILayout.HorizontalScope())
                {
                    trimLeft = EditorGUILayout.IntField("Left", trimLeft);
                    trimRight = EditorGUILayout.IntField("Right", trimRight);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    trimTop = EditorGUILayout.IntField("Top", trimTop);
                    trimBottom = EditorGUILayout.IntField("Bottom", trimBottom);
                }

                clampTrimsToValid(ref trimLeft, ref trimRight, ref trimTop, ref trimBottom, originalSize);

                if (keepToMultipleOf4)
                {
                    int _newW = originalSize.x - (trimLeft + trimRight);
                    int _newH = originalSize.y - (trimTop + trimBottom);
                    int _modX = _newW % k_MinimalCompressionSizeStep;
                    int _modY = _newH % k_MinimalCompressionSizeStep;

                    if (_modX != 0)
                    {
                        if (trimRight >= _modX)
                        {
                            trimRight += _modX;
                        }
                        else if (trimLeft >= _modX)
                        {
                            trimLeft += _modX;
                        }
                    }

                    if (_modY != 0)
                    {
                        if (trimBottom >= _modY)
                        {
                            trimBottom += _modY;
                        }
                        else if (trimTop >= _modY)
                        {
                            trimTop += _modY;
                        }
                    }

                    clampTrimsToValid(ref trimLeft, ref trimRight, ref trimTop, ref trimBottom, originalSize);
                }

                EditorGUILayout.Space(8);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Calculate Trim For Black"))
                    {
                        calculateBlackPixelsToTrim();
                    }

                    if (GUILayout.Button("Calculate Trim For Transparent"))
                    {
                        calculateTransparentPixelsToTrim();
                    }
                }

                EditorGUILayout.Space(8);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Trim", GUILayout.Width(60)))
                    {
                        tryTrimAndSave();
                    }

                    if (GUILayout.Button("Close", GUILayout.Width(60)))
                    {
                        editorWindow.Close();
                    }
                }
            }
        }

        private void checkImageForValidPixels(System.Predicate<Color> _canTrim, out int _left, out int _right, out int _top, out int _bottom)
        {
            _left = 0;
            _right = 0;
            _top = 0;
            _bottom = 0;

            if (texture == null || _canTrim == null)
            {
                return;
            }

            if (texture.isReadable == false)
            {
                EditorUtility.DisplayDialog("Trim Texture", "Texture is not readable. Please check the import settings.", "OK");
                return;
            }

            Vector2Int _size = new Vector2Int(texture.width, texture.height);
            Color[] _pixels = texture.GetPixels();

            for (int x = 0; x < _size.x; x++)
            {
                bool _found = false;

                for (int y = 0; y < _size.y; y++)
                {
                    Color _pixel = _pixels[y * _size.x + x];

                    if (!_canTrim(_pixel))
                    {
                        _found = true;
                        break;
                    }
                }

                if (_found)
                {
                    break;
                }

                _left++;
            }

            for (int x = _size.x - 1; x >= 0; x--)
            {
                bool _found = false;

                for (int y = 0; y < _size.y; y++)
                {
                    Color _pixel = _pixels[y * _size.x + x];

                    if (!_canTrim(_pixel))
                    {
                        _found = true;
                        break;
                    }
                }

                if (_found)
                {
                    break;
                }

                _right++;
            }

            for (int y = 0; y < _size.y; y++)
            {
                bool _found = false;

                for (int x = 0; x < _size.x; x++)
                {
                    Color _pixel = _pixels[y * _size.x + x];

                    if (!_canTrim(_pixel))
                    {
                        _found = true;
                        break;
                    }
                }

                if (_found)
                {
                    break;
                }

                _bottom++;
            }

            for (int y = _size.y - 1; y >= 0; y--)
            {
                bool _found = false;

                for (int x = 0; x < _size.x; x++)
                {
                    Color _pixel = _pixels[y * _size.x + x];

                    if (!_canTrim(_pixel))
                    {
                        _found = true;
                        break;
                    }
                }

                if (_found)
                {
                    break;
                }

                _top++;
            }

            if (keepToMultipleOf4)
            {
                int _widthTrimmed = _size.x - (_left + _right);
                int _heightTrimmed = _size.y - (_top + _bottom);
                int _modX = _widthTrimmed % k_MinimalCompressionSizeStep;
                int _modY = _heightTrimmed % k_MinimalCompressionSizeStep;

                if (_modX != 0)
                {
                    if (_right >= _modX)
                    {
                        _right += _modX;
                    }
                    else if (_left >= _modX)
                    {
                        _left += _modX;
                    }
                }

                if (_modY != 0)
                {
                    if (_bottom >= _modY)
                    {
                        _bottom += _modY;
                    }
                    else if (_top >= _modY)
                    {
                        _top += _modY;
                    }
                }
            }
        }

        private void calculateBlackPixelsToTrim()
        {
            checkImageForValidPixels(
                _canTrim,
                out trimLeft,
                out trimRight,
                out trimTop,
                out trimBottom);

            bool _canTrim(Color _p) => _p.r == 0f && _p.g == 0f && _p.b == 0f;
        }

        private void calculateTransparentPixelsToTrim()
        {
            checkImageForValidPixels(
                _canTrim,
                out trimLeft,
                out trimRight,
                out trimTop,
                out trimBottom);

            bool _canTrim(Color _p) => _p.a == 0f;
        }

        private void tryTrimAndSave()
        {
            clampTrimsToValid(ref trimLeft, ref trimRight, ref trimTop, ref trimBottom, originalSize);

            int _newW = originalSize.x - (trimLeft + trimRight);
            int _newH = originalSize.y - (trimTop + trimBottom);

            if (_newW <= 0 || _newH <= 0)
            {
                EditorUtility.DisplayDialog("Trim Texture", "New size must be > 0.", "OK");
                return;
            }

            try
            {
                SpriteUtilities.TrimTextureAssetInPlace(
                    assetPath,
                    trimLeft,
                    trimRight,
                    trimTop,
                    trimBottom);

                editorWindow.Close();
            }
            catch (Exception _ex)
            {
                Debug.LogException(_ex);
                EditorUtility.DisplayDialog("Trim Texture", $"Failed to trim texture:\n{_ex.Message}", "OK");
            }
        }

        private static Vector2Int calculatePixelsToTrimToMultipleOf(Vector2Int _size, int _step)
        {
            int _x = _size.x % _step;
            int _y = _size.y % _step;
            return new Vector2Int(_x, _y);
        }

        private static void clampTrimsToValid(ref int _left, ref int _right, ref int _top, ref int _bottom, Vector2Int _size)
        {
            _left = Mathf.Max(0, _left);
            _right = Mathf.Max(0, _right);
            _top = Mathf.Max(0, _top);
            _bottom = Mathf.Max(0, _bottom);

            int _maxX = Mathf.Max(0, _size.x - 1);
            int _maxY = Mathf.Max(0, _size.y - 1);

            if (_left + _right > _maxX)
            {
                int _over = (_left + _right) - _maxX;
                int _reduceRight = Mathf.Min(_right, _over);
                _right -= _reduceRight;
                _over -= _reduceRight;
                _left = Mathf.Max(0, _left - _over);
            }

            if (_top + _bottom > _maxY)
            {
                int _over = (_top + _bottom) - _maxY;
                int _reduceTop = Mathf.Min(_top, _over);
                _top -= _reduceTop;
                _over -= _reduceTop;
                _bottom = Mathf.Max(0, _bottom - _over);
            }
        }
    }

    public static class SpriteUtilities
    {
        public static void TrimTextureAssetInPlace(string _assetPath, int _trimLeft, int _trimRight, int _trimTop, int _trimBottom)
        {
            if (string.IsNullOrWhiteSpace(_assetPath))
            {
                throw new ArgumentException("Asset path is empty.", nameof(_assetPath));
            }

            var _texture = AssetDatabase.LoadAssetAtPath<Texture2D>(_assetPath);

            if (_texture == null)
            {
                throw new InvalidOperationException($"Could not find Texture2D at path: {_assetPath}");
            }

            var _importer = AssetImporter.GetAtPath(_assetPath) as TextureImporter;

            if (_importer == null)
            {
                throw new InvalidOperationException($"Importer is not a TextureImporter: {_assetPath}");
            }

            using (var _scope = new TextureImportScope(_importer))
            {
                _scope.SetReadable(true);

                Texture2D _reloaded = AssetDatabase.LoadAssetAtPath<Texture2D>(_assetPath);

                if (_reloaded == null)
                {
                    throw new InvalidOperationException("Failed to reload texture after changing importer settings.");
                }

                int _w = _reloaded.width;
                int _h = _reloaded.height;

                ValidateTrimRanges(_trimLeft, _trimRight, _trimTop, _trimBottom, _w, _h);

                int _newW = _w - (_trimLeft + _trimRight);
                int _newH = _h - (_trimTop + _trimBottom);

                int _startX = _trimLeft;
                int _startY = _trimBottom;

                Color[] _pixels = _reloaded.GetPixels(_startX, _startY, _newW, _newH);

                var _newTex = new Texture2D(_newW, _newH, TextureFormat.RGBA32, false);
                _newTex.SetPixels(_pixels);
                _newTex.Apply(updateMipmaps: false, makeNoLongerReadable: false);

                SaveTextureOverAssetFile(_assetPath, _newTex);
            }

            AssetDatabase.ImportAsset(_assetPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }

        private static void ValidateTrimRanges(int _trimLeft, int _trimRight, int _trimTop, int _trimBottom, int _w, int _h)
        {
            if (_trimLeft < 0 || _trimRight < 0 || _trimTop < 0 || _trimBottom < 0)
            {
                throw new ArgumentOutOfRangeException("Trim values cannot be negative.");
            }

            int _newW = _w - (_trimLeft + _trimRight);
            int _newH = _h - (_trimTop + _trimBottom);

            if (_newW <= 0 || _newH <= 0)
            {
                throw new ArgumentOutOfRangeException("Trim is too large - new size would be <= 0.");
            }
        }

        private static void SaveTextureOverAssetFile(string _assetPath, Texture2D _texture)
        {
            string _fullPath = Path.GetFullPath(_assetPath);

            if (!File.Exists(_fullPath))
            {
                throw new FileNotFoundException("Asset file not found on disk.", _fullPath);
            }

            string _ext = Path.GetExtension(_assetPath).ToLowerInvariant();
            byte[] _bytes;

            if (_ext == ".png")
            {
                _bytes = _texture.EncodeToPNG();
            }
            else if (_ext == ".jpg" || _ext == ".jpeg")
            {
                _bytes = _texture.EncodeToJPG(quality: 95);
            }
            else
            {
                throw new NotSupportedException($"Unsupported texture format: {_ext}. Supported: .png, .jpg.");
            }

            File.WriteAllBytes(_fullPath, _bytes);
        }

        private sealed class TextureImportScope : IDisposable
        {
            private readonly TextureImporter importer;
            private readonly bool prevIsReadable;

            public TextureImportScope(TextureImporter _importer)
            {
                importer = _importer ?? throw new ArgumentNullException(nameof(_importer));
                prevIsReadable = importer.isReadable;
            }

            public void SetReadable(bool _readable)
            {
                if (importer.isReadable == _readable)
                {
                    return;
                }

                importer.isReadable = _readable;
                importer.SaveAndReimport();
            }

            public void Dispose()
            {
                if (importer == null)
                {
                    return;
                }

                if (importer.isReadable != prevIsReadable)
                {
                    importer.isReadable = prevIsReadable;
                    importer.SaveAndReimport();
                }
            }
        }
    }

    internal static class TrimTextureContextMenu
    {
        [MenuItem("Assets/Trim Texture", true)]
        private static bool ValidateTrimTexture()
        {
            return Selection.activeObject is Texture2D;
        }

        [MenuItem("Assets/Trim Texture", false, 2050)]
        private static void TrimTexture()
        {
            var _tex = Selection.activeObject as Texture2D;
            Vector2 _pos = GUIUtility.GUIToScreenPoint(Event.current != null ? Event.current.mousePosition : new Vector2(200, 200));
            SpriteEditionPopup.Open(_tex, _pos);
        }
    }
}