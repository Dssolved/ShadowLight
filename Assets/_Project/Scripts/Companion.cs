using UnityEngine;

public class Companion : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector2 offset = new Vector2(-0.5f, 1.15f);
    [SerializeField] float followSmoothTime = 0.3f;
    [SerializeField] float bobHeight = 0.12f;
    [SerializeField] float bobSpeed = 3f;

    Vector3 followVelocity;

    void LateUpdate()
    {
        Vector3 goal = target.position + (Vector3)offset;
        goal.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = Vector3.SmoothDamp(transform.position, goal, ref followVelocity, followSmoothTime);
    }
}
