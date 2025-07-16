using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class EditorUtilities
{
    public const string DOT_PREFAB = ".prefab";
    public const string DOT_ASSET = ".asset";

    public static void DestroyObject(GameObject _cacheObj)
    {
        if (Application.isPlaying)
        {
            GameObject.Destroy(_cacheObj);
        }
        else
        {
            GameObject.DestroyImmediate(_cacheObj);
        }
    }

    public static GameObject GetGameObjectPrefab(string _prefabNameWithoutDotPrefab)
    {
#if UNITY_EDITOR
        string[] _guids = AssetDatabase.FindAssets("t:" + typeof(GameObject).Name);
        int _count = _guids.Length;

        if (_count <= 0)
        {
            return null;
        }

        string _fileName = _prefabNameWithoutDotPrefab + DOT_PREFAB;
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
            return null;
        }

        return AssetDatabase.LoadAssetAtPath<GameObject>(_path);
#else
        return null;
#endif
    }

    public static T GetPrefabComponent<T>(string _prefabNameWithoutDotPrefab, out GameObject _prefab) where T : Component
    {
#if UNITY_EDITOR
        _prefab = GetGameObjectPrefab(_prefabNameWithoutDotPrefab);
        return _prefab != null ? _prefab.GetComponent<T>() : null;
#else
        _prefab = null;
        return null;
#endif
    }

    public static void SetDirty(this Object _target)
    {
#if UNITY_EDITOR
        if (_target == null)
        {
            MyLog.Warning("EDITOR UTILITY :: Object that you tried to set dirty is null!");
            return;
        }

        UnityEditor.EditorUtility.SetDirty(_target);
#endif
    }

    public static void SaveAssetDatabase()
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    public static void SaveAssetIfDirty(this UnityEngine.Object _object)
    {
#if UNITY_EDITOR
        if (_object != null)
        {
            UnityEditor.AssetDatabase.SaveAssetIfDirty(_object);
        }
#endif
    }

    public static void SaveAsset(Object _object)
    {
#if UNITY_EDITOR

        if (_object == null)
        {
            return;
        }

        UnityEditor.AssetDatabase.SaveAssetIfDirty(_object);
#endif
    }

    public static T InstantiatePrefab<T>(T _object) where T : Object
    {
#if UNITY_EDITOR
        return PrefabUtility.InstantiatePrefab(_object) as T;
#else
        return null;
#endif
    }

    public static Object InstantiatePrefab(Object _object) => InstantiatePrefab<Object>(_object);

    public static T InstantiatePrefab<T>(T _object, Transform _parent) where T : Object
    {
#if UNITY_EDITOR
        return PrefabUtility.InstantiatePrefab(_object, _parent) as T;
#else
        return null;
#endif
    }

    public static Object InstantiatePrefab(Object _object, Transform _parent) => InstantiatePrefab<Object>(_object, _parent);

    public static void Ping(this Object _object)
    {
#if UNITY_EDITOR
        EditorGUIUtility.PingObject(_object);
#endif
    }

    public static void PingPath(string _path)
    {
#if UNITY_EDITOR
        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(_path.TrimEnd('/'), typeof(Object)));
#endif
    }

    public static void ClearDirty(this Object _target)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearDirty(_target);
#endif
    }

    public static bool IsDirty(this Object _target)
    {
#if UNITY_EDITOR
        return UnityEditor.EditorUtility.IsDirty(_target);
#else
        return false;
#endif
    }

    public static bool IsDirty(int _instanceID)
    {
#if UNITY_EDITOR
        return UnityEditor.EditorUtility.IsDirty(_instanceID);
#else
        return false;
#endif
    }

    public static int GetDirtyCount(Object _target)
    {
#if UNITY_EDITOR
        return UnityEditor.EditorUtility.GetDirtyCount(_target);
#else
        return 0;
#endif
    }

    public static int GetDirtyCount(int _instanceID)
    {
#if UNITY_EDITOR
        return UnityEditor.EditorUtility.GetDirtyCount(_instanceID);
#else
        return 0;
#endif
    }

    public static void RenameAsset(this UnityEngine.Object _assetObject, string _newAssetName)
    {
#if UNITY_EDITOR
        if (_assetObject.name != _newAssetName && AssetDatabase.IsMainAsset(_assetObject))
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_assetObject), _newAssetName);
            SetDirty(_assetObject);
        }
