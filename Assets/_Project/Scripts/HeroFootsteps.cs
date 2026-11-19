using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HeroFootsteps : MonoBehaviour
{
    [SerializeField] float minSpeed = 0.5f;
    [SerializeField] float stepIntervalSeconds = 0.45f;

    Rigidbody2D body;
    float stepTimer;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Mathf.Abs(body.linearVelocity.x) < minSpeed)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;
        if (stepTimer > 0f) return;

        stepTimer = stepIntervalSeconds;
        SfxPlayer.Instance.Play(SfxPlayer.Instance.Bank.footsteps);
    }
}
