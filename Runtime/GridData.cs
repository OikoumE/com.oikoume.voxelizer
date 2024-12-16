#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine;

#endif


[CreateAssetMenu(fileName = "New GridData", menuName = "MyScriptableObjects/GridData")]
public class GridData : ScriptableObject
{
    public int extraRows = 1;

    [Header("Node Settings")] //
    [Min(0)]
    public float nodeSize = 1f;

    [Min(0)] public int subGridSize = 10;
    private GameObject _gameObject;

    private Node[] _nodes;

    [Header("Grid Settings")] //
    public Vector3 GridSize => Bounds.size;

    public Vector3Int GridSizeInt => new(
        Mathf.CeilToInt(GridSize.x),
        Mathf.CeilToInt(GridSize.y),
        Mathf.CeilToInt(GridSize.z));

    public Vector3 Position => _gameObject.transform.position;

    public MeshCollider VoxelizeObjectCollider { get; private set; }

    public int GridSizeCubed => Mathf.CeilToInt(GridSize.x * GridSize.y * GridSize.z);
    public Vector3 NodeSizeV3 => Vector3.one * nodeSize;
    public float SubNodeSize => nodeSize / subGridSize;
    public Vector3 SubSizeV3 => NodeSizeV3 / subGridSize;

    public Bounds Bounds
    {
        get
        {
            if (VoxelizeObjectCollider) return VoxelizeObjectCollider.bounds;
            return new Bounds(Vector3.zero, Vector3.zero); // Return empty bounds when no OtherCollider
        }
    }

    public bool IsInitialized { get; private set; }

    public void Initialize(GameObject gameObject, MeshCollider meshCollider)
    {
        _gameObject = gameObject;
        name = _gameObject.name;
        VoxelizeObjectCollider = meshCollider;
        IsInitialized = true;
    }
}