namespace DL.TextureSaver
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(TextureSaver))]
    public class TextureSaverEditor : Editor
    {
        private string directoryPath = "Assets/SavedTextures";
        private string filename = "SavedTexture";
        private TextureFormat textureFormat = TextureFormat.RGBA32;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(10);
            GUILayout.Label("Texture Saving Settings", EditorStyles.boldLabel);

            using (new GUILayout.HorizontalScope())
            {
                directoryPath = EditorGUILayout.TextField("Directory Path", directoryPath);

                if (GUILayout.Button("...", GUILayout.MaxWidth(30)))
                {
                    string _selectedPath = EditorUtility.OpenFolderPanel("Select Directory", Application.dataPath, "");

                    if (!string.IsNullOrEmpty(_selectedPath))
                    {
                        if (_selectedPath.StartsWith(Application.dataPath))
                        {
                            directoryPath = "Assets" + _selectedPath.Substring(Application.dataPath.Length);
                        }
                        else
                        {
                            Debug.LogError("Please select a folder within the Assets directory.");
                        }
                    }
                }
            }

            filename = EditorGUILayout.TextField("Filename", filename);
            textureFormat = (TextureFormat)EditorGUILayout.EnumPopup("Texture Format", textureFormat);

            if (GUILayout.Button("Save Texture"))
            {
                if (!System.IO.Directory.Exists(directoryPath))
                {
                    System.IO.Directory.CreateDirectory(directoryPath);
                    AssetDatabase.Refresh();
                }

                if (target is TextureSaver _textureSaver)
                {
                    _textureSaver.SaveTexture(directoryPath, filename, textureFormat);
                }

                AssetDatabase.Refresh();
            }
        }
    }
}
