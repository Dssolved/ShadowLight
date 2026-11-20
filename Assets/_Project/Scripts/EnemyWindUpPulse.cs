using UnityEngine;

public class EnemyWindUpPulse : MonoBehaviour
{
    [SerializeField] float pulseX = 0.1f;
    [SerializeField] float pulseY = 0.1f;
    [SerializeField] float pulsesPerSecond = 3f;
    [SerializeField] float returnSpeed = 2f;

    IWindUpSource source;
    Vector3 baseScale;
    float phase;

    void Awake()
    {
        source = GetComponent<IWindUpSource>();
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (!source.IsWindingUp)
        {
            phase = 0f;
            transform.localScale = Vector3.MoveTowards(transform.localScale, baseScale, returnSpeed * Time.deltaTime);
            return;
        }

        phase += Time.deltaTime * pulsesPerSecond * 2f * Mathf.PI;
        float wave = 0.5f - 0.5f * Mathf.Cos(phase);
        transform.localScale = new Vector3(baseScale.x * (1f + pulseX * wave), baseScale.y * (1f + pulseY * wave), baseScale.z);
    }
}
