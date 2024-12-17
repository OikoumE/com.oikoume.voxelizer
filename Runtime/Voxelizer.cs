using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[Serializable] //
public class GridNode
{
    public GridNodeType nodeType;
    public Vector3 position;
    public Vector3 size;
    public Vector3Int index;
    public bool[,,] SubNodes;

    public GridNode(Vector3 position, GridNodeType nodeType, Vector3 size, Vector3Int index, bool[,,] subNodes)
    {
        this.nodeType = nodeType;
        this.position = position;
        this.size = size;
        this.index = index;
        SubNodes = subNodes;
    }
}

public enum GridNodeType
{
    Empty,
    Occupied,
    Blocked
}

public partial class Voxelizer : MonoBehaviour
{
    private GameObject[] _gameObjects;
    private GridData[] _gridData;

    [NonSerialized] //
    public GridData EditorGridData;

    public void BuildInitialGridData(GridData[] gridDataArray)
    {
        if (gridDataArray == null || gridDataArray.Length == 0) return;
        foreach (var data in gridDataArray)
        {
            if (!data || !data.IsInitialized) continue;
            var extra = data.extraRows;

            var max = data.GetSize();
            var nodes = new GridNode[max.x, max.y, max.z];
            var halfExtents = Vector3.one / 2;
            for (var z = 0; z < max.z; z++)
            for (var y = 0; y < max.y; y++)
            for (var x = 0; x < max.x; x++)
            {
                var currentOrigin = data.NodeOffsetPosition(x, y, z);
                if (!Physics.CheckBox(currentOrigin, halfExtents)) continue;
                var subNodes = IterateSubNodes(currentOrigin, data);
                var index = new Vector3Int(x, y, z);
                var node = new GridNode(currentOrigin, GridNodeType.Empty, Vector3.one, index, subNodes);
                nodes[x, y, z] = node;
            }

            data.Nodes = nodes;
        }
    }


    private bool[,,] IterateSubNodes(Vector3 currOrigin, GridData data)
    {
        var subNodeSize = 1 / data.subGridResolution;
        // origin
        var halfNodeSize = Vector3.one * 1f / 2f;
        var halfSubNodeSize = subNodeSize / 2f * Vector3.one;
        var newOrigin = currOrigin + halfNodeSize - halfSubNodeSize;


        var res = data.subGridResolution;
        var subNodes = new bool[res, res, res];

        for (var z = 0; z < res; z++)
        for (var y = 0; y < res; y++)
        for (var x = 0; x < res; x++)
        {
            var subPosition = newOrigin - new Vector3(x, y, z) * subNodeSize;
            var halfExtents = Vector3.one * subNodeSize / 2;
            var index = z * res * res + y * res + x;

            subNodes[x, y, z] = Physics.CheckBox(subPosition, halfExtents);
        }

        return subNodes;
    }

    public void GetValidGameObjects(ref GameObject[] gameObjects)
    {
        if (transform.childCount == 0)
        {
            Debug.Log("No valid gameObjects found, make sure they are children of the Voxelizer");
            return;
        }

        var childCount = transform.childCount;
        var childObjects = new GameObject[childCount];

        for (var i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            var childGameObject = child.gameObject;
            if (child.childCount > 0)
            {
                Debug.LogError("Invalid children found, " +
                               "make sure they are direct child of the Voxelizer and " +
                               "have no children themselves." +
                               $"Removed <{child.name}>");
                RemoveInvalidChild(child, i, ref childObjects);
                continue;
            }

            if (!childGameObject.TryGetComponent(out MeshCollider meshCollider))
            {
                Debug.LogError("Invalid child found, make sure they all have MeshCollider components. " +
                               $"Removed <{child.name}>");
                RemoveInvalidChild(child, i, ref childObjects);
                continue;
            }

            childObjects[i] = childGameObject;
        }

        gameObjects = childObjects;
    }

    private void RemoveInvalidChild(Transform child, int i, ref GameObject[] children)
    {
        child.parent = null;
        ArrayUtility.RemoveAt(ref children, i);
    }

    public void Voxelize(GameObject[] gameObjects, GridData currentSettings)
    {
        _gameObjects = gameObjects;


        if (_gridData is { Length: > 0 })
            foreach (var gridData in _gridData.ToArray())
                DestroyImmediate(gridData); // Destroy ScriptableObject instance

        _gridData = new GridData[gameObjects.Length];
        for (var i = 0; i < _gameObjects.Length; i++)
        {
            var o = _gameObjects[i];
            if (!o.TryGetComponent(out MeshCollider meshCollider)) continue;


            var newData = CloneGridData(currentSettings);
            newData.Initialize(o, meshCollider);
            _gridData[i] = newData;
        }

        BuildInitialGridData(_gridData);


        return;

        GridData CloneGridData(GridData gridData)
        {
            var newData = ScriptableObject.CreateInstance<GridData>();
            var fields = gridData.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.IsNotSerialized) continue; // Skip non-serialized fields
                var value = field.GetValue(gridData);
                field.SetValue(newData, value);
            }

            return newData;
        }

        //TODO grid and slap it
    }
}