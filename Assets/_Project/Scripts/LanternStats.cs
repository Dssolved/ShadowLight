using UnityEngine;

[CreateAssetMenu(fileName = "LanternStats", menuName = "ShadowLight/Lantern stats")]
public class LanternStats : ScriptableObject
{
    public float depthFromWall = 10f;
    public float lightRadius = 7f;
}
