using UnityEditor;
using UnityEngine;

namespace Editor
{
    public partial class VoxelizerMainWindow : EditorWindow
    {
        public void OnGUI()
        {
            UpdateOnGUI();
            VoxelizerRenderer.Render();
        }


        [MenuItem("Window/Voxelizer")]
        public static void ShowWindow()
        {
            var window = GetWindow<VoxelizerMainWindow>();
            window.titleContent = new GUIContent("Voxelizer");
            window.Show();
        }

        public static void CloseWindow()
        {
            var window = GetWindow<VoxelizerMainWindow>();
            if (window) window.Close();
        }
    }
}