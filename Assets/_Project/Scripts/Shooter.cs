using TMPro;
using UnityEngine;

[RequireComponent(typeof(LightSensor), typeof(SpriteRenderer), typeof(LineRenderer))]
public class Shooter : MonoBehaviour
{
    enum ShooterState { Idle, Aim, Cooldown, Blinded }

    [SerializeField] ShooterStats stats;
    [SerializeField] Hero hero;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Vector2 lookDirection = Vector2.left;
    [SerializeField] LayerMask wallMask;
    [SerializeField] Color normalColor = new Color(0.878f, 0.353f, 0.278f);
    [SerializeField] Color blindedColor = new Color(0.353f, 0.306f, 0.42f);
    [SerializeField] TMP_Text stateLabel;
    [SerializeField] SpriteStateAnimator spriteAnimator;

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

    public void Init(Hero sceneHero, LanternController sceneLantern)
    {
        hero = sceneHero;
        lightSensor.Init(sceneLantern);
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
                else if (stateTimer >= stats.aimSeconds)
                {
                    Fire();
                    SetState(ShooterState.Cooldown);
                }
                break;

            case ShooterState.Cooldown:
                if (stateTimer >= stats.cooldownSeconds) SetState(ShooterState.Idle);
                break;

            case ShooterState.Blinded:
                if (lit) stateTimer = 0f;
                else if (stateTimer >= stats.blindRecoverSeconds) SetState(ShooterState.Idle);
                break;
        }
    }

    void SetState(ShooterState next)
    {
        state = next;
        stateTimer = 0f;
        aimLaser.enabled = next == ShooterState.Aim;
        spriteRenderer.color = next == ShooterState.Blinded ? blindedColor : normalColor;
        spriteAnimator.Play((int)next);
        if (next == ShooterState.Aim) SfxPlayer.Instance.Play(SfxPlayer.Instance.Bank.shooterAim);
        if (stateLabel != null) stateLabel.text = next.ToString();
    }

    void Fire()
    {
        Vector2 direction = hero.transform.position - transform.position;
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Launch(direction, stats.projectileSpeed, stats.projectileLifetimeSeconds);
        SfxPlayer.Instance.Play(SfxPlayer.Instance.Bank.shooterShot);
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
        if (Vector2.Angle(lookDirection, toHero) > stats.fieldOfViewDegrees / 2f) return false;

        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, toHero.normalized, toHero.magnitude, wallMask);
        bool lineIsClear = wallHit.collider == null;
        Debug.DrawLine(transform.position, hero.transform.position, lineIsClear ? Color.green : Color.red, stats.sightCheckIntervalSeconds);
        return lineIsClear;
    }

    bool IsOnScreen()
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        return viewportPoint.x >= 0f && viewportPoint.x <= 1f && viewportPoint.y >= 0f && viewportPoint.y <= 1f;
    }

    void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Vector2 left = Quaternion.Euler(0f, 0f, stats.fieldOfViewDegrees / 2f) * lookDirection.normalized;
        Vector2 right = Quaternion.Euler(0f, 0f, -stats.fieldOfViewDegrees / 2f) * lookDirection.normalized;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.viewRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, left * stats.viewRange);
        Gizmos.DrawRay(transform.position, right * stats.viewRange);
    }
}
