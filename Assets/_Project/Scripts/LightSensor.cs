using UnityEngine;

public class LightSensor : MonoBehaviour
{
    [SerializeField] LanternController lantern;
    [SerializeField] LayerMask shadowMask;

    public Vector2 LanternPosition => lantern.transform.position;

    public void Init(LanternController sceneLantern)
    {
        lantern = sceneLantern;
    }

    public bool IsLit()
    {
        float distanceToLantern = Vector2.Distance(transform.position, lantern.transform.position);
        bool insideLight = distanceToLantern <= lantern.LightRadius;
        bool insideShadow = Physics2D.OverlapPoint(transform.position, shadowMask) != null;
        return insideLight && !insideShadow;
    }
}
