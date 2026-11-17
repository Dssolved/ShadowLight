using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] LevelList levelList;
    [SerializeField] Light2D globalLight;
    [SerializeField] Hero hero;
    [SerializeField] LanternController lantern;
    [SerializeField] Lives lives;
    [SerializeField] StateLabelSwitch stateLabels;
    [SerializeField] TMP_Text levelText;

    public LevelData CurrentLevel { get; private set; }

    void Start()
    {
        int index = Mathf.Clamp(GameSession.levelIndex, 0, levelList.levels.Length - 1);
        CurrentLevel = levelList.levels[index];
        levelText.text = "LEVEL " + (index + 1) + ": " + CurrentLevel.levelName.ToUpper() + "   GOAL " + CurrentLevel.parTimeSeconds.ToString("0") + " S";

        hero.transform.position = CurrentLevel.heroStart;
        hero.SetRespawnPoint(CurrentLevel.heroStart);

        BuildBackground();
        BuildLayout(index);
        SpawnEnemies();
    }

    void BuildBackground()
    {
        globalLight.color = CurrentLevel.ambientColor;
        globalLight.intensity = CurrentLevel.ambientIntensity;

        if (CurrentLevel.backgroundPrefab != null)
        {
            Instantiate(CurrentLevel.backgroundPrefab);
        }
    }

    void BuildLayout(int index)
    {
        GameObject layout = Instantiate(CurrentLevel.layoutPrefab);

        foreach (BlockShadow shadow in layout.GetComponentsInChildren<BlockShadow>())
        {
            shadow.Init(lantern);
        }

        bool hasNextLevel = index + 1 < levelList.levels.Length;
        layout.GetComponentInChildren<Door>().Init(lives, index, hasNextLevel);
    }

    void SpawnEnemies()
    {
        foreach (EnemySpawn spawn in CurrentLevel.enemies)
        {
            GameObject enemy = Instantiate(spawn.prefab, spawn.position, Quaternion.identity);
            stateLabels.Apply(enemy);

            if (enemy.TryGetComponent(out Patroller patroller)) patroller.Init(hero, lantern, spawn.waypoints);
            if (enemy.TryGetComponent(out Shooter shooter)) shooter.Init(hero, lantern);
            if (enemy.TryGetComponent(out Charger charger)) charger.Init(hero, lantern);
        }
    }
}
