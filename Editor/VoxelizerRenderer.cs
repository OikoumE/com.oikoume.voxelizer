using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Editor
{
    public static class VoxelizerSettingsEditor
    {
        private static VoxelizerGizmosSettings _settings;
        private static bool _showGizmo;
        private static bool _isExpanded;
        private static Voxelizer _voxelizer;

        public static VoxelizerGizmosSettings GetSettings()
        {
            return _settings;
        }

        public static void Render()
        {
            EditorGUILayout.Space();
            // EditorGUI.BeginChangeCheck();
            //
            //   GUILayout.BeginVertical(EditorStyles.helpBox);
            //   _isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(_isExpanded,
            //       "Voxelizer Settings: " );
            // if (!_isExpanded)
            // {
            //     EndHelperBox();
            //     return;
            // }  
            //
            // _showGizmo = VoxelizerEditorHelper.DrawToggle("Show Gizmos", _showGizmo);
            //
            // var spaceWas = _settings.NodeSpace;
            // var spaceIs = VoxelizerEditorHelper.DrawFloatSlider("", spaceWas, 0, 10, 100);
            //
            // if (!Mathf.Approximately(spaceIs, spaceWas))
            // {
            //     _settings.NodeSpace  = spaceIs;
            // }
            //
            // //DO STUFF
            // if (EditorGUI.EndChangeCheck() )
            // {
            //     Voxelizer.GizmosSettings = _settings;
            //     Undo.RecordObject(_settings, $"Modify {field.Name}");
            //     EditorUtility.SetDirty(_settings); // Mark as dirty to save changes
            // }


            // EndHelperBox();
            ScriptableObjectTypeRenderer.RenderFields(typeof(VoxelizerGizmosSettings));
            return;

            void EndHelperBox()
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }
        }
    }

    public static class VoxelizerRenderer
    {
        public static Transform voxelizerParent;
        public static GameObject[] gameObjects;
        private static bool _isExpanded;
        internal static Voxelizer voxelizer;
        private static Vector2 _listScrollPosition = Vector2.zero;


        public static void Render()
        {
            GUILayout.Label("Voxelizer", EditorStyles.boldLabel);
            DrawGridData();
            // FindObjectsByType<Voxelizer>();
            // Shows window content depending on state
            DrawCreateVoxelizerButtons();
            EditorGUILayout.Space();
            DrawList();
        }

        private static void DrawGridData()
        {
            GUILayout.Space(10);
            ScriptableObjectTypeRenderer.RenderScriptableObjectFields(typeof(GridData));
        }

        private static void DrawList()
        {
            DrawListItems();
            return;

            void DrawListItems()
            {
                EditorGUILayout.Space();
                GUILayout.BeginVertical(EditorStyles.helpBox);
                if (gameObjects == null)
                    gameObjects = Array.Empty<GameObject>();

                _isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(_isExpanded,
                    "Valid Voxelizeable Objects: " + gameObjects.Length);
                DrawVoxelizeButtons();

                if (_isExpanded)
                {
                    _listScrollPosition =
                        EditorGUILayout.BeginScrollView(_listScrollPosition, GUILayout.ExpandHeight(true));
                    for (var i = 0; i < gameObjects.Length; i++)
                        gameObjects[i] = (GameObject)EditorGUILayout.ObjectField($"GameObject {i + 1}",
                            gameObjects[i],
                            typeof(GameObject), true);
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }

            void DrawVoxelizeButtons()
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Scan Voxelizer", EditorStyles.miniButton))
                    voxelizer.GetValidGameObjects(ref gameObjects);
                // Add button to remove the last GameObject
                using (new EditorGUI.DisabledScope(gameObjects.Length == 0))
                {
                    if (GUILayout.Button("Remove GameObject", EditorStyles.miniButton))
                    {
                        var lastIndex = gameObjects.Length - 1;
                        gameObjects[lastIndex].transform.parent = null;
                        ArrayUtility.RemoveAt(ref gameObjects, lastIndex); // Remove the last element
                    }
                }

                GUILayout.EndHorizontal();
                using (new EditorGUI.DisabledScope(gameObjects.Length == 0))
                {
                    if (GUILayout.Button("Voxelize objects", EditorStyles.miniButton))
                        voxelizer.Voxelize(gameObjects, ScriptableObjectTypeRenderer.GetCurrentGridData());
                }
            }
        }

        private static void DrawCreateVoxelizerButtons()
        {
            voxelizerParent =
                (Transform)EditorGUILayout.ObjectField("Target Transform", voxelizerParent, typeof(Transform), true);
            if (!voxelizerParent)
            {
                voxelizer = Object.FindFirstObjectByType<Voxelizer>();
                if (voxelizer) voxelizerParent = voxelizer.transform;

                EditorGUILayout.HelpBox("Assign or Generate the parent object", MessageType.Warning);
                if (GUILayout.Button("Generate Voxelizer GameObject"))
                {
                    var voxelizerGo = new GameObject("Voxelizer");
                    voxelizer = voxelizerGo.AddComponent<Voxelizer>();
                    voxelizerParent = voxelizerGo.transform;
                }

                return;
            }

            var isValid = voxelizerParent.TryGetComponent(out voxelizer);
            if (isValid) return;
            EditorGUILayout.HelpBox("Assigned object does not contain Voxalizer component",
                MessageType.Warning);
            if (GUILayout.Button("Add Voxalizer to assigned object"))
                voxelizerParent.gameObject.AddComponent<Voxelizer>();
        }
    }
}