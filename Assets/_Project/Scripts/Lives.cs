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
        Current = stats.maxLives;
    }

    public void LoseLife()
    {
        if (Current <= 0) return;

        Current--;
        Changed?.Invoke(Current);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
