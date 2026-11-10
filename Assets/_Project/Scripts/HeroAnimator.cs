using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class HeroAnimator : MonoBehaviour
{
    static readonly int SpeedParameter = Animator.StringToHash("Speed");
    static readonly int FallingParameter = Animator.StringToHash("Falling");

    [SerializeField] float fallingSpeedThreshold = 1f;

    Rigidbody2D body;
    Animator animator;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 velocity = body.linearVelocity;
        animator.SetFloat(SpeedParameter, Mathf.Abs(velocity.x));
        animator.SetBool(FallingParameter, velocity.y < -fallingSpeedThreshold);
    }
}
