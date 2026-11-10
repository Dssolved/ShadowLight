using UnityEngine;

public class FloatingBlock : MonoBehaviour
{
    [SerializeField] float depthFromWall = 5f;

    static readonly Vector2[] unitSquareCorners =
    {
        new Vector2(-0.5f, -0.5f),
        new Vector2(-0.5f, 0.5f),
        new Vector2(0.5f, 0.5f),
        new Vector2(0.5f, -0.5f)
    };

    public float DepthFromWall => depthFromWall;
    public int CornerCount => unitSquareCorners.Length;

    public void FillWorldCorners(Vector2[] worldCorners)
    {
        for (int i = 0; i < unitSquareCorners.Length; i++)
        {
            worldCorners[i] = transform.TransformPoint(unitSquareCorners[i]);
        }
    }
}
