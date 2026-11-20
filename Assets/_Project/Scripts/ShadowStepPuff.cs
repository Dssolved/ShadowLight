using UnityEngine;

public class ShadowStepPuff : MonoBehaviour
{
    [SerializeField] HeroFootsteps footsteps;
    [SerializeField] SpriteRenderer[] puffs;
    [SerializeField] LayerMask shadowMask;
    [SerializeField] float checkDistance = 0.8f;
    [SerializeField] int puffsPerStep = 2;
    [SerializeField] float lifetimeSeconds = 0.8f;
    [SerializeField] float positionJitter = 0.15f;
    [SerializeField] float spreadSpeed = 0.05f;
    [SerializeField] float riseSpeed = 0.45f;
    [SerializeField] float startScale = 0.18f;
    [SerializeField] float endScale = 0.08f;
    [SerializeField] Color puffColor = new Color(0.85f, 0.8f, 1f, 0.9f);

    float[] ages;
    Vector2[] velocities;
    int nextPuff;

    void Awake()
    {
        ages = new float[puffs.Length];
        velocities = new Vector2[puffs.Length];
        for (int i = 0; i < puffs.Length; i++)
        {
            ages[i] = lifetimeSeconds;
            puffs[i].enabled = false;
        }
    }

    void OnEnable()
    {
        footsteps.Stepped += OnStep;
    }

    void OnDisable()
    {
        footsteps.Stepped -= OnStep;
    }

    void OnStep()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, checkDistance, shadowMask);
        if (!hit) return;

        for (int i = 0; i < puffsPerStep; i++)
        {
            SpawnPuff(hit.point);
        }
    }

    void SpawnPuff(Vector2 point)
    {
        int index = nextPuff;
        nextPuff = (nextPuff + 1) % puffs.Length;

        float x = point.x + Random.Range(-positionJitter, positionJitter);
        puffs[index].transform.position = new Vector3(x, point.y, puffs[index].transform.position.z);
        velocities[index] = new Vector2(Random.Range(-spreadSpeed, spreadSpeed), riseSpeed * Random.Range(0.6f, 1f));
        ages[index] = 0f;
        puffs[index].enabled = true;
    }

    void Update()
    {
        for (int i = 0; i < puffs.Length; i++)
        {
            if (ages[i] >= lifetimeSeconds) continue;

            ages[i] += Time.deltaTime;
            float progress = Mathf.Clamp01(ages[i] / lifetimeSeconds);

            puffs[i].transform.position += (Vector3)(velocities[i] * Time.deltaTime);
            puffs[i].transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, progress);
            Color color = puffColor;
            color.a = puffColor.a * (1f - progress);
            puffs[i].color = color;

            if (progress >= 1f) puffs[i].enabled = false;
        }
    }
}
