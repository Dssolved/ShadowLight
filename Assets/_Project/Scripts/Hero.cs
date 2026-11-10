using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Hero : MonoBehaviour
{
    enum HeroState { Walk, Wait, Fall, Respawn, ReachedDoor }

    [SerializeField] float walkSpeed = 1.5f;
    [SerializeField] float groundCheckDistance = 0.6f;
    [SerializeField] float edgeLookAhead = 0.4f;
    [SerializeField] float edgeCheckDistance = 0.9f;
    [SerializeField] float fallDeathHeight = -6f;
    [SerializeField] float respawnDelaySeconds = 1f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Lives lives;

    Rigidbody2D body;
    HeroState state;
    float stateTimer;
    Vector2 respawnPoint;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        respawnPoint = transform.position;
        SetState(HeroState.Fall);
    }

    void Update()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case HeroState.Walk:
                SetHorizontalSpeed(walkSpeed);
                if (!IsGrounded()) SetState(HeroState.Fall);
                else if (!HasGroundAhead()) SetState(HeroState.Wait);
                break;

            case HeroState.Wait:
                SetHorizontalSpeed(0f);
                if (!IsGrounded()) SetState(HeroState.Fall);
                else if (HasGroundAhead()) SetState(HeroState.Walk);
                break;

            case HeroState.Fall:
                SetHorizontalSpeed(0f);
                if (transform.position.y < fallDeathHeight) Die();
                else if (IsGrounded()) SetState(HeroState.Walk);
                break;

            case HeroState.Respawn:
                if (stateTimer >= respawnDelaySeconds)
                {
                    if (lives.HasLivesLeft) ReturnToRespawnPoint();
                    else lives.RestartLevel();
                }
                break;

            case HeroState.ReachedDoor:
                break;
        }
    }

    public void SetRespawnPoint(Vector2 point)
    {
        respawnPoint = point;
    }

    public void EnterDoor()
    {
        SetState(HeroState.ReachedDoor);
    }

    public void Die()
    {
        if (state == HeroState.Respawn || state == HeroState.ReachedDoor) return;

        lives.LoseLife();
        SetState(HeroState.Respawn);
    }

    void ReturnToRespawnPoint()
    {
        transform.position = respawnPoint;
        body.linearVelocity = Vector2.zero;
        body.simulated = true;
        SetState(HeroState.Fall);
    }

    void SetState(HeroState next)
    {
        state = next;
        stateTimer = 0f;

        if (next == HeroState.Respawn || next == HeroState.ReachedDoor)
        {
            body.linearVelocity = Vector2.zero;
            body.simulated = false;
        }
    }

    void SetHorizontalSpeed(float speed)
    {
        body.linearVelocity = new Vector2(speed, body.linearVelocity.y);
    }

    bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundMask);
    }

    bool HasGroundAhead()
    {
        Vector2 lookPoint = (Vector2)transform.position + Vector2.right * edgeLookAhead;
        return Physics2D.Raycast(lookPoint, Vector2.down, edgeCheckDistance, groundMask);
    }

    void OnDrawGizmosSelected()
    {
        Vector2 lookPoint = (Vector2)transform.position + Vector2.right * edgeLookAhead;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.down * groundCheckDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(lookPoint, lookPoint + Vector2.down * edgeCheckDistance);
    }
}
