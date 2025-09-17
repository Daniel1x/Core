namespace DL
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public static class ProjectSetup
    {
        public const string DEFAULT_ROOT_FOLDER = "Project";

        public static readonly string[] DefaultFolders = new string[]
        {
            "Code",
            "Inputs",
            "Art",
            "Animations",
            "Audio",
            "Shaders",
            "Materials",
            "Models",
            "Prefabs",
            "Scenes",
            "Textures",
        };

        [MenuItem(CoreData.ROOT + "Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            CreateDefaultFolders(DEFAULT_ROOT_FOLDER, DefaultFolders);
            AssetDatabase.Refresh();
        }

        public static void CreateDefaultFolders(string _root, params string[] _folders)
        {
            string _fullPath = Path.Combine(Application.dataPath, _root);

            foreach (var _folder in _folders)
            {
                string _folderPath = Path.Combine(_fullPath, _folder);

                if (!Directory.Exists(_folderPath))
                {
                    Directory.CreateDirectory(_folderPath);
                    Debug.Log($"Created folder: {_folderPath}");
                }
            }
        }
    }
}