#endif
    }

    ///<summary>This function will rename object only in editor and while application is playing!</summary>
    public static void Rename(this UnityEngine.Object _gameplayObject, string _name)
    {
#if UNITY_EDITOR
        if (Application.isPlaying && _gameplayObject != null && _name.IsNullEmptyOrWhitespace() == false)
        {
            _gameplayObject.name = _name;
        }
#endif
    }

    ///<summary>This function will rename object only in editor and while application is playing!</summary>
    public static void SetNameSuffix(this UnityEngine.Object _gameplayObject, string _suffix, ref string _defaultName)
    {
#if UNITY_EDITOR
        if (Application.isPlaying && _gameplayObject != null)
        {
            if (_defaultName.IsNullEmptyOrWhitespace())
            {
                _defaultName = _gameplayObject.name;
            }

            _gameplayObject.name = _defaultName + _suffix;
        }
#endif
    }

    public static GameObject GetGameObjectPrefab(string _prefabName, bool _equals = true)
    {
        return GetGameObjectPrefab(_prefabName, string.Empty, _equals);
    }

    public static GameObject GetGameObjectPrefab(string _prefabName, string _directoryPath, bool _equals)
    {
#if UNITY_EDITOR
        string[] _guids;

        if (_directoryPath != string.Empty)
        {
            _guids = AssetDatabase.FindAssets("t:" + typeof(GameObject).Name, new string[] { _directoryPath });
        }
        else
        {
            _guids = AssetDatabase.FindAssets("t:" + typeof(GameObject).Name);
        }

        int _count = _guids.Length;

        if (_count <= 0)
        {
            MyLog.Error($"ASSETS UTILITY :: There is no prefab file of type {typeof(GameObject).Name}!");
            return null;
        }

        for (int i = 0; i < _count; i++)
        {
            string _path = AssetDatabase.GUIDToAssetPath(_guids[i]);
            GameObject _loadedGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(_path);

            if (_loadedGameObject != null)
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

#endif
        return null;
    }

    public static GameObject[] GetGameObjectPrefabs(string _filter, string _directoryPath)
    {
#if UNITY_EDITOR
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
            MyLog.Error($"ASSETS UTILITY :: There is no prefab file of type {typeof(GameObject).Name}!");
            return null;
        }

        GameObject[] _prefabs = new GameObject[_guids.Length];

        for (int i = 0; i < _count; i++)
        {
            _prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(_guids[i]));
        }

        return _prefabs;
#else
        return null;
#endif
    }

    public static List<GameObject> GetGameObjectPrefabsWithComponent<T>(string _filter, string _directoryPath, out List<T> _components) where T : Component
    {
#if UNITY_EDITOR
        GameObject[] _prefabs = GetGameObjectPrefabs(_filter, _directoryPath);

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
#else
        _components = null;
        return null;
#endif
    }

    public static T GetScriptableObjectFile<T>(string _fileNameWithoutDotAsset, bool _silentLog = false) where T : ScriptableObject
    {
#if UNITY_EDITOR
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
#else
        return null;
#endif
    }

    public static T[] GetScriptableObjectFiles<T>(params string[] _fileNamesWithoutDotAsset) where T : ScriptableObject
    {
#if UNITY_EDITOR
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

            if (_fileNamesToFind.Contains(_fileName))
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
#else
        return null;
#endif
    }

    public static T[] GetAllScriptableObjectFiles<T>() where T : ScriptableObject
    {
#if UNITY_EDITOR
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
#else
        return null;
#endif
    }

    public static T GetFileWithName<T>(string _filter = "", bool _exact = false) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        string _objectType = $"{typeof(T)}";

        if (_objectType.StartsWith("UnityEngine."))
        {
            _objectType = _objectType.TrimStart("UnityEngine.");
        }

        string _searchFilter = $"t:{_objectType} {_filter}";
        string[] _guids = AssetDatabase.FindAssets(_searchFilter);

        if (_guids.Length > 0)
        {
            if (_exact)
            {
                T _asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(_guids[0]));

                if (_asset.name == _filter)
                {
                    return _asset;
                }
            }

            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(_guids[0]));
        }

        return null;
