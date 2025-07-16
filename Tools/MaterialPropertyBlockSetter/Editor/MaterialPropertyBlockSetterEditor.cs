namespace DL.MaterialPropertyBlockSetter
{
    using DL.Editor;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(MaterialPropertyBlockSetter))]
    public class MaterialPropertyBlockSetterEditor : Editor
    {
        private const float SPACE_BEFORE_BUTTONS = 16f;

        protected MaterialPropertyBlockSetter blockSetter = null;

        protected SerializedProperty updateModeProperty = null;
        protected SerializedProperty updateIntervalProperty = null;
        protected SerializedProperty renderersListProperty = null;

        protected virtual void OnEnable()
        {
            blockSetter = (MaterialPropertyBlockSetter)target;

            blockSetter.FindRenderers();

            updateModeProperty = serializedObject.FindProperty("updateMode");
            updateIntervalProperty = serializedObject.FindProperty("updateInterval");
            renderersListProperty = serializedObject.FindProperty("Renderers");
        }

        public override void OnInspectorGUI()
        {
            EditorDrawing.DrawScriptProperty(serializedObject);
            EditorGUILayout.LabelField("Settings:", EditorStyles.boldLabel);

            using (var _check = new ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(updateModeProperty);

                if ((updateModeProperty.enumValueFlag & (int)BlockUpdateMode.OnUpdate) != 0)
                {
                    EditorGUILayout.PropertyField(updateIntervalProperty);
                }

                if (_check)
                {
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();

                    EditorUtility.SetDirty(target);
                }
            }

            EditorGUILayout.Space();

            drawRenderers();
            drawButtons();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnDisable()
        {
            if (blockSetter == null)
            {
                blockSetter = (MaterialPropertyBlockSetter)target;

                if (blockSetter == null)
                {
                    return;
                }
            }

            blockSetter.MaterialThatDisplaysEditorTools = null;

            bool _anyFieldModified = blockSetter.RemoveNullReferences();

            for (int i = 0; i < blockSetter.Renderers.Count; i++)
            {
                for (int j = 0; j < blockSetter.Renderers[i].Materials.Count; j++)
                {
                    blockSetter.Renderers[i].Materials[j].EditMode = false;
                }
            }

            if (_anyFieldModified)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(blockSetter);
            }
        }

        private void drawRenderers()
        {
            blockSetter.DisplayRenderers = EditorDrawing.FoldoutWithBackground(blockSetter.DisplayRenderers, "Renderers:");

            if (blockSetter.DisplayRenderers == false)
            {
                return;
            }

            using (new IndentScope())
            {
                int _renderersCount = renderersListProperty.arraySize;

                if (_renderersCount <= 0)
                {
                    using (new GUIColorScope(Color.yellow))
                    {
                        EditorGUILayout.LabelField("No renderers available!", EditorStyles.helpBox);
                    }

                    return;
                }

                EditorGUILayout.Space();

                for (int i = 0; i < _renderersCount; i++)
                {
                    if (blockSetter.Renderers.IsIndexOutOfRange(i) || blockSetter.Renderers[i] == null || blockSetter.Renderers[i].Renderer == null)
                    {
                        continue;
                    }

                    RendererDataDrawer.Draw(renderersListProperty.GetArrayElementAtIndex(i), blockSetter.Renderers[i], i, blockSetter);

                    if (i + 1 < _renderersCount)
                    {
                        EditorGUILayout.Space();
                    }
                }
            }
        }

        private void drawButtons()
        {
            EditorGUILayout.Space(SPACE_BEFORE_BUTTONS);

            using (new HorizontalScope())
            {
                if (GUILayout.Button("Find"))
                {
                    blockSetter.FindRenderers();
                    EditorUtility.SetDirty(blockSetter);
                }

                if (GUILayout.Button("Clear"))
                {
                    blockSetter.RemoveAllRenderers();
                    EditorUtility.SetDirty(blockSetter);
                }

                if (GUILayout.Button("Remove Null"))
                {
                    blockSetter.RemoveNullReferences();
                    EditorUtility.SetDirty(blockSetter);
                }

                if (GUILayout.Button("Apply"))
                {
                    blockSetter.UpdateMaterials();
                    EditorUtility.SetDirty(blockSetter);
                }

                if (GUILayout.Button("Apply New"))
                {
                    blockSetter.UpdateMaterials(true);
                    EditorUtility.SetDirty(blockSetter);
                }
            }
        }
    }
}
