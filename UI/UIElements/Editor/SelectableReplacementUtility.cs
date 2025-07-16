using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
public static class SelectableReplacementUtility
{
    [MenuItem("CONTEXT/Button/Replace with CustomButton")]
    private static void ReplaceWithCustomButton(MenuCommand command) => ReplaceComponent<Button, CustomButton>(command.context as Button);

    [MenuItem("CONTEXT/TMP_Dropdown/Replace with CustomDropdown")]
    private static void ReplaceWithCustomDropdown(MenuCommand command) => ReplaceComponent<TMPro.TMP_Dropdown, CustomDropdown>(command.context as TMPro.TMP_Dropdown);

    [MenuItem("CONTEXT/Scrollbar/Replace with CustomScrollbar")]
    private static void ReplaceWithCustomScrollbar(MenuCommand command) => ReplaceComponent<Scrollbar, CustomScrollbar>(command.context as Scrollbar);

    [MenuItem("CONTEXT/Toggle/Replace with CustomToggle")]
    private static void ReplaceWithCustomToggle(MenuCommand command) => ReplaceComponent<Toggle, CustomToggle>(command.context as Toggle);

    /// <summary>
    /// Replaces a component of type TBase with TDerived on the same GameObject by swapping the m_Script reference.
    /// TDerived must inherit from TBase. Both must be MonoBehaviour.
    /// </summary>
    public static void ReplaceComponent<TBase, TDerived>(this TBase _baseComponent)
        where TBase : MonoBehaviour
        where TDerived : TBase
    {
        if (_baseComponent == null)
        {
            return;
        }

        GameObject _go = _baseComponent.gameObject;

        // Get the MonoScript for TDerived
        MonoScript _derivedScript = GetMonoScriptOfType<TDerived>();

        if (_derivedScript == null)
        {
            Debug.LogError($"Script for {typeof(TDerived).Name} not found in project.");
            return;
        }

        // Swap m_Script property
        SerializedObject _so = new SerializedObject(_baseComponent);
        _so.Update();
        SerializedProperty _scriptProp = _so.FindProperty("m_Script");

        if (_scriptProp == null)
        {
            Debug.LogError("Could not find m_Script property.");
            return;
        }

        _scriptProp.objectReferenceValue = _derivedScript;
        _so.ApplyModifiedPropertiesWithoutUndo();

        // Force Unity to reserialize and reload the component
        EditorUtility.SetDirty(_go);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(_go.scene);

        if (!Application.isPlaying)
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(_go);
        }

        UnityEditorInternal.ComponentUtility.CopyComponent(_baseComponent);
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(_go);
        UnityEngine.Object.DestroyImmediate(_baseComponent, true);

        // Save assets and scenes to ensure changes are persisted
        AssetDatabase.SaveAssets();
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

        Debug.Log($"{typeof(TBase).Name} replaced with {typeof(TDerived).Name} by changing script reference.", _go);
    }

    /// <summary>
    /// Finds the MonoScript asset for the given type T.
    /// </summary>
    public static MonoScript GetMonoScriptOfType<T>() where T : class
    {
        System.Type _type = typeof(T);
        string[] _guids = AssetDatabase.FindAssets($"{_type.Name} t:MonoScript");

        foreach (string _guid in _guids)
        {
            string _path = AssetDatabase.GUIDToAssetPath(_guid);
            MonoScript _monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(_path);

            if (_monoScript != null && _monoScript.GetClass() == _type)
            {
                return _monoScript;
            }
        }

        return null;
    }
}
#endif