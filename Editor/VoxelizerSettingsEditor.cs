using UnityEngine;

namespace Editor
{
    public static class VoxelizerSettingsEditor
    {
        private static VoxelizerGizmosSettings _settings;

        public static VoxelizerGizmosSettings GetSettings()
        {
            if (_settings) return _settings;
            _settings = ScriptableObject.CreateInstance<VoxelizerGizmosSettings>();
            return _settings;
        }

        public static void SetSettings(VoxelizerGizmosSettings settings)
        {
            _settings = settings;
            Voxelizer.gizmosSettings = _settings;
        }
    }
}