using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] LevelList levelList;
    [SerializeField] Hero hero;
    [SerializeField] LanternController lantern;
    [SerializeField] Lives lives;

    public LevelData CurrentLevel { get; private set; }

    void Start()
    {
        int index = Mathf.Clamp(GameSession.levelIndex, 0, levelList.levels.Length - 1);
        CurrentLevel = levelList.levels[index];

        hero.transform.position = CurrentLevel.heroStart;
        hero.SetRespawnPoint(CurrentLevel.heroStart);

        BuildLayout(index);
        SpawnEnemies();
    }

    void BuildLayout(int index)
    {
        GameObject layout = Instantiate(CurrentLevel.layoutPrefab);

        foreach (BlockShadow shadow in layout.GetComponentsInChildren<BlockShadow>())
        {
            shadow.Init(lantern);
        }

        layout.GetComponentInChildren<Door>().Init(lives, index);
    }

    void SpawnEnemies()
    {
        foreach (EnemySpawn spawn in CurrentLevel.enemies)
        {
            GameObject enemy = Instantiate(spawn.prefab, spawn.position, Quaternion.identity);

            if (enemy.TryGetComponent(out Patroller patroller)) patroller.Init(hero, lantern, spawn.waypoints);
            if (enemy.TryGetComponent(out Shooter shooter)) shooter.Init(hero, lantern);
            if (enemy.TryGetComponent(out Charger charger)) charger.Init(hero, lantern);
        }
    }
}
