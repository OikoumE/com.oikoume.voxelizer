using UnityEngine;

#if UNITY_EDITOR

public partial class Voxelizer
{
    [Range(0.01f, 0.5f)] //
    public float nodeVizSpaceSize = 0.1f;

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, Vector3.one * .25f);

        DrawArray();
        return;


        void DrawArray()
        {
            if (_gridData == null || _gridData.Length == 0) return;
            foreach (var data in _gridData)
            {
                if (!data || !data.IsInitialized) continue;
                var c = Gizmos.color;
                DrawMapOutline(data);
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(data.Position, data.Bounds.size);
                DrawNodes(data);
                Gizmos.color = c;
            }
        }

        void DrawSubNodes(Vector3 currOrigin, GridData data)
        {
            var s = Mathf.CeilToInt(data.subGridResolution / 2f);
            Gizmos.color = Color.grey * .25f;
            for (var z = -s; z < s; z++)
            for (var y = -s; y < s; y++)
            for (var x = -s; x < s; x++)
            {
                var newOffset  = data.SubNodeOffsetPosition(currOrigin, x, y, z);
                Gizmos.DrawWireCube(currOrigin + newOffset,
                    Vector3.one * data.SubNodeSize - Vector3.one * nodeVizSpaceSize);
            }
        }

        void DrawNodes(GridData data)
        {
            var nodeSize = data.nodeSize;
            var center = data.Position;

            Gizmos.DrawWireCube(center, Vector3.one * nodeSize);
            Gizmos.color = Color.red * .5f;

            var max = data.GetSize();
            for (var z = -max.z; z < max.z; z++)
            for (var y = -max.y; y < max.y; y++)
            for (var x = -max.x; x < max.x; x++)
            {
                var currentOrigin = center + data.NodeOffsetPosition( x,y,z);
                DrawSubNodes(currentOrigin, data);
                Gizmos.DrawWireCube(currentOrigin,
                    Vector3.one * data.nodeSize - Vector3.one * nodeVizSpaceSize);
            }
        }


        void DrawMapOutline(GridData data)
        {
            var size = data.GridSize + Vector3Int.one * data.extraRows;
            var p = data.Position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(p, size);
        }
    }
}
#endif