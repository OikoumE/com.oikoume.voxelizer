using System;
using UnityEngine;

#if UNITY_EDITOR
[Serializable] //
public class VoxelizerGizmosSettings : ScriptableObject
{
    [Flags]
    public enum GizmoTypes
    {
        None = 0,
        Node = 1 << 1,
        Bounds = 1 << 2,
        SubNode = 1 << 3
    }

    [Range(0.01f, 0.5f)] //
    public float nodeSpace = 0.01f;

    public GizmoTypes gizmoType = GizmoTypes.Bounds | GizmoTypes.SubNode | GizmoTypes.Node;
}

public partial class Voxelizer
{
    public static VoxelizerGizmosSettings gizmosSettings;
    private readonly VoxelizerGizmosSettings.GizmoTypes _boundsType = VoxelizerGizmosSettings.GizmoTypes.Bounds;
    private readonly VoxelizerGizmosSettings.GizmoTypes _nodeType = VoxelizerGizmosSettings.GizmoTypes.Node;
    private readonly VoxelizerGizmosSettings.GizmoTypes _subNodeType = VoxelizerGizmosSettings.GizmoTypes.SubNode;

    private void OnDrawGizmos()
    {
        if (!gizmosSettings || gizmosSettings.gizmoType == 0) return;
        CheckGizTypes(out var drawNode, out var drawSubNode, out var drawBounds);
        DrawArray();
        return;

        void DrawArray()
        {
            var c = Gizmos.color;
            if (_gridData == null || _gridData.Length == 0) return;
            foreach (var data in _gridData)
            {
                if (!data || !data.IsInitialized) continue;
                if (drawBounds)
                    DrawMapOutline(data);
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(data.Position, data.Bounds.size);
                DrawNodes(data);
            }

            Gizmos.color = c;
        }

        void DrawSubNodes(Vector3 currOrigin, GridData data, bool[,,] currNode = null)
        {
            var s = data.subGridResolution;
            var size = 1 / data.subGridResolution;
            var halfSize = size / 2f * Vector3.one;

            // origin
            var halfNs = Vector3.one / 2f;
            var newOrigin = currOrigin + halfNs - halfSize;

            Gizmos.color = Color.grey * .5f;
            for (var z = 0; z < s; z++)
            for (var y = 0; y < s; y++)
            for (var x = 0; x < s; x++)
            {
                // Gizmos.color = currNode[x, y, z] ? Color.green : Color.red;
                var newOffset = new Vector3(x, y, z) * size;
                Gizmos.DrawWireCube(newOrigin - newOffset,
                    Vector3.one * size - Vector3.one * gizmosSettings.nodeSpace);
            }
        }

        void DrawNodes(GridData data)
        {
            var c = Gizmos.color;
            var center = data.Position;
            var extra = data.extraRows;

            Gizmos.DrawWireCube(center, Vector3.one);

            var max = data.GetSize();
            for (var z = 0; z < max.z; z++)
            for (var y = 0; y < max.y; y++)
            for (var x = 0; x < max.x; x++)
            {
                // var node = data.Nodes[x, y, z];

                var currentOrigin = data.NodeOffsetPosition(x, y, z);
                if (drawSubNode)
                    DrawSubNodes(currentOrigin, data); //, node.SubNodes);
                Gizmos.color = Color.red * .5f;
                if (drawNode)
                    Gizmos.DrawWireCube(currentOrigin,
                        Vector3.one - Vector3.one * gizmosSettings.nodeSpace);
            }

            Gizmos.color = c;
        }


        void DrawMapOutline(GridData data)
        {
            var c = Gizmos.color;
            var size = data.GetSize();
            var p = data.Position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(p, size);
            Gizmos.color = c;
        }

        void CheckGizTypes(out bool node, out bool subNode, out bool bounds)
        {
            var gizType = gizmosSettings.gizmoType;

            node = (gizType & _nodeType) == _nodeType;
            subNode = (gizType & _subNodeType) == _subNodeType;
            bounds = (gizType & _boundsType) == _boundsType;
        }
    }
}
#endif