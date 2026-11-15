using TMPro;
using UnityEngine;

[RequireComponent(typeof(LightSensor), typeof(SpriteRenderer))]
public class Patroller : MonoBehaviour
{
    enum PatrollerState { Patrol, Chase, WindUp, Recover, Flee }

    [SerializeField] PatrollerStats stats;
    [SerializeField] Hero hero;
    [SerializeField] Transform[] waypoints;
    [SerializeField] LayerMask wallMask;
    [SerializeField] Color normalColor = new Color(0.608f, 0.42f, 0.839f);
    [SerializeField] Color windUpColor = new Color(1f, 0.29f, 0.24f);
    [SerializeField] TMP_Text stateLabel;
    [SerializeField] SpriteStateAnimator spriteAnimator;

    LightSensor lightSensor;
    SpriteRenderer spriteRenderer;
    Camera mainCamera;
    PatrollerState state;
    float stateTimer;
    float sightTimer;
    bool canSeeHero;
    int waypointIndex;
    Vector2 facing = Vector2.right;

    void Awake()
    {
        lightSensor = GetComponent<LightSensor>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        SetState(PatrollerState.Patrol);
    }

    void Update()
    {
        stateTimer += Time.deltaTime;
        UpdateSight();
        bool lit = lightSensor.IsLit();

        switch (state)
        {
            case PatrollerState.Patrol:
                Patrol();
                if (lit) SetState(PatrollerState.Flee);
                else if (canSeeHero) SetState(PatrollerState.Chase);
                break;

            case PatrollerState.Chase:
                if (canSeeHero) stateTimer = 0f;
                MoveTo(hero.transform.position, stats.chaseSpeed);
                if (lit) SetState(PatrollerState.Flee);
                else if (DistanceToHero() <= stats.attackStartRange && IsOnScreen()) SetState(PatrollerState.WindUp);
                else if (stateTimer >= stats.loseSightSeconds) SetState(PatrollerState.Patrol);
                break;

            case PatrollerState.WindUp:
                if (lit) SetState(PatrollerState.Flee);
                else if (stateTimer >= stats.windUpSeconds)
                {
                    if (DistanceToHero() <= stats.strikeRange) hero.Die();
                    SetState(PatrollerState.Recover);
                }
                break;

            case PatrollerState.Recover:
                if (stateTimer >= stats.recoverSeconds) SetState(PatrollerState.Patrol);
                break;

            case PatrollerState.Flee:
                MoveTo(FarthestWaypointFromLantern(), stats.fleeSpeed);
                if (!lit && stateTimer >= stats.minFleeSeconds) SetState(PatrollerState.Patrol);
                break;
        }
    }

    void SetState(PatrollerState next)
    {
        state = next;
        stateTimer = 0f;
        spriteRenderer.color = next == PatrollerState.WindUp ? windUpColor : normalColor;
        spriteAnimator.Play((int)next);
        if (stateLabel != null) stateLabel.text = next.ToString();
    }

    void Patrol()
    {
        Vector2 target = waypoints[waypointIndex].position;
        MoveTo(target, stats.patrolSpeed);
        if (Vector2.Distance(transform.position, target) <= stats.waypointReachDistance)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
        }
    }

    void MoveTo(Vector2 target, float speed)
    {
        Vector2 direction = target - (Vector2)transform.position;
        if (direction.sqrMagnitude > 0f)
        {
            facing = direction.normalized;
            spriteRenderer.flipX = facing.x < 0f;
        }
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    Vector2 FarthestWaypointFromLantern()
    {
        Vector2 lanternPosition = lightSensor.LanternPosition;
        Vector2 farthest = waypoints[0].position;
        foreach (Transform waypoint in waypoints)
        {
            if (Mathf.Abs(waypoint.position.x - lanternPosition.x) > Mathf.Abs(farthest.x - lanternPosition.x))
            {
                farthest = waypoint.position;
            }
        }
        return farthest;
    }

    void UpdateSight()
    {
        sightTimer -= Time.deltaTime;
        if (sightTimer > 0f) return;

        sightTimer = stats.sightCheckIntervalSeconds;
        canSeeHero = CanSeeHero();
    }

    bool CanSeeHero()
    {
        Vector2 toHero = hero.transform.position - transform.position;
        if (toHero.magnitude > stats.viewRange) return false;
        if (Vector2.Angle(facing, toHero) > stats.fieldOfViewDegrees / 2f) return false;

        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, toHero.normalized, toHero.magnitude, wallMask);
        return wallHit.collider == null;
    }

    float DistanceToHero()
    {
        return Vector2.Distance(transform.position, hero.transform.position);
    }

    bool IsOnScreen()
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        return viewportPoint.x >= 0f && viewportPoint.x <= 1f && viewportPoint.y >= 0f && viewportPoint.y <= 1f;
    }

    void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.viewRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackStartRange);
    }
}
