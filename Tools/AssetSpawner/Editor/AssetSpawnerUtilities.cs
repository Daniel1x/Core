namespace DL.AssetLoading
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public static class AssetSpawnerUtilities
    {
        private const string MENU_PATH = "GameObject/Replace Prefab With Asset Spawner";

        [MenuItem(MENU_PATH, priority = 49)]
        private static void replaceSelectedPrefabs()
        {
            GameObject[] _selection = Selection.gameObjects;

            if (_selection == null || _selection.Length == 0)
            {
                Debug.LogWarning("No GameObjects selected.");
                return;
            }

            Undo.IncrementCurrentGroup();
            int _undoGroup = Undo.GetCurrentGroup();
            GameObject _lastCreated = null;

            for (int i = 0; i < _selection.Length; i++)
            {
                GameObject _go = _selection[i];

                if (_go == null)
                {
                    continue;
                }

                GameObject _prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(_go);

                if (_prefabAsset == null)
                {
                    Debug.LogWarning($"Skipping '{_go.name}' - not a prefab instance.");
                    continue;
                }

                string _assetPath = AssetDatabase.GetAssetPath(_prefabAsset);
                string _guid = AssetDatabase.AssetPathToGUID(_assetPath);

                if (string.IsNullOrEmpty(_guid))
                {
                    Debug.LogWarning($"Could not resolve GUID for prefab '{_prefabAsset.name}'. Skipping.");
                    continue;
                }

                Transform _parent = _go.transform.parent;
                int _siblingIndex = _go.transform.GetSiblingIndex();
                Vector3 _localPos = _go.transform.localPosition;
                Quaternion _localRot = _go.transform.localRotation;
                Vector3 _localScale = _go.transform.localScale;
                bool _wasActive = _go.activeSelf;

                GameObject _placeholder = new GameObject(_go.name);
                Undo.RegisterCreatedObjectUndo(_placeholder, "Create AssetInstance Placeholder");

                _placeholder.transform.SetParent(_parent, false);
                _placeholder.transform.SetSiblingIndex(_siblingIndex);
                _placeholder.transform.localPosition = _localPos;
                _placeholder.transform.localRotation = _localRot;
                _placeholder.transform.localScale = _localScale;
                _placeholder.SetActive(_wasActive);

                _placeholder.GetOrAddComponent<AssetSpawner>(out _).AssetReference = new(_guid);

                Undo.DestroyObjectImmediate(_go);

                _lastCreated = _placeholder;
            }

            if (_lastCreated != null)
            {
                Selection.activeGameObject = _lastCreated;
            }

            Undo.CollapseUndoOperations(_undoGroup);
        }

        [MenuItem(MENU_PATH, validate = true)]
        private static bool validateReplaceSelectedPrefabs()
        {
            return Selection.gameObjects != null
                && Selection.gameObjects.Length != 0
                && Selection.gameObjects.Any(_go => PrefabUtility.GetCorrespondingObjectFromSource(_go) != null);
        }
    }
}
