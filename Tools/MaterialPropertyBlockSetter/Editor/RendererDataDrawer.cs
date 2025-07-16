namespace DL.MaterialPropertyBlockSetter
{
    using DL.Editor;
    using UnityEditor;
    using UnityEngine;

    public static class RendererDataDrawer
    {
        public static void Draw(SerializedProperty _rendererProperty, RendererData _rendererData, int _id, MaterialPropertyBlockSetter _rendererOwner)
        {
            _rendererData.VisibleInEditor = EditorDrawing.FoldoutWithBackground(_rendererData.VisibleInEditor, $"Renderer {_id}: {_rendererData.Renderer.name}", Color.gray);

            if (_rendererData.VisibleInEditor == false)
            {
                return;
            }

            SerializedProperty _renderer = _rendererProperty.FindPropertyRelative("renderer");
            SerializedProperty _materials = _rendererProperty.FindPropertyRelative("materials");

            using (new IndentScope())
            {
                using (new DisabledScope())
                {
                    EditorGUILayout.LabelField($"Renderer Type: {_rendererData.Renderer.GetType()}");
                }

                if (_materials.arraySize > 0)
                {
                    for (int i = 0; i < _materials.arraySize; i++)
                    {
                        MaterialDataDrawer.Draw(_materials.GetArrayElementAtIndex(i), _rendererData.Materials[i], _rendererData, _rendererOwner);
                    }
                }
                else
                {
                    using (new GUIColorScope(Color.yellow))
                    {
                        EditorGUILayout.LabelField("There is no material in this renderer!", EditorStyles.helpBox);
                    }
                }
            }
        }
    }
}
