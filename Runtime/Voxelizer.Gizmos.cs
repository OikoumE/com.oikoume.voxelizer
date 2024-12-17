using System;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
[Serializable] //
public class VoxelizerGizmosSettings : Object
{
    [Flags]
    public enum GizmoTypes
    {
        None = 0,
        Node = 1 << 1,
        Bounds = 1 << 2,
        SubNode = 1 << 3,
        All = Node | Bounds | SubNode
    }

    [Range(0.01f, 0.5f)] //
    public float NodeSpace = 0.1f;

    public bool ShowGizmos;
    public GizmoTypes GizmoType = GizmoTypes.All;
}

public partial class Voxelizer
{
    public static VoxelizerGizmosSettings GizmosSettings { get; set; }

    private void OnDrawGizmos()
    {
        if (!GizmosSettings.ShowGizmos) return;
        Gizmos.DrawCube(transform.position, Vector3.one * .25f);

        DrawArray();
        return;


        void DrawArray()
        {
            var c = Gizmos.color;
            if (_gridData == null || _gridData.Length == 0) return;
            foreach (var data in _gridData)
            {
                if (!data || !data.IsInitialized) continue;
                DrawMapOutline(data);
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(data.Position, data.Bounds.size);
                DrawNodes(data);
            }

            Gizmos.color = c;
        }

        void DrawSubNodes(Vector3 currOrigin, GridData data)
        {
            var c = Gizmos.color;
            var s = Mathf.CeilToInt(data.subGridResolution / 2f);
            Gizmos.color = Color.grey * .25f;
            for (var z = -s; z < s; z++)
            for (var y = -s; y < s; y++)
            for (var x = -s; x < s; x++)
                Gizmos.DrawWireCube(data.SubNodeOffsetPosition(currOrigin, x, y, z),
                    Vector3.one * data.SubNodeSize - Vector3.one * nodeVizSpaceSize);
            Gizmos.color = c;
        }

        void DrawNodes(GridData data)
        {
            var c = Gizmos.color;
            var nodeSize = data.nodeSize;
            var center = data.Position;

            Gizmos.DrawWireCube(center, Vector3.one * nodeSize);
            Gizmos.color = Color.red * .5f;

            var max = data.GetSize();
            for (var z = -max.z; z < max.z; z++)
            for (var y = -max.y; y < max.y; y++)
            for (var x = -max.x; x < max.x; x++)
            {
                var currentOrigin = data.NodeOffsetPosition(x, y, z);
                DrawSubNodes(currentOrigin, data);
                Gizmos.DrawWireCube(currentOrigin,
                    Vector3.one * data.nodeSize - Vector3.one * nodeVizSpaceSize);
            }

            Gizmos.color = c;
        }


        void DrawMapOutline(GridData data)
        {
            var c = Gizmos.color;
            var size = data.GridSize + Vector3Int.one * data.extraRows;
            var p = data.Position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(p, size);
            Gizmos.color = c;
        }
    }
}
#endif