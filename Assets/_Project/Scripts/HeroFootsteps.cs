using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HeroFootsteps : MonoBehaviour
{
    [SerializeField] float minSpeed = 0.5f;
    [SerializeField] float stepIntervalSeconds = 0.45f;

    public event System.Action Stepped;

    public bool IsWalking => Mathf.Abs(body.linearVelocity.x) >= minSpeed;
    public float StepIntervalSeconds => stepIntervalSeconds;

    Rigidbody2D body;
    float stepTimer;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!IsWalking)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;
        if (stepTimer > 0f) return;

        stepTimer = stepIntervalSeconds;
        SfxPlayer.Instance.Play(SfxPlayer.Instance.Bank.footsteps);
        Stepped?.Invoke();
    }
}
