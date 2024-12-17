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
        private static Voxelizer _voxelizer;

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
        public static void RenderScriptableObjectFields(Type type)
        {
            if (!typeof(ScriptableObject).IsAssignableFrom(type))
            {
                GUILayout.Label("Provided type is not a ScriptableObject.");
                return;
            }

            RenderFields(type);
        }


        /// <summary>
        ///     Renders all supported fields for a given type.
        /// </summary>
        /// <param name="type">The type of the object to inspect.</param>
        public static void RenderFields(Type type)
        {
            // if (type == typeof(GridData))
            Object data = GetCurrentGridData();
            if (type == typeof(VoxelizerGizmosSettings))
                data = VoxelizerSettingsEditor.GetSettings();
            // if (!data) return;


            // Get all fields of the type
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var didChange = false;


            // Iterate over fields
            foreach (var field in fields)
            {
                // Skip fields that are not serialized or are marked [HideInInspector]
                if (!field.IsPublic && !field.IsDefined(typeof(SerializeField), true)) continue;
                if (field.IsDefined(typeof(HideInInspector), true)) continue;

                // Default range values
                float min = 0f, max = 15f;
                // Check for Range or Min/Max attributes
                var rangeAttribute = field.GetCustomAttribute<RangeAttribute>();
                if (rangeAttribute != null)
                {
                    min = rangeAttribute.min;
                    max = rangeAttribute.max;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(ObjectNames.NicifyVariableName(field.Name), GUILayout.ExpandWidth(true));
                // Begin detecting changes for this field
                EditorGUI.BeginChangeCheck();
                var fieldType = field.FieldType;
                object newValue = null;
                // Render field based on its type and detect changes
                if (fieldType == typeof(int))
                    newValue = EditorGUILayout.IntSlider((int)field.GetValue(data), (int)min, (int)max,
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (fieldType == typeof(float))
                    newValue = EditorGUILayout.Slider((float)field.GetValue(data), min, max,
                        GUILayout.ExpandWidth(true));
                else if (fieldType == typeof(string))
                    newValue = EditorGUILayout.TextField((string)field.GetValue(data),
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (fieldType == typeof(bool))
                    newValue = EditorGUILayout.Toggle((bool)field.GetValue(data),
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (fieldType == typeof(Color))
                    newValue = EditorGUILayout.ColorField((Color)field.GetValue(data),
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (fieldType == typeof(Vector3))
                    newValue = EditorGUILayout.Vector3Field("",
                        (Vector3)field.GetValue(data), GUILayout.ExpandWidth(true)); // Assign current value
                else if (fieldType == typeof(Vector3Int))
                    newValue = EditorGUILayout.Vector3IntField("",
                        (Vector3Int)field.GetValue(data), GUILayout.ExpandWidth(true)); // Assign current value
                else if (fieldType.IsEnum)
                    if (fieldType.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
                        // Use EnumFlagsField for flagged enums
                        newValue = EditorGUILayout.EnumFlagsField((Enum)field.GetValue(data),
                            GUILayout.ExpandWidth(true));
                    else
                        // Use EnumPopup for regular enums
                        newValue = EditorGUILayout.EnumPopup((Enum)field.GetValue(data), GUILayout.ExpandWidth(true));
                else
                    GUILayout.Label("Unsupported Type");

                // If a change was made, apply the new value to the ScriptableObject
                if (EditorGUI.EndChangeCheck() && newValue != null)
                {
                    didChange = true;
                    Undo.RecordObject(data, $"Modify {fieldType}>{field.Name}");
                    field.SetValue(data, newValue); // Set the updated value back to the ScriptableObject
                    EditorUtility.SetDirty(data); // Mark as dirty to save changes
                }

                GUILayout.EndHorizontal();
            }

            if (type == typeof(VoxelizerGizmosSettings))
            {
                VoxelizerSettingsEditor.SetSettings((VoxelizerGizmosSettings)data);
            }
            else
            {
                _voxelizer ??= Object.FindFirstObjectByType<Voxelizer>(); // Find the Voxelizer instance
                if (_voxelizer && !_voxelizer.EditorGridData)
                    _voxelizer.EditorGridData = (GridData)data;
            }
        }
    }
}