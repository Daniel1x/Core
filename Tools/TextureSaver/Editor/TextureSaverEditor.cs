namespace DL.TextureSaver
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(TextureSaver))]
    public class TextureSaverEditor : Editor
    {
        public static string DirectoryPath = "Assets";
        public static string Filename = "SavedTexture";
        public static TextureFormat Format = TextureFormat.RGBA32;
        public static bool OptimizeFor9Slice = true;

        private GameObject getObjectFromTarget() => (target as TextureSaver)?.gameObject;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(10);
            DrawTextureSaverSettings(getObjectFromTarget);
        }

        public static bool DrawTextureSaverSettings(System.Func<GameObject> _getObject)
        {
            GUILayout.Label("Texture Saving Settings", EditorStyles.boldLabel);

            using (new GUILayout.HorizontalScope())
            {
                DirectoryPath = EditorGUILayout.TextField("Directory Path", DirectoryPath);

                if (GUILayout.Button("...", GUILayout.MaxWidth(30)))
                {
                    string _selectedPath = EditorUtility.OpenFolderPanel("Select Directory", Application.dataPath, "");

                    if (!string.IsNullOrEmpty(_selectedPath))
                    {
                        if (_selectedPath.StartsWith(Application.dataPath))
                        {
                            DirectoryPath = "Assets" + _selectedPath.Substring(Application.dataPath.Length);
                        }
                        else
                        {
                            Debug.LogError("Please select a folder within the Assets directory.");
                        }
                    }
                }
            }

            Filename = EditorGUILayout.TextField("Filename", Filename);
            Format = (TextureFormat)EditorGUILayout.EnumPopup("Texture Format", Format);
            OptimizeFor9Slice = EditorGUILayout.Toggle("Optimize for 9-Slice", OptimizeFor9Slice);

            if (!GUILayout.Button("Save Texture"))
            {
                return false;
            }

            if (!System.IO.Directory.Exists(DirectoryPath))
            {
                System.IO.Directory.CreateDirectory(DirectoryPath);
                AssetDatabase.Refresh();
            }

            if (_getObject != null && _getObject() is GameObject _target)
            {
                TextureSaver.SaveTexture(_target, DirectoryPath, Filename, Format, OptimizeFor9Slice);
            }

            return true;
        }
    }
}
