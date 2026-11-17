using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] BrazierGlow glow;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Hero hero))
        {
            hero.SetRespawnPoint(transform.position);
            glow.Ignite();
        }
    }
}
