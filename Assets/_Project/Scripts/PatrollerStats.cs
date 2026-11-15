using UnityEngine;

[CreateAssetMenu(fileName = "PatrollerStats", menuName = "ShadowLight/Patroller stats")]
public class PatrollerStats : ScriptableObject
{
    public float patrolSpeed = 1.2f;
    public float chaseSpeed = 2f;
    public float fleeSpeed = 2.5f;
    public float viewRange = 4f;
    public float fieldOfViewDegrees = 120f;
    public float sightCheckIntervalSeconds = 0.15f;
    public float loseSightSeconds = 2f;
    public float attackStartRange = 1f;
    public float strikeRange = 1.3f;
    public float windUpSeconds = 0.5f;
    public float recoverSeconds = 1.5f;
    public float minFleeSeconds = 1f;
    public float waypointReachDistance = 0.05f;
}
