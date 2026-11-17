using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BrazierGlow : MonoBehaviour
{
    [SerializeField] SpriteRenderer brazierSprite;
    [SerializeField] Sprite coldSprite;
    [SerializeField] Sprite litSprite;
    [SerializeField] Light2D flameLight;
    [SerializeField] float litIntensity = 1.8f;
    [SerializeField] float popIntensity = 3.2f;
    [SerializeField] float popSeconds = 0.4f;
    [SerializeField] float litRadius = 4.5f;
    [SerializeField] float flickerSpeed = 6f;
    [SerializeField] float flickerAmount = 0.2f;

    bool isLit;
    float litTimer;
    float noiseOffset;

    void Awake()
    {
        brazierSprite.sprite = coldSprite;
        flameLight.enabled = false;
        noiseOffset = Random.value * 100f;
    }

    public void Ignite()
    {
        if (isLit) return;

        isLit = true;
        litTimer = 0f;
        brazierSprite.sprite = litSprite;
        flameLight.enabled = true;
    }

    void Update()
    {
        if (!isLit) return;

        litTimer += Time.deltaTime;
        float noise = Mathf.PerlinNoise(noiseOffset, Time.time * flickerSpeed) * 2f - 1f;
        float baseIntensity = Mathf.Lerp(popIntensity, litIntensity, litTimer / popSeconds);

        flameLight.intensity = baseIntensity * (1f + noise * flickerAmount);
        flameLight.pointLightOuterRadius = litRadius * (1f + noise * flickerAmount * 0.5f);
    }
}
