using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class BlockShadow : MonoBehaviour
{
    const int ConeVertexCount = FloatingBlock.CornerCount * 2;

    [SerializeField] FloatingBlock block;
    [SerializeField] LanternController lantern;
    [SerializeField] Transform shadowVisual;
    [SerializeField] MeshFilter coneFilter;
    [SerializeField] Color coneColor = new Color(0.11f, 0.086f, 0.149f, 1f);
    [SerializeField] float coneAlphaAtBlock = 0.3f;
    [SerializeField] float coneAlphaAtShadow = 0.08f;

    PolygonCollider2D shadowCollider;
    Vector2[] blockCorners;
    Vector2[] shadowCorners;
    Mesh coneMesh;
    Vector3[] coneVertices;

    void Awake()
    {
        shadowCollider = GetComponent<PolygonCollider2D>();
        blockCorners = new Vector2[FloatingBlock.CornerCount];
        shadowCorners = new Vector2[FloatingBlock.CornerCount];
        BuildConeMesh();
    }

    public void Init(LanternController sceneLantern)
    {
        lantern = sceneLantern;
    }

    void BuildConeMesh()
    {
        int cornerCount = FloatingBlock.CornerCount;
        coneVertices = new Vector3[ConeVertexCount];
        Color[] colors = new Color[ConeVertexCount];
        int[] triangles = new int[cornerCount * 6];
        Color linearColor = coneColor.linear;

        for (int i = 0; i < cornerCount; i++)
        {
            colors[i] = new Color(linearColor.r, linearColor.g, linearColor.b, coneAlphaAtBlock);
            colors[cornerCount + i] = new Color(linearColor.r, linearColor.g, linearColor.b, coneAlphaAtShadow);

            int next = (i + 1) % cornerCount;
            int t = i * 6;
            triangles[t] = i;
            triangles[t + 1] = next;
            triangles[t + 2] = cornerCount + next;
            triangles[t + 3] = i;
            triangles[t + 4] = cornerCount + next;
            triangles[t + 5] = cornerCount + i;
        }

        coneMesh = new Mesh();
        coneMesh.vertices = coneVertices;
        coneMesh.colors = colors;
        coneMesh.triangles = triangles;
        coneFilter.mesh = coneMesh;
    }

    void LateUpdate()
    {
        Vector2 lightPosition = lantern.transform.position;
        float scale = lantern.DepthFromWall / (lantern.DepthFromWall - block.DepthFromWall);

        block.FillWorldCorners(blockCorners);
        Vector2 blockCenter = Vector2.zero;
        for (int i = 0; i < blockCorners.Length; i++)
        {
            Vector2 worldShadowCorner = lightPosition + (blockCorners[i] - lightPosition) * scale;
            shadowCorners[i] = transform.InverseTransformPoint(worldShadowCorner);
            blockCenter += blockCorners[i];

            coneVertices[i] = coneFilter.transform.InverseTransformPoint(blockCorners[i]);
            coneVertices[FloatingBlock.CornerCount + i] = coneFilter.transform.InverseTransformPoint(worldShadowCorner);
        }
        blockCenter /= blockCorners.Length;

        shadowCollider.SetPath(0, shadowCorners);

        Vector2 shadowCenter = lightPosition + (blockCenter - lightPosition) * scale;
        shadowVisual.position = new Vector3(shadowCenter.x, shadowCenter.y, shadowVisual.position.z);
        shadowVisual.localScale = block.transform.lossyScale * scale;

        coneMesh.vertices = coneVertices;
        coneMesh.RecalculateBounds();
    }
}
