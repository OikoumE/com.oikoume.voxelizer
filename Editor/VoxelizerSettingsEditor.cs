namespace Editor
{
    public static class VoxelizerSettingsEditor
    {
        private static VoxelizerGizmosSettings _settings;
        private static Voxelizer _voxelizer;

        public static VoxelizerGizmosSettings GetSettings()
        {
            if (!_settings)
                _settings = new VoxelizerGizmosSettings();
            return _settings;
        }

        public static void SetSettings(VoxelizerGizmosSettings settings)
        {
            _settings = settings;
            Voxelizer.GizmosSettings = _settings;
        }
    }
}