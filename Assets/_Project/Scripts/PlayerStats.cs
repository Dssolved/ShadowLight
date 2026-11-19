using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ShadowLight/Player stats")]
public class PlayerStats : ScriptableObject
{
    public int maxLives = 10;
    public int startLives = 6;
    public int minLivesAtLevelStart = 3;
    public int healPerBrazier = 1;
    public float walkSpeed = 1.5f;
    public float groundCheckDistance = 0.6f;
    public float edgeLookAhead = 0.4f;
    public float edgeCheckDistance = 0.9f;
    public float fallDeathHeight = -6f;
    public float respawnDelaySeconds = 1f;
    public float emergeSeconds = 1f;
}
