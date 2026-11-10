using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lives : MonoBehaviour
{
    [SerializeField] int maxLives = 3;

    public event Action<int> Changed;

    public int Current { get; private set; }
    public bool HasLivesLeft => Current > 0;

    void Awake()
    {
        Current = maxLives;
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
