namespace DL.TextureSaver
{
    using UnityEditor;
    using UnityEngine;

    public class TextureSaverSettingsPopup : PopupWindowContent
    {
        private GameObject objectWithImage;

        public static void Show(GameObject _objectWithImage, Vector2 _position, string _filename = "t_New")
        {
            TextureSaverEditor.Filename = _filename;

            TextureSaverSettingsPopup _popup = new TextureSaverSettingsPopup
            {
                objectWithImage = _objectWithImage,
            };

            PopupWindow.Show(new Rect(_position, Vector2.zero), _popup);
        }

        public override void OnGUI(Rect _rect)
        {
            bool _clicked = TextureSaverEditor.DrawTextureSaverSettings(getObjectWithImage);

            if (_clicked)
            {
                editorWindow.Close();
            }
        }

        public override Vector2 GetWindowSize()
        {
            //Adjust the size to fit the content
            int _lineCount = 6;
            int _lineHeight = 20;
            int _width = 500;

            return new Vector2(_width, _lineCount * _lineHeight);
        }

        private GameObject getObjectWithImage() => objectWithImage;
    }
}
