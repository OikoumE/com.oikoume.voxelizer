#if UNITY_EDITOR
using UnityEngine;

#endif


[CreateAssetMenu(fileName = "New GridData", menuName = "MyScriptableObjects/GridData")]
public class GridData : ScriptableObject
{
    [Range(0, 5)] public int extraRows = 1;

    [Range(0, 15)] public int subGridResolution = 1;

    private GameObject _gameObject;


    public Vector3 GridSize => Bounds.size;

    public Vector3Int GridSizeInt => new(
        Mathf.CeilToInt(GridSize.x),
        Mathf.CeilToInt(GridSize.y),
        Mathf.CeilToInt(GridSize.z));

    public Vector3 Position => Bounds.center;

    public MeshCollider VoxelizeObjectCollider { get; private set; }

    public int GridSizeCubed => Mathf.CeilToInt(GridSize.x * GridSize.y * GridSize.z);

    public Bounds Bounds
    {
        get
        {
            if (VoxelizeObjectCollider) return VoxelizeObjectCollider.bounds;
            return new Bounds(Vector3.zero, Vector3.zero); // Return empty bounds when no OtherCollider
        }
    }

    public bool IsInitialized { get; private set; }
    public GridNode[,,] Nodes { get; set; }

    public Vector3 NodeOffsetPosition(int x, int y, int z)
    {
        var hn = Vector3.one / 2;
        var offset = Vector3.one * extraRows;
        return Position - hn - offset + new Vector3(x, y, z);
    }


    public Vector3Int GetSize()
    {
        var s = GridSize;
        var e = extraRows;
        return new Vector3Int(
            Mathf.CeilToInt(s.x) + e * 2,
            Mathf.CeilToInt(s.y) + e * 2,
            Mathf.CeilToInt(s.z) + e * 2
        );
    }

    public void Initialize(GameObject gameObject, MeshCollider meshCollider)
    {
        _gameObject = gameObject;
        name = _gameObject.name;
        VoxelizeObjectCollider = meshCollider;
        IsInitialized = true;
    }
}