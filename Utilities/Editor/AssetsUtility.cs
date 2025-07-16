namespace DL.Editor
{
    using DL.Enum;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

    public static class AssetsUtility
    {
        public const string DOT_ASSET = ".asset";
        public const string ASSETS_STRING = "Assets";

        public delegate bool ObjectValidator<EnumType, DataObject>(EnumType _enum, DataObject _object) where DataObject : ScriptableObject where EnumType : System.Enum;
        public delegate bool ObjectNameValidator<EnumType, DataObject>(EnumNameCreator<EnumType> _nameCreator, DataObject _object) where DataObject : ScriptableObject where EnumType : System.Enum;

        public delegate string EnumNameCreator<EnumType>(EnumType _type) where EnumType : System.Enum;
        public delegate string NameCreator();
        public delegate string NameCreator<Type>(Type _type);
        public delegate string NameCreator<Type, ID>(Type _type, ID _id);

        public static string GetAssetPath(this UnityEngine.Object _object)
        {
            return AssetDatabase.GetAssetPath(_object);
        }

        public static string GetFolderPath(this UnityEngine.Object _object)
        {
            string _assetPath = AssetDatabase.GetAssetPath(_object);
            string _assetName = Path.GetFileName(_assetPath);

            int _length = _assetPath.Length - (_assetName.Length + 1);
            return _assetPath.Substring(0, _length);
        }

        public static string GetFileExtension(this UnityEngine.Object _object)
        {
            string _assetPath = AssetDatabase.GetAssetPath(_object);
            return Path.GetExtension(_assetPath);
        }

        public static string ConvertAbsolutePathToDataPath(string _path, string _fileName)
        {
            return $"{ConvertAbsolutePathToDataPath(_path)}/{_fileName}";
        }

        public static string ConvertAbsolutePathToDataPath(string _path)
        {
            return TrimPathUntilItStartsWithAssetDirectory(_path);
        }

        public static string TrimPathUntilItStartsWithAssetDirectory(string _path)
        {
            while (_path.Length > 0 && _path.StartsWith("Assets/") == false)
            {
                _path = _path.Remove(0, 1);
            }

            return _path;
        }

        public static void PingObject(UnityEngine.Object _objectToPing, bool _withSelection = true)
        {
            EditorGUIUtility.PingObject(_objectToPing);

            if (_withSelection)
            {
                Selection.activeObject = _objectToPing;
            }
        }

        public static void PingFile(string _path, bool _withSelection = true)
        {
            PingObject(AssetDatabase.LoadAssetAtPath(_path.TrimEnd('/'), typeof(UnityEngine.Object)), _withSelection);
        }

        public static bool CreateDirectoryIfNotExists(string _directoryPath)
        {
            if (_directoryPath.StartsWith("/") == false)
            {
                _directoryPath = $"/{_directoryPath}";
            }

            if (_directoryPath.StartsWith("/Assets"))
            {
                _directoryPath = _directoryPath.TrimStart("/Assets");
            }

            string _totalPath = Application.dataPath + _directoryPath;

            if (Directory.Exists(_totalPath) == false)
            {
                Directory.CreateDirectory(_totalPath);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Function that tries to get scriptable object file, or creates new file if there is no existing one.
        /// </summary>
        /// <typeparam name="T">Scriptable object type.</typeparam>
        /// <param name="_withSelection">Selects file as active in inspector window.</param>
        /// <param name="_directoryPath">Path to directory that should contains file.</param>
        /// <param name="_fullPathToFile">Full path to file, with name of the file.</param>
        /// <returns></returns>
        public static T GetFirstScriptableObjectFile<T>(bool _withSelection, string _directoryPath, string _fullPathToFile) where T : ScriptableObject
        {
            if (Directory.Exists(Application.dataPath + _directoryPath) == false)
            {
                Directory.CreateDirectory(Application.dataPath + _directoryPath);
            }

            T _file;

            string[] _guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

            if (_guids.Length > 0)
            {
                _file = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(_guids[0]));
            }
            else
            {
                _file = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(_file, _fullPathToFile);
                AssetDatabase.SaveAssets();
            }

            if (_withSelection)
            {
                Selection.activeObject = _file;
            }

            return _file;
        }

        public static bool EnsureDirectoryExist(string _path, out string _pathToDirectory, out string _directoryName)
        {
            _pathToDirectory = "";
            _directoryName = "";

            if (_path == null || _path == "")
            {
                return false;
            }

            List<string> _directories = _path.Split('/').ToList();

            for (int i = _directories.Count - 1; i >= 0; i--)
            {
                if (_directories[i].IsNullOrWhitespace())
                {
                    _directories.RemoveAt(i);
                }
            }

            int _dirCount = _directories.Count;

            if (_dirCount < 2 || _directories[0].Contains("Assets") == false)
            {
                return false;
            }

            string _fullPath = _directories[0] + "/";

            for (int i = 1; i < _dirCount; i++)
            {
                _pathToDirectory = _fullPath;
                _directoryName = _directories[i];
                _fullPath += "/" + _directoryName;

                if (AssetDatabase.IsValidFolder(_fullPath) == false)
                {
                    AssetDatabase.CreateFolder(_pathToDirectory, _directoryName);
                }
            }

            return true;
        }

        public static T GetOrCreateScriptableObjectFile<T>(string _directoryPath, string _fullPathToFile, bool _withSelection = false) where T : ScriptableObject
        {
            bool _created = false;
            T _object = GetOrCreateScriptableObjectFile<T>(_directoryPath, _fullPathToFile, out _created);

            if (_withSelection)
            {
                EditorGUIUtility.PingObject(_object);
                Selection.activeObject = _object;
            }

            return _object;
        }

        public static T GetOrCreateScriptableObjectFile<T>(string _directoryPath, string _fullPathToFile, out bool _newFileCreated, bool _withSelection = false) where T : ScriptableObject
        {
            T _object = GetOrCreateScriptableObjectFile<T>(_directoryPath, _fullPathToFile, out _newFileCreated);

            if (_withSelection)
            {
                EditorGUIUtility.PingObject(_object);
                Selection.activeObject = _object;
            }

            return _object;
        }

        public static T GetOrCreateScriptableObjectFile<T>(string _directoryPath, string _fullPathToFile, out bool _newFileCreated) where T : ScriptableObject
        {
            if (EnsureDirectoryExist(_directoryPath, out string _, out string _) == false)
            {
                _newFileCreated = false;
                MyLog.Error($"ASSETS UTILITY :: Directory path to file is incorrect!");
                return default;
            }

            T _file;

            string[] _guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            bool _found = false;
            string _path = "";

            for (int i = 0; i < _guids.Length; i++)
            {
                string _tempPath = AssetDatabase.GUIDToAssetPath(_guids[i]);

                if (_fullPathToFile == _tempPath)
                {
                    _found = true;
                    _path = _tempPath;
                    break;
                }
            }

            if (_found == true)
            {
                _file = AssetDatabase.LoadAssetAtPath<T>(_path);
            }
            else
            {
                _file = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(_file, _fullPathToFile);
                AssetDatabase.SaveAssets();
            }

            _newFileCreated = !_found;
            return _file;
        }

        public static T LoadAssetWithName<T>(string _name, bool _equalName = false) where T : UnityEngine.Object
        {
            string[] _guids = AssetDatabase.FindAssets($"t:{typeof(T).Name} {_name}");

            if (_guids.Length > 0)
            {
                UnityEngine.Object[] _objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(_guids[0]));

                foreach (var _object in _objects)
                {
                    if (_object is T == false)
                    {
                        continue;
                    }

                    if (_equalName)
                    {
                        if (_object.name == _name)
                        {
                            return _object as T;
                        }
                    }
                    else
                    {
                        if (_object.name.Contains(_name))
                        {
                            return _object as T;
                        }
                    }
                }
            }

            return null;
        }

        public static GameObject GetGameObjectPrefab(string _prefabName, bool _equals = true)
        {
            return GetGameObjectPrefab(_prefabName, string.Empty, _equals);
        }

        public static GameObject GetGameObjectPrefab(string _prefabName, string _directoryPath, bool _equals, bool _silentLog = false)
        {
            string[] _guids = _directoryPath != string.Empty
                ? AssetDatabase.FindAssets("t:" + typeof(GameObject).Name, new string[] { _directoryPath })
                : AssetDatabase.FindAssets("t:" + typeof(GameObject).Name);

            int _count = _guids.Length;

            if (_count <= 0)
            {
                if (_silentLog == false)
                {
                    MyLog.Error($"ASSETS UTILITY :: There is no prefab file of type {typeof(GameObject).Name}!");
                }

                return null;
            }

            for (int i = 0; i < _count; i++)
            {
                string _path = AssetDatabase.GUIDToAssetPath(_guids[i]);
                GameObject _loadedGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(_path);

                if (_loadedGameObject)
                {
                    if (_equals)
                    {
                        if (_prefabName == _loadedGameObject.name)
                        {
                            return _loadedGameObject;
                        }
                    }
                    else
                    {
                        if (_loadedGameObject.name.Contains(_prefabName))
                        {
                            return _loadedGameObject;
                        }
                    }
                }
            }

            return null;
        }

        public static GameObject[] GetGameObjectPrefabs(string _filter, string _directoryPath, bool _disableLog = false)
        {
            string[] _guids;

            if (_directoryPath != string.Empty)
            {
                _guids = AssetDatabase.FindAssets(_filter + " t:" + typeof(GameObject).Name, new string[] { _directoryPath });
            }
            else
            {
                _guids = AssetDatabase.FindAssets(_filter + " t:" + typeof(GameObject).Name);
            }

            int _count = _guids.Length;

            if (_count <= 0)
            {
                if (_disableLog == false)
                {
                    MyLog.Error($"ASSETS UTILITY :: There is no prefab file of type {typeof(GameObject).Name}!");
                }

                return null;
            }

            GameObject[] _prefabs = new GameObject[_guids.Length];

            for (int i = 0; i < _count; i++)
            {
                _prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(_guids[i]));
            }

            return _prefabs;
        }

        public static List<GameObject> GetGameObjectPrefabsWithComponent<T>(string _filter, string _directoryPath, out List<T> _components, bool _disableLog = false) where T : Component
        {
            GameObject[] _prefabs = GetGameObjectPrefabs(_filter, _directoryPath, _disableLog);

            List<GameObject> _objects = new List<GameObject>();
            _components = new List<T>();

            if (_prefabs == null)
            {
                return _objects;
            }

            for (int i = 0; i < _prefabs.Length; i++)
            {
                T _component = _prefabs[i].GetComponent<T>();

                if (_component != null)
                {
                    _objects.Add(_prefabs[i]);
                    _components.Add(_component);
                }
            }

            return _objects;
        }

        public static T GetPrefabComponent<T>(string _prefabName, out GameObject _prefab) where T : Component
        {
            _prefab = GetGameObjectPrefab(_prefabName);
            return _prefab != null ? _prefab.GetComponent<T>() : null;
        }

        public static T GetScriptableObjectFile<T>(string _fileNameWithoutDotAsset, bool _silentLog = false) where T : ScriptableObject
        {
            string[] _guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            int _count = _guids.Length;

            if (_count <= 0)
            {
                if (_silentLog == false)
                {
                    MyLog.Error($"ASSETS UTILITY :: There is no scriptable object file of type {typeof(T).Name}!");
                }

                return null;
            }

            string _fileName = _fileNameWithoutDotAsset + DOT_ASSET;
            string _path = null;
            bool _found = false;

            for (int i = 0; i < _count; i++)
            {
                _path = AssetDatabase.GUIDToAssetPath(_guids[i]);
                string _name = _path.Split('/', '\\').Last();

                if (_name == _fileName)
                {
                    _found = true;
                    break;
                }
            }

            if (_found == false)
            {
                if (_silentLog == false)
                {
                    MyLog.Error($"ASSETS UTILITY :: There is no scriptable object file of type {typeof(T).Name} named {_fileName}!");
                }

                return null;
            }

            return AssetDatabase.LoadAssetAtPath<T>(_path);
        }

        public static T[] GetScriptableObjectFiles<T>(params string[] _fileNamesWithoutDotAsset) where T : ScriptableObject
        {
            string[] _guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            int _guidsCount = _guids.Length;

            if (_guidsCount <= 0)
            {
                MyLog.Error($"ASSETS UTILITY :: There is no scriptable object file of type {typeof(T).Name}!");
                return new T[] { };
            }

            int _namesCount = _fileNamesWithoutDotAsset.Length;
            List<string> _fileNamesToFind = new List<string>();

            for (int i = 0; i < _namesCount; i++)
            {
                _fileNamesToFind.Add(_fileNamesWithoutDotAsset[i] + DOT_ASSET);
            }

            List<T> _files = new List<T>();

            for (int i = 0; i < _guidsCount; i++)
            {
                string _path = AssetDatabase.GUIDToAssetPath(_guids[i]);
                string _fileName = _path.Split('/', '\\').Last();

                if (_fileNamesToFind.Contains(_fileName) == true)
                {
                    _fileNamesToFind.Remove(_fileName);
                    _files.Add(AssetDatabase.LoadAssetAtPath<T>(_path));

                    if (_fileNamesToFind.Count <= 0)
                    {
                        break;
                    }
                }
            }

            return _files.ToArray();
        }

        public static T[] GetAllScriptableObjectFiles<T>() where T : ScriptableObject
        {
            string[] _guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            int _guidsCount = _guids.Length;

            if (_guidsCount <= 0)
            {
                MyLog.Error($"ASSETS UTILITY :: There is no scriptable object file of type {typeof(T).Name}!");
                return new T[] { };
            }

            List<T> _files = new List<T>();

            for (int i = 0; i < _guidsCount; i++)
            {
                string _path = AssetDatabase.GUIDToAssetPath(_guids[i]);
                _files.Add(AssetDatabase.LoadAssetAtPath<T>(_path));
            }

            return _files.ToArray();
        }

        public static T[] GetAllObjectFiles<T>(string _filter = "") where T : UnityEngine.Object
        {
            string[] _guids = AssetDatabase.FindAssets(_filter + " t:" + typeof(T).Name);
            int _guidsCount = _guids.Length;

            if (_guidsCount <= 0)
            {
                MyLog.Error($"ASSETS UTILITY :: There is no scriptable object file of type {typeof(T).Name}!");
                return new T[] { };
            }

            List<T> _files = new List<T>();

            for (int i = 0; i < _guidsCount; i++)
            {
                string _path = AssetDatabase.GUIDToAssetPath(_guids[i]);
                _files.Add(AssetDatabase.LoadAssetAtPath<T>(_path));
            }

            return _files.ToArray();
        }

        public static string GetAssetGUID(this UnityEngine.Object _asset)
        {
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_asset));
        }

        public static void PerformActionOnEveryScriptableObjectFile<T>(Action<T> _action) where T : ScriptableObject
        {
            foreach (T _obj in GetAllScriptableObjectFiles<T>())
            {
                if (_obj != null)
                {
                    _action(_obj);
                }
            }
        }

        public static T FindObjectOfType<T>(bool _onlyAtRuntime = true, FindObjectsInactive _includeInactive = FindObjectsInactive.Exclude) where T : UnityEngine.Object
        {
            if (_onlyAtRuntime == true && Application.isPlaying == false)
            {
                MyLog.Log("ASSETS UTILITY :: This action can be performed only at runtime!");
                return null;
            }

            T _object = UnityEngine.Object.FindFirstObjectByType<T>(_includeInactive);

            if (_object == null)
            {
                MyLog.Log($"ASSETS UTILITY :: Object of type <{typeof(T).Name}> is not available in current scene!");
            }

            return _object;
        }

        public static T[] FindObjectsOfType<T>(bool _onlyAtRuntime = true, FindObjectsInactive _includeInactive = FindObjectsInactive.Exclude, FindObjectsSortMode _sortMode = FindObjectsSortMode.None) where T : UnityEngine.Object
        {
            if (_onlyAtRuntime == true && Application.isPlaying == false)
            {
                MyLog.Log("ASSETS UTILITY :: This action can be performed only at runtime!");
                return null;
            }

            T[] _objects = UnityEngine.Object.FindObjectsByType<T>(_includeInactive, _sortMode);

            if (_objects == null || _objects.Length < 1)
            {
                MyLog.Log($"ASSETS UTILITY :: Object of type <{typeof(T).Name}> is not available in current scene!");
            }

            return _objects;
        }

        public static T FindScriptableOfType<T>() where T : ScriptableObject
        {
            string[] _guids = AssetDatabase.FindAssets("t:" + typeof(T));

            if (_guids.Length > 0)
            {
                return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(_guids[0]));
            }

            return null;
        }

        public static T[] FindScriptablesOfType<T>(string _filter = "", string _directoryPath = "") where T : ScriptableObject
        {
            string[] _guids = _directoryPath.IsNullEmptyOrWhitespace()
                ? AssetDatabase.FindAssets($"{_filter} t:" + typeof(T))
                : AssetDatabase.FindAssets($"{_filter} t:" + typeof(T), new string[] { _directoryPath });

            List<T> _foundObjects = new List<T>();

            if (_guids.Length > 0)
            {
                for (int i = 0; i < _guids.Length; i++)
                {
                    _foundObjects.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(_guids[i])));
                }
            }

            return _foundObjects.ToArray();
        }

        public static int FindScriptablesOfType<T>(List<T> _resultList, string _filter = "", string _directoryPath = "") where T : ScriptableObject
        {
            string[] _guids = _directoryPath.IsNullEmptyOrWhitespace()
                ? AssetDatabase.FindAssets($"{_filter} t:" + typeof(T))
                : AssetDatabase.FindAssets($"{_filter} t:" + typeof(T), new string[] { _directoryPath });

            if (_guids.Length > 0)
            {
                for (int i = 0; i < _guids.Length; i++)
                {
                    _resultList.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(_guids[i])));
                }
            }

            return _guids.Length;
        }

        public static T FindScriptableOfType<T>(string _filter) where T : ScriptableObject
        {
            string[] _guids = AssetDatabase.FindAssets("t:" + typeof(T));

            if (_guids.Length > 0)
            {
                string _assetPath = string.Empty;

                for (int i = 0; i < _guids.Length; i++)
                {
                    _assetPath = AssetDatabase.GUIDToAssetPath(_guids[i]);

                    if (_assetPath.ToLower().Contains(_filter.ToLower()))
                    {
                        break;
                    }
                }

                return AssetDatabase.LoadAssetAtPath<T>(_assetPath);
            }

            return null;
        }

        /// <param name="_nameCreator">Function that converts enum to name.</param>
        /// <param name="_pathToDirectoryWithoutSlash">Path that starts with Assets/...</param>
        /// <param name="_checkIfObjectExistInOtherDirectory">Checks that the scriptable object with the created name exists in a different directory and if it does, the new scriptable object will not be created.</param>
        /// <param name="_onSingleObjectCreated">Callback when item was created. Params: <bool(new file created), EnumType, CreatedObject></param>
        /// <param name="_onAllObjectsCreated">Action called at the end.</param>
        public static void CreateEnumScriptableObjects<EnumType, ScriptableObjectType>
                (EnumNameCreator<EnumType> _nameCreator = null, string _pathToDirectoryWithoutSlash = "Assets/Data/Scriptables/Generic", bool _checkIfObjectExistInOtherDirectory = false,
                UnityAction<bool, EnumType, ScriptableObjectType> _onSingleObjectCreated = null, UnityAction _onAllObjectsCreated = null)
                where EnumType : System.Enum where ScriptableObjectType : ScriptableObject
        {
            if (_checkIfObjectExistInOtherDirectory)
            {
                ScriptableObjectType[] _objectsFound = GetAllScriptableObjectFiles<ScriptableObjectType>();

                EnumExtensions.PerformActionForEachEnumValue<EnumType>(_type => getOrCreateSingleObject(_type, _nameCreator, _pathToDirectoryWithoutSlash, _checkIfObjectExistInOtherDirectory, _objectsFound, _onSingleObjectCreated));
            }
            else
            {
                EnumExtensions.PerformActionForEachEnumValue<EnumType>(_type => getOrCreateSingleObject(_type, _nameCreator, _pathToDirectoryWithoutSlash, _checkIfObjectExistInOtherDirectory, null, _onSingleObjectCreated));
            }

            _onAllObjectsCreated?.Invoke();
        }

        /// <param name="_nameCreator">Function that converts enum to name.</param>
        /// <param name="_pathToDirectoryWithoutSlash">Path that starts with Assets/...</param>
        /// <param name="_checkIfObjectExistInOtherDirectory">Checks that the scriptable object with the created name exists in a different directory and if it does, the new scriptable object will not be created.</param>
        /// <param name="_onSingleObjectCreated">Callback when item was created. Params: <bool(new file created), EnumType, CreatedObject></param>
        /// <param name="_onAllObjectsCreated">Action called at the end.</param>
        public static void CreateEnumScriptableObjects<EnumType, ScriptableObjectType>
                (EnumType[] _types, EnumNameCreator<EnumType> _nameCreator = null, string _pathToDirectoryWithoutSlash = "Assets/Data/Scriptables/Generic", bool _checkIfObjectExistInOtherDirectory = false,
                UnityAction<bool, EnumType, ScriptableObjectType> _onSingleObjectCreated = null, UnityAction _onAllObjectsCreated = null)
                where EnumType : System.Enum where ScriptableObjectType : ScriptableObject
        {
            if (_types == null)
            {
                return;
            }

            _types = _types.WithoutDuplicates();

            if (_checkIfObjectExistInOtherDirectory)
            {
                ScriptableObjectType[] _objectsFound = GetAllScriptableObjectFiles<ScriptableObjectType>();

                for (int i = 0; i < _types.Length; i++)
                {
                    getOrCreateSingleObject(_types[i], _nameCreator, _pathToDirectoryWithoutSlash, _checkIfObjectExistInOtherDirectory, _objectsFound, _onSingleObjectCreated);
                }
            }
            else
            {
                for (int i = 0; i < _types.Length; i++)
                {
                    getOrCreateSingleObject(_types[i], _nameCreator, _pathToDirectoryWithoutSlash, _checkIfObjectExistInOtherDirectory, null, _onSingleObjectCreated);
                }
            }

            _onAllObjectsCreated?.Invoke();
        }

        private static void getOrCreateSingleObject<EnumType, ScriptableObjectType>(EnumType _type, EnumNameCreator<EnumType> _nameCreator, string _pathToDirectoryWithoutSlash,
            bool _checkIfObjectExistInOtherDirectory, ScriptableObjectType[] _foundObjects, UnityAction<bool, EnumType, ScriptableObjectType> _onSingleObjectCreated)
                where EnumType : System.Enum where ScriptableObjectType : ScriptableObject
        {
            string _objectNameWithoutDotAsset = (_nameCreator == null ? "data_" + _type.ToString() : _nameCreator(_type));
            string _objectName = _objectNameWithoutDotAsset + ".asset";
            string _fullPathToFile = _pathToDirectoryWithoutSlash + "/" + _objectName;
            ScriptableObjectType _object;
            bool _newFileCreated;

            if (_checkIfObjectExistInOtherDirectory && _foundObjects != null && _objectExist(_objectNameWithoutDotAsset, out ScriptableObjectType _foundObject))
            {
                _newFileCreated = false;
                _object = _foundObject;
            }
            else
            {
                _object = GetOrCreateScriptableObjectFile<ScriptableObjectType>(_pathToDirectoryWithoutSlash, _fullPathToFile, out _newFileCreated);
            }

            _onSingleObjectCreated?.Invoke(_newFileCreated, _type, _object);

            bool _objectExist(string _objectNameWithoutDotAsset, out ScriptableObjectType _file)
            {
                for (int i = 0; i < _foundObjects.Length; i++)
                {
                    if (_foundObjects[i].name == _objectNameWithoutDotAsset)
                    {
                        _file = _foundObjects[i];
                        return true;
                    }
                }

                _file = null;
                return false;
            }
        }

        public static bool IsRootOfPrefabInstance(this GameObject _root)
        {
            return _root.IsObjectFromScene() && PrefabUtility.IsAnyPrefabInstanceRoot(_root);
        }

        public static bool IsObjectFromAssets<T>(this T _root) where T : UnityEngine.Object
        {
            return _root != null && EditorUtility.IsPersistent(_root);
        }

        public static bool IsObjectFromScene<T>(this T _root) where T : UnityEngine.Object
        {
            return _root != null && EditorUtility.IsPersistent(_root) == false;
        }

        public static PrefabAssetType GetPrefabAssetType(this GameObject _root)
        {
            return PrefabUtility.GetPrefabAssetType(_root);
        }
    }
}
