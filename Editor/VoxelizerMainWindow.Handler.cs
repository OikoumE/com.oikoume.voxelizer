namespace Editor
{
    public partial class VoxelizerMainWindow
    {
        private int _lastChildCount;

        private void OnEnable()
        {
            if (VoxelizerRenderer.voxelizerParent)
                _lastChildCount = VoxelizerRenderer.voxelizerParent.transform.childCount;
        }

        private void UpdateOnGUI()
        {
            if (!VoxelizerRenderer.voxelizerParent) return;
            var currentChildCount = VoxelizerRenderer.voxelizerParent.transform.childCount;
            if (currentChildCount == _lastChildCount) return;
            _lastChildCount = currentChildCount;
            VoxelizerRenderer.voxelizer.GetValidGameObjects(ref VoxelizerRenderer.gameObjects);
        }
    }
}