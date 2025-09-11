using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class EditorComponentExtensions
{
    private const string SORTING_MENU_PATH = "GameObject/Sort Objects By Name";

    [MenuItem(SORTING_MENU_PATH, priority = 50)]
    private static void sortSelectedObjectsByName()
    {
        GameObject[] _selectedObjects = Selection.gameObjects;

        if (_selectedObjects == null || _selectedObjects.Length <= 1)
        {
            return;
        }

        GameObject[] _validObjects = Array.FindAll(_selectedObjects, go => go.scene.IsValid());

        if (_validObjects.Length <= 1)
        {
            return;
        }

        Dictionary<Transform, List<GameObject>> _groupedByParent = new();

        foreach (GameObject _go in _validObjects)
        {
            Transform _parent = _go.transform.parent;

            if (!_groupedByParent.ContainsKey(_parent))
            {
                _groupedByParent[_parent] = new List<GameObject>();
            }

            _groupedByParent[_parent].Add(_go);
        }

        Undo.IncrementCurrentGroup();
        int _undoGroup = Undo.GetCurrentGroup();

        foreach (var _data in _groupedByParent)
        {
            List<GameObject> _list = _data.Value;
            int _count = _list.Count;

            if (_count <= 1)
            {
                continue; // Nothing to sort
            }

            List<int> _availableSiblingIndexes = _list.Select(_obj => _obj.transform.GetSiblingIndex()).ToList();
            _availableSiblingIndexes.Sort();
            _list.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));

            for (int i = 0; i < _count; i++)
            {
                GameObject _go = _list[i];
                int _newIndex = _availableSiblingIndexes[i];

                if (_go.transform.GetSiblingIndex() != _newIndex)
                {
                    Undo.SetTransformParent(_go.transform, _data.Key, "Sort Objects By Name");
                    Undo.RecordObject(_go.transform, "Sort Objects By Name");
                    _go.transform.SetSiblingIndex(_newIndex);
                    EditorUtility.SetDirty(_go);
                }
            }
        }

        Undo.CollapseUndoOperations(_undoGroup);
    }

    [MenuItem(SORTING_MENU_PATH, validate = true)]
    private static bool validateSortSelectedObjectsByName()
    {
        return Selection.gameObjects != null
            && Selection.gameObjects.Length > 1
            && Array.Exists(Selection.gameObjects, _go => _go.scene.IsValid());
    }
}
