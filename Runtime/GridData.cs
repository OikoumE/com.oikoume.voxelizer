using UnityEngine.Serialization;
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

    [FormerlySerializedAs("subGridSize")] [Min(0)]
    public int subGridResolution = 2;

    private GameObject _gameObject;

    private Node[] _nodes;

    [Header("Grid Settings")] //
    public Vector3 GridSize => Bounds.size;

    public Vector3Int GridSizeInt => new(
        Mathf.CeilToInt(GridSize.x),
        Mathf.CeilToInt(GridSize.y),
        Mathf.CeilToInt(GridSize.z));

    public Vector3 Position => Bounds.center;

    public MeshCollider VoxelizeObjectCollider { get; private set; }

    public int GridSizeCubed => Mathf.CeilToInt(GridSize.x * GridSize.y * GridSize.z);
    public Vector3 NodeSizeV3 => Vector3.one * nodeSize;
    public float SubNodeSize => nodeSize / subGridResolution;
    public Vector3 SubSizeV3 => NodeSizeV3 / subGridResolution;

    public Bounds Bounds
    {
        get
        {
            if (VoxelizeObjectCollider) return VoxelizeObjectCollider.bounds;
            return new Bounds(Vector3.zero, Vector3.zero); // Return empty bounds when no OtherCollider
        }
    }

    public bool IsInitialized { get; private set; }
    public GridNode[] Nodes { get; set; }

    public Vector3 NodeOffsetPosition(int x, int y, int z)
    {
        var ns = nodeSize;
        var halfNs = Vector3.one * ns / 2f;
        return Position + halfNs + new Vector3(ns * x, ns * y, ns * z);
    }

    public Vector3 SubNodeOffsetPosition(Vector3 currOrigin, int x, int y, int z)
    {
        var ns = SubNodeSize;
        var halfNs = Vector3.one * ns / 2f;

        return currOrigin + halfNs + new Vector3(ns * x, ns * y, ns * z);
    }

    public Vector3Int GetSize()
    {
        var ns = nodeSize;
        var s = GridSize;
        var e = extraRows;
        return new Vector3Int(
            (Mathf.CeilToInt(s.x / ns) + e) / 2,
            (Mathf.CeilToInt(s.y / ns) + e) / 2,
            (Mathf.CeilToInt(s.z / ns) + e) / 2
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