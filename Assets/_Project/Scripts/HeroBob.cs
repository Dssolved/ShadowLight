using UnityEngine;

public class HeroBob : MonoBehaviour
{
    [SerializeField] HeroFootsteps footsteps;
    [SerializeField] Transform bodyTransform;
    [SerializeField] float bobHeight = 0.05f;
    [SerializeField] float tiltDegrees = 3f;
    [SerializeField] float tiltDegreesPerSecond = 40f;

    float timeSinceStep;
    float tilt;
    float baseHeight;

    void Awake()
    {
        baseHeight = bodyTransform.localPosition.y;
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
        timeSinceStep = 0f;
    }

    void Update()
    {
        timeSinceStep += Time.deltaTime;
        float progress = Mathf.Clamp01(timeSinceStep / footsteps.StepIntervalSeconds);
        float bob = footsteps.IsWalking ? bobHeight * Mathf.Sin(Mathf.PI * progress) : 0f;
        bodyTransform.localPosition = new Vector3(bodyTransform.localPosition.x, baseHeight + bob, bodyTransform.localPosition.z);

        float targetTilt = footsteps.IsWalking ? -tiltDegrees : 0f;
        tilt = Mathf.MoveTowards(tilt, targetTilt, tiltDegreesPerSecond * Time.deltaTime);
        bodyTransform.localRotation = Quaternion.Euler(0f, 0f, tilt);
    }
}
