using UnityEngine;

public class HeroFadeIn : MonoBehaviour
{
    [SerializeField] SpriteRenderer bodySprite;
    [SerializeField] float fadeSeconds = 0.4f;

    float timer;

    void Awake()
    {
        SetAlpha(0f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        SetAlpha(Mathf.Clamp01(timer / fadeSeconds));

        if (timer >= fadeSeconds) enabled = false;
    }

    void SetAlpha(float alpha)
    {
        Color color = bodySprite.color;
        color.a = alpha;
        bodySprite.color = color;
    }
}
