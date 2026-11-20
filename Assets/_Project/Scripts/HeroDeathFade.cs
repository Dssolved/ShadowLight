using UnityEngine;

public class HeroDeathFade : MonoBehaviour
{
    enum FadeState { Idle, FadeOut, FadeIn }

    [SerializeField] Hero hero;
    [SerializeField] SpriteRenderer bodySprite;
    [SerializeField] Color flashColor = new Color(1f, 0.55f, 0.55f, 1f);
    [SerializeField] float flashSeconds = 0.1f;
    [SerializeField] float fadeOutSeconds = 0.4f;
    [SerializeField] float fadeInSeconds = 0.3f;

    FadeState state;
    float stateTimer;

    void OnEnable()
    {
        hero.Died += OnDied;
        hero.Respawned += OnRespawned;
    }

    void OnDisable()
    {
        hero.Died -= OnDied;
        hero.Respawned -= OnRespawned;
    }

    void OnDied()
    {
        SetState(FadeState.FadeOut);
    }

    void OnRespawned()
    {
        SetState(FadeState.FadeIn);
    }

    void SetState(FadeState next)
    {
        state = next;
        stateTimer = 0f;
    }

    void Update()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case FadeState.FadeOut:
                if (stateTimer < flashSeconds) bodySprite.color = flashColor;
                else bodySprite.color = new Color(1f, 1f, 1f, 1f - Mathf.Clamp01((stateTimer - flashSeconds) / fadeOutSeconds));
                break;

            case FadeState.FadeIn:
                bodySprite.color = new Color(1f, 1f, 1f, Mathf.Clamp01(stateTimer / fadeInSeconds));
                if (stateTimer >= fadeInSeconds) SetState(FadeState.Idle);
                break;
        }
    }
}
