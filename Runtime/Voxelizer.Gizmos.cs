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
                if (!data) continue;
                var c = Gizmos.color;
                DrawMapOutline(data);
                Gizmos.color = c;
                DrawNodes(data);
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(data.Position, data.Bounds.size);
                Gizmos.color = c;
            }
        }


        void DrawNodes(GridData data)
        {
            var extra = data.extraRows;
            var s = data.GridSize;
            var nodeSize = data.nodeSize;
            var halfNodeSize = nodeSize / 2;
            var center = data.Position;

            var maxX = Mathf.CeilToInt((s.x + extra * 2) / nodeSize);
            var maxY = Mathf.CeilToInt((s.y + extra * 2) / nodeSize);
            var maxZ = Mathf.CeilToInt((s.z + extra * 2) / nodeSize);


            // var origin = center - halfGridSize;
            //confirm origin
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(center, Vector3.one * nodeSize);

            for (var z = -maxX / 2; z < maxX / 2; z++)
            for (var y = -maxY / 2; y < maxY / 2; y++)
            for (var x = -maxZ / 2; x < maxZ / 2; x++)
            {
                var newOffset = new Vector3(halfNodeSize + nodeSize * x, halfNodeSize + nodeSize * y,
                    halfNodeSize + nodeSize * z);
                Gizmos.color = Color.red * .5f;
                Gizmos.DrawWireCube(center + newOffset,
                    Vector3.one * data.nodeSize - Vector3.one * nodeVizSpaceSize);
            }
        }


        void DrawMapOutline(GridData data)
        {
            var size = data.GridSize + Vector3Int.one * data.extraRows;
            var c2 = Gizmos.color;
            var p = data.Position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(p, size);
            Gizmos.color = c2;
        }
    }
}
#endif