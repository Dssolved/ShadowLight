using UnityEngine;

public class UiSway : MonoBehaviour
{
    [SerializeField] float swayDegrees = 0.8f;
    [SerializeField] float periodSeconds = 6f;
    [SerializeField] float phaseRadians;

    void Update()
    {
        float angle = Mathf.Sin(Time.unscaledTime / periodSeconds * Mathf.PI * 2f + phaseRadians) * swayDegrees;
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
