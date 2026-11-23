using UnityEngine;
using UnityEngine.UI;

public class UiFlicker : MonoBehaviour
{
    [SerializeField] Graphic target;
    [SerializeField] float baseAlpha = 0.35f;
    [SerializeField] float alphaVariation = 0.12f;
    [SerializeField] float baseScale = 1f;
    [SerializeField] float scaleVariation = 0.06f;
    [SerializeField] float speed = 2.2f;

    float seed;

    void Awake()
    {
        seed = Random.value * 100f;
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(seed, Time.unscaledTime * speed) * 2f - 1f;
        Color color = target.color;
        color.a = baseAlpha + noise * alphaVariation;
        target.color = color;
        transform.localScale = Vector3.one * (baseScale + noise * scaleVariation);
    }
}
