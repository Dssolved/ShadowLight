using UnityEngine;

[CreateAssetMenu(fileName = "LevelList", menuName = "ShadowLight/Level list")]
public class LevelList : ScriptableObject
{
    public LevelData[] levels;
}
