using TMPro;
using UnityEngine;

[RequireComponent(typeof(LightSensor), typeof(SpriteRenderer), typeof(LineRenderer))]
public class Charger : MonoBehaviour
{
    enum ChargerState { Dormant, WindUp, Dash, Stunned, Return }

    [SerializeField] ChargerStats stats;
    [SerializeField] Hero hero;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Color normalColor = new Color(0.769f, 0.416f, 0.243f);
    [SerializeField] Color windUpColor = new Color(1f, 0.29f, 0.24f);
    [SerializeField] Color stunnedColor = new Color(0.353f, 0.306f, 0.42f);
    [SerializeField] TMP_Text stateLabel;
    [SerializeField] SpriteStateAnimator spriteAnimator;

    LightSensor lightSensor;
    SpriteRenderer spriteRenderer;
    LineRenderer dashLane;
    Camera mainCamera;
    ChargerState state;
    float stateTimer;
    Vector2 dashDirection;
    Vector2 dashTarget;
    Vector2 homePosition;

    void Awake()
    {
        lightSensor = GetComponent<LightSensor>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        dashLane = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
        homePosition = transform.position;
        SetState(ChargerState.Dormant);
    }

    void Update()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case ChargerState.Dormant:
                if (!lightSensor.IsLit() && HeroInRange() && IsOnScreen())
                {
                    AimDash();
                    SetState(ChargerState.WindUp);
                }
                break;

            case ChargerState.WindUp:
                if (lightSensor.IsLit()) SetState(ChargerState.Dormant);
                else if (stateTimer >= stats.windUpSeconds) SetState(ChargerState.Dash);
                break;

            case ChargerState.Dash:
                transform.position = Vector2.MoveTowards(transform.position, dashTarget, stats.dashSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, hero.transform.position) <= stats.hitRadius) hero.Die();
                bool reachedTarget = (Vector2)transform.position == dashTarget;
                if (reachedTarget || !HasGroundAhead()) SetState(ChargerState.Stunned);
                break;

            case ChargerState.Stunned:
                if (stateTimer >= stats.stunSeconds) SetState(ChargerState.Return);
                break;

            case ChargerState.Return:
                transform.position = Vector2.MoveTowards(transform.position, homePosition, stats.returnSpeed * Time.deltaTime);
                if ((Vector2)transform.position == homePosition) SetState(ChargerState.Dormant);
                break;
        }
    }

    void SetState(ChargerState next)
    {
        state = next;
        stateTimer = 0f;
        dashLane.enabled = next == ChargerState.WindUp;
        spriteRenderer.color = ColorFor(next);
        if (next == ChargerState.Return) spriteRenderer.flipX = homePosition.x < transform.position.x;
        spriteAnimator.Play((int)next);
        if (stateLabel != null) stateLabel.text = next.ToString();
    }

    Color ColorFor(ChargerState chargerState)
    {
        if (chargerState == ChargerState.WindUp) return windUpColor;
        if (chargerState == ChargerState.Stunned) return stunnedColor;
        return normalColor;
    }

    void AimDash()
    {
        float side = Mathf.Sign(hero.transform.position.x - transform.position.x);
        dashDirection = new Vector2(side, 0f);
        spriteRenderer.flipX = side < 0f;
        dashTarget = (Vector2)transform.position + dashDirection * stats.dashDistance;
        dashLane.SetPosition(0, transform.position);
        dashLane.SetPosition(1, dashTarget);
    }

    bool HeroInRange()
    {
        return Vector2.Distance(transform.position, hero.transform.position) <= stats.triggerRange;
    }

    bool HasGroundAhead()
    {
        Vector2 lookPoint = (Vector2)transform.position + dashDirection * stats.edgeLookAhead;
        return Physics2D.Raycast(lookPoint, Vector2.down, stats.edgeCheckDistance, groundMask);
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
        Gizmos.DrawWireSphere(transform.position, stats.triggerRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.hitRadius);
    }
}
