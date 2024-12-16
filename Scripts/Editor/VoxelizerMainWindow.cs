using UnityEditor;
using UnityEngine;

namespace Packages.com.oikoume.voxelizer.Scripts.Editor
{
    public class VoxelizerMainWindow : EditorWindow
    {
        private const int FontSize = 12;
        private int _subdivision;
        private float _voxelSize;

        public void OnGUI()
        {
            // Shows window content depending on state
            GUILayout.Label("Voxelizer", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("", MessageType.Warning);
            VoxelizerEditorHelper.DrawIntField("Sub Divisions:", _subdivision, FontSize);

            using (new EditorGUI.DisabledScope())
            {
                if (GUILayout.Button("var1"))
                {
                }
            }
        }
    }
}