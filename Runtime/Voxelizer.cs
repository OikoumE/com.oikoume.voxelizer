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
    public int index;

    public GridNode(Vector3 position, GridNodeType nodeType, Vector3 size, int index)
    {
        this.nodeType = nodeType;
        this.position = position;
        this.size = size;
        this.index = index;
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
        foreach (var gridData in gridDataArray)
        {
            if (!gridData || !gridData.IsInitialized) continue;
            var max = gridData.GetSize();
            var nodes = new GridNode[max.x * max.y * max.z];

            for (var z = -max.z; z < max.z; z++)
            for (var y = -max.y; y < max.y; y++)
            for (var x = -max.x; x < max.x; x++)
            {
                var index = (z + max.z) * max.x * max.y + (y + max.y) * max.x + x + max.x;
                var currentOrigin = gridData.NodeOffsetPosition(x, y, z);
                nodes[index] = IterateSubNodes(currentOrigin, gridData);
            }

            gridData.Nodes = nodes;
        }
    }

    
    
    private GridNode IterateSubNodes(Vector3 currOrigin, GridData data)
    {
        var ns = data.SubNodeSize;
        var s = Mathf.CeilToInt(data.subGridResolution / 2f);
        var maxResults = 10;
        var results = new Collider[maxResults];
        for (var z = -s; z < s; z++)
        for (var y = -s; y < s; y++)
        for (var x = -s; x < s; x++)
        {
            var newPos = data.SubNodeOffsetPosition(currOrigin, x, y, z);
            var index = (z + s) * data.subGridResolution * data.subGridResolution
                        + (y + s) * data.subGridResolution
                        + x + s;
            var numberOfHits = Physics.OverlapBoxNonAlloc(newPos, ns * Vector3.one, results);
            if (numberOfHits <= 0) continue;
            // Vector3 position, GridNodeType nodeType, Vector3 size, int index)
            foreach (var result in results)
            {
                //TODO determine the level of marched cube
                // check each collider point or w.e
            }

            //TODO if valid
            return new GridNode(newPos, GridNodeType.Empty, data.nodeSize * Vector3.one, index);
        }

        return null;
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