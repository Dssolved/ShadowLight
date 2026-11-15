using UnityEngine;

[CreateAssetMenu(fileName = "ShooterStats", menuName = "ShadowLight/Shooter stats")]
public class ShooterStats : ScriptableObject
{
    public float viewRange = 7f;
    public float fieldOfViewDegrees = 90f;
    public float sightCheckIntervalSeconds = 0.15f;
    public float aimSeconds = 0.8f;
    public float cooldownSeconds = 1.5f;
    public float blindRecoverSeconds = 1f;
    public float projectileSpeed = 5f;
    public float projectileLifetimeSeconds = 4f;
}
