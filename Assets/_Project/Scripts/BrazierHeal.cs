using UnityEngine;

public class BrazierHeal : MonoBehaviour
{
    const int KeysPerLevel = 10;

    [SerializeField] BrazierGlow glow;
    [SerializeField] PlayerStats stats;

    Lives lives;
    int levelIndex;
    int brazierIndex;

    public void Init(Lives sceneLives, int level, int index)
    {
        lives = sceneLives;
        levelIndex = level;
        brazierIndex = index;
    }

    void OnEnable()
    {
        glow.Lit += OnLit;
    }

    void OnDisable()
    {
        glow.Lit -= OnLit;
    }

    void OnLit()
    {
        if (SaveSystem.Data.IsCompleted(levelIndex)) return;

        int key = levelIndex * KeysPerLevel + brazierIndex;
        if (!GameSession.healedBraziers.Add(key)) return;

        lives.Heal(stats.healPerBrazier);
    }
}
