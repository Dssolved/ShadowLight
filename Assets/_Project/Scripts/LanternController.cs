using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class LanternController : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Transform railStart;
    [SerializeField] Transform railEnd;
    [SerializeField] Light2D lanternLight;
    [SerializeField] LanternStats stats;

    public float DepthFromWall => stats.depthFromWall;
    public float LightRadius => stats.lightRadius;

    void Awake()
    {
        lanternLight.pointLightOuterRadius = stats.lightRadius;
        transform.position = RailPoint(slider.value);
    }

    void Update()
    {
        Vector3 target = RailPoint(slider.value);
        transform.position = Vector3.MoveTowards(transform.position, target, stats.moveSpeed * Time.deltaTime);
    }

    Vector3 RailPoint(float railProgress)
    {
        return Vector3.Lerp(railStart.position, railEnd.position, railProgress);
    }
}
