using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float lifetimeSeconds = 4f;
    [SerializeField] LayerMask wallMask;

    Rigidbody2D body;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetimeSeconds);
    }

    public void Launch(Vector2 direction)
    {
        transform.right = direction;
        body.linearVelocity = direction.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Hero hero))
        {
            hero.Die();
            Destroy(gameObject);
        }
        else if (IsWall(other.gameObject))
        {
            Destroy(gameObject);
        }
    }

    bool IsWall(GameObject other)
    {
        return (wallMask.value & (1 << other.layer)) != 0;
    }
}
