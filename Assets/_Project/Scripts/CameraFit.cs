using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    [SerializeField] float levelWidth = 17.8f;
    [SerializeField] float minOrthographicSize = 5f;

    Camera fittedCamera;
    float fittedAspect;

    void Awake()
    {
        fittedCamera = GetComponent<Camera>();
        Fit();
    }

    void Update()
    {
        if (fittedCamera.aspect != fittedAspect) Fit();
    }

    void Fit()
    {
        fittedAspect = fittedCamera.aspect;
        float sizeForFullWidth = levelWidth / 2f / fittedAspect;
        fittedCamera.orthographicSize = Mathf.Max(minOrthographicSize, sizeForFullWidth);
    }
}