#else
        return null;
#endif
    }

    public static T[] GetAllObjectFiles<T>(string _filter = "") where T : UnityEngine.Object
    {
#if UNITY_EDITOR
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
#else
        return null;
#endif
    }

    public static void PerformActionOnEveryScriptableObjectFile<T>(UnityAction<T> _action, bool _setDirty = true, bool _saveAssetDatabase = true) where T : ScriptableObject
    {
#if UNITY_EDITOR
        foreach (T _obj in GetAllScriptableObjectFiles<T>())
        {
            if (_obj != null)
            {
                _action(_obj);

                if (_setDirty)
                {
                    SetDirty(_obj);
                }
            }
        }

        if (_saveAssetDatabase)
        {
            SaveAssetDatabase();
        }
#endif
    }

    public static T FindObjectOfType<T>(bool _onlyAtRuntime = true, FindObjectsInactive _includeInactive = FindObjectsInactive.Exclude) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (_onlyAtRuntime && Application.isPlaying == false)
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
#else
        return null;
#endif
    }

    public static T[] FindObjectsOfType<T>(bool _onlyAtRuntime = true, FindObjectsInactive _includeInactive = FindObjectsInactive.Exclude, FindObjectsSortMode _sortMode = FindObjectsSortMode.None) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (_onlyAtRuntime && Application.isPlaying == false)
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
#else
        return null;
#endif
    }

    public static T FindScriptableOfType<T>() where T : ScriptableObject
    {
#if UNITY_EDITOR
        string[] _guids = AssetDatabase.FindAssets("t:" + typeof(T));

        if (_guids.Length > 0)
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(_guids[0]));
        }
#endif

        return null;
    }

    public static T[] FindScriptablesOfType<T>() where T : ScriptableObject
    {
#if UNITY_EDITOR
        string[] _guids = AssetDatabase.FindAssets("t:" + typeof(T));
        List<T> _foundObjects = new List<T>();

        if (_guids.Length > 0)
        {
            for (int i = 0; i < _guids.Length; i++)
            {
                _foundObjects.Add(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(_guids[i])));
            }
        }

        return _foundObjects.ToArray();
#else
        return null;
#endif
    }

    public static T FindScriptableOfType<T>(string _filter) where T : ScriptableObject
    {
#if UNITY_EDITOR
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

#endif
        return null;
    }

    public static string GetAssetGUID<T>(this T _asset) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (_asset != null)
        {
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_asset));
        }
#endif
        return "";
    }

    public static string GetAssetPathInEditor<T>(this T _asset) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (_asset != null)
        {
            return AssetDatabase.GetAssetPath(_asset);
        }
#endif
        return "";
    }

    public static void SelectInEditor<T>(this T _selectedObject) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        new UnityEngine.Object[] { _selectedObject }.SelectInEditor();
#endif
    }

    public static void AddToSelectionInEditor<T>(this T _selectedObject) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        Selection.objects = Selection.objects.Extend(1, _selectedObject);
#endif
    }

    public static void SelectInEditor<T>(this T[] _selectedObjects) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        Selection.objects = _selectedObjects;
#endif
    }

    public static void SelectInEditor<T>(this List<T> _selectedObjects) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        Selection.objects = _selectedObjects.ToArray();
#endif
    }

    public static void AddToSelectionInEditor<T>(this T[] _selectedObjects) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        Selection.objects = Selection.objects.AddToArray(_selectedObjects);
#endif
    }

    public static void AddToSelectionInEditor<T>(this List<T> _selectedObjects) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        Selection.objects = Selection.objects.AddToArray(_selectedObjects.ToArray());
#endif
    }

    public static T GetSelectedInEditorObject<T>() where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        foreach (var _obj in Selection.objects)
        {
            if (_obj is T _objectOfType)
            {
                return _objectOfType;
            }
        }
#endif

        return null;
    }

    public static List<T> GetSelectedInEditorObjects<T>() where T : UnityEngine.Object
    {
        List<T> _list = new List<T>();

#if UNITY_EDITOR
        foreach (var _obj in Selection.objects)
        {
            if (_obj is T _objectOfType)
            {
                _list.Add(_objectOfType);
            }
        }
#endif

        return _list;
    }
}
