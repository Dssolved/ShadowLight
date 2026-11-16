using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawn
{
    public GameObject prefab;
    public Vector2 position;
    public Vector2[] waypoints;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "ShadowLight/Level data")]
public class LevelData : ScriptableObject
{
    public string levelName = "Level";
    public GameObject layoutPrefab;
    public Vector2 heroStart = new Vector2(-8f, -1.4f);
    public float parTimeSeconds = 20f;
    public List<EnemySpawn> enemies = new List<EnemySpawn>();
}
