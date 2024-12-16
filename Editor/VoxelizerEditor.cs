#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Voxelizer), true)]
    public class VoxelizerEditor : UnityEditor.Editor
    {
        private static void DrawGridData()
        {
            GUILayout.Space(10);
            // ScriptableObjectTypeRenderer.RenderFields(typeof(GridData));
            VoxelizerRenderer.Render();
        }

        public override void OnInspectorGUI()
        {
            // Add default buttons (Save/Apply, etc.)
            DrawDefaultInspector();
            DrawGridData();
        }
    }
}
#endif