using UnityEngine;

[CreateAssetMenu(fileName = "ChargerStats", menuName = "ShadowLight/Charger stats")]
public class ChargerStats : ScriptableObject
{
    public float triggerRange = 3f;
    public float windUpSeconds = 0.8f;
    public float dashSpeed = 8f;
    public float dashDistance = 5f;
    public float hitRadius = 0.6f;
    public float stunSeconds = 1f;
    public float returnSpeed = 2f;
    public float edgeLookAhead = 0.5f;
    public float edgeCheckDistance = 1f;
}
