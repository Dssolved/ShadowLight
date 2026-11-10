using TMPro;
using UnityEngine;

[RequireComponent(typeof(LightSensor), typeof(SpriteRenderer), typeof(LineRenderer))]
public class Shooter : MonoBehaviour
{
    enum ShooterState { Idle, Aim, Cooldown, Blinded }

    [SerializeField] Hero hero;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Vector2 lookDirection = Vector2.left;
    [SerializeField] float viewRange = 7f;
    [SerializeField] float fieldOfViewDegrees = 90f;
    [SerializeField] float sightCheckIntervalSeconds = 0.15f;
    [SerializeField] LayerMask wallMask;
    [SerializeField] float aimSeconds = 0.6f;
    [SerializeField] float cooldownSeconds = 1.5f;
    [SerializeField] float blindRecoverSeconds = 1f;
    [SerializeField] Color normalColor = new Color(0.878f, 0.353f, 0.278f);
    [SerializeField] Color blindedColor = new Color(0.353f, 0.306f, 0.42f);
    [SerializeField] TMP_Text stateLabel;

    LightSensor lightSensor;
    SpriteRenderer spriteRenderer;
    LineRenderer aimLaser;
    Camera mainCamera;
    ShooterState state;
    float stateTimer;
    float sightTimer;
    bool canSeeHero;

    void Awake()
    {
        lightSensor = GetComponent<LightSensor>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        aimLaser = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
        SetState(ShooterState.Idle);
    }

    void Update()
    {
        stateTimer += Time.deltaTime;
        UpdateSight();
        bool lit = lightSensor.IsLit();

        switch (state)
        {
            case ShooterState.Idle:
                if (lit) SetState(ShooterState.Blinded);
                else if (canSeeHero && IsOnScreen()) SetState(ShooterState.Aim);
                break;

            case ShooterState.Aim:
                aimLaser.SetPosition(0, transform.position);
                aimLaser.SetPosition(1, hero.transform.position);
                if (lit) SetState(ShooterState.Blinded);
                else if (!canSeeHero) SetState(ShooterState.Idle);
                else if (stateTimer >= aimSeconds)
                {
                    Fire();
                    SetState(ShooterState.Cooldown);
                }
                break;

            case ShooterState.Cooldown:
                if (stateTimer >= cooldownSeconds) SetState(ShooterState.Idle);
                break;

            case ShooterState.Blinded:
                if (lit) stateTimer = 0f;
                else if (stateTimer >= blindRecoverSeconds) SetState(ShooterState.Idle);
                break;
        }
    }

    void SetState(ShooterState next)
    {
        state = next;
        stateTimer = 0f;
        aimLaser.enabled = next == ShooterState.Aim;
        spriteRenderer.color = next == ShooterState.Blinded ? blindedColor : normalColor;
        if (stateLabel != null) stateLabel.text = next.ToString();
    }

    void Fire()
    {
        Vector2 direction = hero.transform.position - transform.position;
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Launch(direction);
    }

    void UpdateSight()
    {
        sightTimer -= Time.deltaTime;
        if (sightTimer > 0f) return;

        sightTimer = sightCheckIntervalSeconds;
        canSeeHero = CanSeeHero();
    }

    bool CanSeeHero()
    {
        Vector2 toHero = hero.transform.position - transform.position;
        if (toHero.magnitude > viewRange) return false;
        if (Vector2.Angle(lookDirection, toHero) > fieldOfViewDegrees / 2f) return false;

        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, toHero.normalized, toHero.magnitude, wallMask);
        return wallHit.collider == null;
    }

    bool IsOnScreen()
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        return viewportPoint.x >= 0f && viewportPoint.x <= 1f && viewportPoint.y >= 0f && viewportPoint.y <= 1f;
    }

    void OnDrawGizmosSelected()
    {
        Vector2 left = Quaternion.Euler(0f, 0f, fieldOfViewDegrees / 2f) * lookDirection.normalized;
        Vector2 right = Quaternion.Euler(0f, 0f, -fieldOfViewDegrees / 2f) * lookDirection.normalized;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, left * viewRange);
        Gizmos.DrawRay(transform.position, right * viewRange);
    }
}
