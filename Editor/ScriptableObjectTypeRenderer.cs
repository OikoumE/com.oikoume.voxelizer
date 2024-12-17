﻿using System;
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
                if (!data) return;
                object newValue = null;
                // Render field based on its type and detect changes
                if (field.FieldType == typeof(int))
                    newValue = EditorGUILayout.IntSlider((int)field.GetValue(data), 0, 15,
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType == typeof(float))
                    newValue = EditorGUILayout.Slider((float)field.GetValue(data), 0.1f, 10,
                        GUILayout.ExpandWidth(true));
                else if (field.FieldType == typeof(string))
                    newValue = EditorGUILayout.TextField((string)field.GetValue(data),
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType == typeof(bool))
                    newValue = EditorGUILayout.Toggle((bool)field.GetValue(data),
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType == typeof(Color))
                    newValue = EditorGUILayout.ColorField((Color)field.GetValue(data),
                        GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType == typeof(Vector3))
                    newValue = EditorGUILayout.Vector3Field("",
                        (Vector3)field.GetValue(data), GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType == typeof(Vector3Int))
                    newValue = EditorGUILayout.Vector3IntField("",
                        (Vector3Int)field.GetValue(data), GUILayout.ExpandWidth(true)); // Assign current value
                else if (field.FieldType.IsEnum)
                    newValue = EditorGUILayout.EnumPopup((Enum)field.GetValue(data),
                        GUILayout.ExpandWidth(true)); // Assign current value
                else
                    GUILayout.Label("Unsupported Type");

                // If a change was made, apply the new value to the ScriptableObject
                if (EditorGUI.EndChangeCheck() && newValue != null)
                {
                    didChange = true;


                    Undo.RecordObject(data, $"Modify {field.Name}");
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
                if (_voxelizer && (!_voxelizer.EditorGridData || didChange))
                    _voxelizer.EditorGridData = (GridData)data;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.EndVertical();
        }
    }
}