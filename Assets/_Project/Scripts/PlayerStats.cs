using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ShadowLight/Player stats")]
public class PlayerStats : ScriptableObject
{
    public int maxLives = 3;
    public float walkSpeed = 1.5f;
    public float groundCheckDistance = 0.6f;
    public float edgeLookAhead = 0.4f;
    public float edgeCheckDistance = 0.9f;
    public float fallDeathHeight = -6f;
    public float respawnDelaySeconds = 1f;
}
