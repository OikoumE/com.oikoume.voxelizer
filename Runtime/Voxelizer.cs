using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public partial class Voxelizer : MonoBehaviour
{
    private GameObject[] _gameObjects;
    private GridData[] _gridData;

    [NonSerialized] //
    public GridData EditorGridData;

    public void BuildInitialGridData()
    {
        if (_gridData == null || _gridData.Length == 0) return;
        foreach (var data in _gridData)
        {
            if (!data || !data.IsInitialized) continue;
            var max = data.GetSize();
            for (var z = -max.z; z < max.z; z++)
            for (var y = -max.y; y < max.y; y++)
            for (var x = -max.x; x < max.x; x++)
            {
                var currentOrigin = data.NodeOffsetPosition(x, y, z);
                IterateSubNodes(currentOrigin, data);
            }
        }
    }

    private void IterateSubNodes(Vector3 currOrigin, GridData data)
    {
        var s = Mathf.CeilToInt(data.subGridResolution / 2f);
        for (var z = -s; z < s; z++)
        for (var y = -s; y < s; y++)
        for (var x = -s; x < s; x++)
        {
            var newOffset = data.SubNodeOffsetPosition(currOrigin, x, y, z);
            var currPos = currOrigin + newOffset;
        }
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