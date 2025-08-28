namespace DL.TextureSaver
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public static class TextureSaverUtilities
    {
        [MenuItem("CONTEXT/Image/Save Image Texture")]
        private static void saveImageTextureToPNG(MenuCommand _command)
        {
            if (_command.context is Image _img)
            {
                TextureSaver.SaveTexture(_img.gameObject, _filename: "t_" + _img.gameObject.name.RemoveSpaces());
            }
        }

        [MenuItem("CONTEXT/Image/Save Image Texture (With Settings)")]
        private static void saveImageTextureToPNGWithSettingsWindow(MenuCommand _command)
        {
            if (_command.context is Image _img)
            {
                Vector2 _popupPosition = Event.current != null
                    ? Event.current.mousePosition
                    : new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

                TextureSaverSettingsPopup.Show(_img.gameObject, _popupPosition, _filename: "t_" + _img.gameObject.name.RemoveSpaces());
            }
        }
    }
}
