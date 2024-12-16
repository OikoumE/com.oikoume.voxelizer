using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    public static class ScriptableObjectTypeRenderer
    {
        private static bool _isExpanded;
        private static GridData _gridData;

        public static GridData GetCurrentGridData()
        {
            if (!_gridData)
                _gridData = ScriptableObject.CreateInstance<GridData>();
            return _gridData;
        }

        /// <summary>
        ///     Renders all supported fields for a given ScriptableObject type.
        /// </summary>
        /// <param name="type">The type of the ScriptableObject to inspect.</param>
        public static void RenderFields(Type type)
        {
            if (!typeof(ScriptableObject).IsAssignableFrom(type))
            {
                GUILayout.Label("Provided type is not a ScriptableObject.");
                return;
            }

            GUILayout.BeginVertical(EditorStyles.helpBox);

            _isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(_isExpanded,
                $"{type.Name} Settings");
            if (!_isExpanded)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                GUILayout.EndVertical();
                return;
            }

            // Get all fields of the type
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var didChange = false;

            // Iterate over fields
            foreach (var field in fields)
            {
                // Skip fields that are not serialized or are marked [HideInInspector]
                if (!field.IsPublic && !field.IsDefined(typeof(SerializeField), true)) continue;
                if (field.IsDefined(typeof(HideInInspector), true)) continue;


                GUILayout.BeginHorizontal();
                GUILayout.Label(ObjectNames.NicifyVariableName(field.Name), GUILayout.ExpandWidth(true));

                // Begin detecting changes for this field
                EditorGUI.BeginChangeCheck();

                object newValue = null;
                // Render field based on its type and detect changes
                if (field.FieldType == typeof(int))
                    newValue = EditorGUILayout.IntSlider((int)field.GetValue(_gridData), 0, 15,
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType == typeof(float))
                    newValue = EditorGUILayout.Slider((float)field.GetValue(_gridData), 0.1f, 10,
                        GUILayout.ExpandWidth(true));
                else if (field.FieldType == typeof(string))
                    newValue = EditorGUILayout.TextField((string)field.GetValue(_gridData),
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType == typeof(bool))
                    newValue = EditorGUILayout.Toggle((bool)field.GetValue(_gridData),
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType == typeof(Color))
                    newValue = EditorGUILayout.ColorField((Color)field.GetValue(_gridData),
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType == typeof(Vector3))
                    newValue = EditorGUILayout.Vector3Field("",
                        (Vector3)field.GetValue(_gridData), GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType == typeof(Vector3Int))
                    newValue = EditorGUILayout.Vector3IntField("",
                        (Vector3Int)field.GetValue(_gridData), GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType.IsEnum)
                    newValue = EditorGUILayout.EnumPopup((Enum)field.GetValue(_gridData),
                        GUILayout.ExpandWidth(true)); // Assign current value
                else
                    GUILayout.Label("Unsupported Type");

                // If a change was made, apply the new value to the ScriptableObject
                if (EditorGUI.EndChangeCheck() && newValue != null)
                {
                    didChange = true;
                    Undo.RecordObject(_gridData, $"Modify {field.Name}");
                    field.SetValue(_gridData, newValue); // Set the updated value back to the ScriptableObject
                    EditorUtility.SetDirty(_gridData); // Mark as dirty to save changes
                }

                GUILayout.EndHorizontal();
            }


            var voxelizer = Object.FindFirstObjectByType<Voxelizer>(); // Find the Voxelizer instance
            if (voxelizer && (!voxelizer.EditorGridData || didChange))
                voxelizer.EditorGridData = _gridData;
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.EndVertical();
        }
    }
}