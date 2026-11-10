using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D), typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class BlockShadow : MonoBehaviour
{
    [SerializeField] FloatingBlock block;
    [SerializeField] LanternController lantern;
    [SerializeField] Color shadowColor = new Color(0.553f, 0.498f, 0.69f);

    PolygonCollider2D shadowCollider;
    LineRenderer edgeLine;
    Mesh shadowMesh;
    Vector2[] blockCorners;
    Vector2[] shadowCorners;
    Vector3[] meshVertices;

    void Awake()
    {
        shadowCollider = GetComponent<PolygonCollider2D>();
        edgeLine = GetComponent<LineRenderer>();

        int cornerCount = block.CornerCount;
        edgeLine.positionCount = cornerCount;
        blockCorners = new Vector2[cornerCount];
        shadowCorners = new Vector2[cornerCount];
        meshVertices = new Vector3[cornerCount];

        Color[] vertexColors = new Color[cornerCount];
        for (int i = 0; i < cornerCount; i++)
        {
            vertexColors[i] = shadowColor.linear;
        }

        shadowMesh = new Mesh();
        shadowMesh.vertices = meshVertices;
        shadowMesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
        shadowMesh.colors = vertexColors;
        GetComponent<MeshFilter>().mesh = shadowMesh;
    }

    void LateUpdate()
    {
        Vector2 lightPosition = lantern.transform.position;
        float scale = lantern.DepthFromWall / (lantern.DepthFromWall - block.DepthFromWall);

        block.FillWorldCorners(blockCorners);
        for (int i = 0; i < blockCorners.Length; i++)
        {
            Vector2 worldShadowCorner = lightPosition + (blockCorners[i] - lightPosition) * scale;
            shadowCorners[i] = transform.InverseTransformPoint(worldShadowCorner);
            meshVertices[i] = shadowCorners[i];
        }

        shadowCollider.SetPath(0, shadowCorners);
        shadowMesh.vertices = meshVertices;
        shadowMesh.RecalculateBounds();
        edgeLine.SetPositions(meshVertices);
    }
}
