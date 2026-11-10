using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FloatingBlock : MonoBehaviour
{
    public const int CornerCount = 4;

    [SerializeField] float depthFromWall = 5f;

    Vector2[] localCorners;

    public float DepthFromWall => depthFromWall;

    void Awake()
    {
        Bounds spriteBounds = GetComponent<SpriteRenderer>().sprite.bounds;
        Vector2 min = spriteBounds.min;
        Vector2 max = spriteBounds.max;
        localCorners = new[]
        {
            new Vector2(min.x, min.y),
            new Vector2(min.x, max.y),
            new Vector2(max.x, max.y),
            new Vector2(max.x, min.y)
        };
    }

    public void FillWorldCorners(Vector2[] worldCorners)
    {
        for (int i = 0; i < CornerCount; i++)
        {
            worldCorners[i] = transform.TransformPoint(localCorners[i]);
        }
    }
}
