using UnityEngine;
using UnityEngine.UI;

public class LanternController : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Transform railStart;
    [SerializeField] Transform railEnd;
    [SerializeField] float depthFromWall = 10f;
    [SerializeField] float lightRadius = 7f;

    public float DepthFromWall => depthFromWall;
    public float LightRadius => lightRadius;

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
