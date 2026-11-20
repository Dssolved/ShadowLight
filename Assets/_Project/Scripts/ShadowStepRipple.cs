using UnityEngine;

public class ShadowStepRipple : MonoBehaviour
{
    [SerializeField] HeroFootsteps footsteps;
    [SerializeField] Hero hero;
    [SerializeField] SpriteRenderer[] rings;
    [SerializeField] LayerMask shadowMask;
    [SerializeField] float checkDistance = 0.8f;
    [SerializeField] float lifetimeSeconds = 0.6f;
    [SerializeField] float startScale = 0.25f;
    [SerializeField] float endScale = 1.3f;
    [SerializeField] Color ringColor = new Color(0.75f, 0.68f, 0.95f, 0.8f);

    float[] ages;
    int nextRing;

    void Awake()
    {
        ages = new float[rings.Length];
        for (int i = 0; i < rings.Length; i++)
        {
            ages[i] = lifetimeSeconds;
            rings[i].enabled = false;
        }
    }

    void OnEnable()
    {
        footsteps.Stepped += OnStep;
        hero.Landed += OnStep;
    }

    void OnDisable()
    {
        footsteps.Stepped -= OnStep;
        hero.Landed -= OnStep;
    }

    void OnStep()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, checkDistance, shadowMask);
        if (!hit) return;

        Transform ring = rings[nextRing].transform;
        ring.position = new Vector3(hit.point.x, hit.point.y, ring.position.z);
        ages[nextRing] = 0f;
        rings[nextRing].enabled = true;
        nextRing = (nextRing + 1) % rings.Length;
    }

    void Update()
    {
        for (int i = 0; i < rings.Length; i++)
        {
            if (ages[i] >= lifetimeSeconds) continue;

            ages[i] += Time.deltaTime;
            float progress = Mathf.Clamp01(ages[i] / lifetimeSeconds);

            rings[i].transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, progress);
            Color color = ringColor;
            color.a = ringColor.a * (1f - progress);
            rings[i].color = color;

            if (progress >= 1f) rings[i].enabled = false;
        }
    }
}
