using UnityEngine;

public class HeroSquash : MonoBehaviour
{
    [SerializeField] Hero hero;
    [SerializeField] Transform bodyTransform;
    [SerializeField] float squashSeconds = 0.18f;
    [SerializeField] float squashY = 0.18f;
    [SerializeField] float stretchX = 0.12f;

    Vector3 baseScale;
    float timer;

    void Awake()
    {
        baseScale = bodyTransform.localScale;
        timer = squashSeconds;
    }

    void OnEnable()
    {
        hero.Landed += OnLanded;
    }

    void OnDisable()
    {
        hero.Landed -= OnLanded;
    }

    void OnLanded()
    {
        timer = 0f;
    }

    void Update()
    {
        if (timer >= squashSeconds) return;

        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / squashSeconds);
        float amount = Mathf.Sin(Mathf.PI * progress);

        bodyTransform.localScale = new Vector3(baseScale.x * (1f + stretchX * amount), baseScale.y * (1f - squashY * amount), baseScale.z);
    }
}
