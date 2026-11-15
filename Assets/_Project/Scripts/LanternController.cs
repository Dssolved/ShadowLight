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
    }

    void OnEnable()
    {
        slider.onValueChanged.AddListener(MoveAlongRail);
        MoveAlongRail(slider.value);
    }

    void OnDisable()
    {
        slider.onValueChanged.RemoveListener(MoveAlongRail);
    }

    void MoveAlongRail(float railProgress)
    {
        transform.position = Vector3.Lerp(railStart.position, railEnd.position, railProgress);
    }
}
