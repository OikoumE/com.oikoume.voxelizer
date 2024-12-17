#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Voxelizer), true)]
    public class VoxelizerEditor : UnityEditor.Editor
    {
        private void DisableScriptNameRender()
        {
            // Exclude the script field
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
        }

        public override void OnInspectorGUI()
        {
            DisableScriptNameRender();
            // Add default buttons (Save/Apply, etc.)
            GUILayout.Space(10);
            VoxelizerRenderer.Render();
        }
    }
}
#endif