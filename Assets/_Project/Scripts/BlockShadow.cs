using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class BlockShadow : MonoBehaviour
{
    [SerializeField] FloatingBlock block;
    [SerializeField] LanternController lantern;
    [SerializeField] Transform shadowVisual;

    PolygonCollider2D shadowCollider;
    Vector2[] blockCorners;
    Vector2[] shadowCorners;

    void Awake()
    {
        shadowCollider = GetComponent<PolygonCollider2D>();
        blockCorners = new Vector2[FloatingBlock.CornerCount];
        shadowCorners = new Vector2[FloatingBlock.CornerCount];
    }

    public void Init(LanternController sceneLantern)
    {
        lantern = sceneLantern;
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
        }
        blockCenter /= blockCorners.Length;

        shadowCollider.SetPath(0, shadowCorners);

        Vector2 shadowCenter = lightPosition + (blockCenter - lightPosition) * scale;
        shadowVisual.position = new Vector3(shadowCenter.x, shadowCenter.y, shadowVisual.position.z);
        shadowVisual.localScale = block.transform.lossyScale * scale;
    }
}
