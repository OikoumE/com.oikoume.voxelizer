﻿using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class Voxelizer : MonoBehaviour
{
    private GameObject[] _gameObjects;
    private GridData[] _gridData;

    [NonSerialized] //
    public GridData EditorGridData;

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

        if (_gridData.Length > 0)
            foreach (var gridData in _gridData.ToArray())
                DestroyImmediate(gridData); // Destroy ScriptableObject instance

        _gridData = new GridData[gameObjects.Length];
        for (var i = 0; i < _gameObjects.Length; i++)
        {
            var o = _gameObjects[i];
            if (!o.TryGetComponent(out MeshCollider meshCollider)) continue;

            var newData = ScriptableObject.CreateInstance<GridData>();
            var s = meshCollider.bounds.size;


            newData.nodeSize = currentSettings.nodeSize;
            newData.subGridSize = currentSettings.subGridSize;
            newData.Initialize(o, meshCollider);
            _gridData[i] = newData;
        }

        void CalculateGridSize()
        {
            // var nodeSize = data.nodeSize;
            //
            // var nodesZ = s.z / nodeSize;
            // var nodesY = s.y / nodeSize;
            // var nodesX = s.x / nodeSize;
        }


        //TODO grid and slap it
    }
}