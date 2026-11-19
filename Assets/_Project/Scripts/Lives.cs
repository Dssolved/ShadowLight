using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lives : MonoBehaviour
{
    [SerializeField] PlayerStats stats;

    public event Action<int> Changed;

    public int Current { get; private set; }
    public bool HasLivesLeft => Current > 0;

    void Awake()
    {
        int saved = SaveSystem.Data.flames;
        if (saved < 0) saved = stats.startLives;

        Current = Mathf.Clamp(Mathf.Max(saved, stats.minLivesAtLevelStart), 0, stats.maxLives);
        SaveSystem.Data.flames = Current;
    }

    public void LoseLife()
    {
        if (Current <= 0) return;

        Current--;
        SaveSystem.Data.flames = Current;
        Changed?.Invoke(Current);
    }

    public void Heal(int amount)
    {
        Current = Mathf.Min(Current + amount, stats.maxLives);
        SaveSystem.Data.flames = Current;
        Changed?.Invoke(Current);
    }

    public void RestartLevel()
    {
        SfxPlayer.Instance.Play(SfxPlayer.Instance.Bank.lose);
        SaveSystem.Data.flames = stats.minLivesAtLevelStart;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
