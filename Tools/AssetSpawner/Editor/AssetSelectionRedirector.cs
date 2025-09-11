namespace DL.AssetLoading
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    internal static class AssetSelectionRedirector
    {
        private static bool suppress;
        private static bool pending;

        static AssetSelectionRedirector()
        {
            Selection.selectionChanged -= onSelectionChanged;
            Selection.selectionChanged += onSelectionChanged;
        }

        private static void onSelectionChanged()
        {
            if (suppress || AssetSelectionBase.ActiveInstancesCount == 0)
            {
                return;
            }

            Object[] _current = Selection.objects;

            if (_current == null || _current.Length == 0)
            {
                return;
            }

            bool _changed = false;

            for (int i = 0; i < _current.Length; i++)
            {
                if (_current[i] is GameObject _go
                    && _go.TryGetComponent(out AssetSelectionBase _selection)
                    && _selection.Redirection != null
                    && _selection.Redirection != _current[i])
                {
                    _current[i] = _selection.Redirection;
                    _changed = true;
                }
            }

            if (!_changed || pending)
            {
                return; // No changes or already pending
            }

            pending = true;

            EditorApplication.delayCall += () =>
            {
                pending = false;
                suppress = true;
                Selection.objects = _current;
                suppress = false;
            };
        }
    }
#endif
}
