using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] string menuSceneName = "MainMenu";
    [SerializeField] string gameSceneName = "Game";
    [SerializeField] float loadDelaySeconds = 1f;
    [SerializeField] int levelIndex;
    [SerializeField] Lives lives;

    bool opened;
    bool hasNextLevel;

    public void Init(Lives sceneLives, int index, bool nextLevelExists)
    {
        lives = sceneLives;
        levelIndex = index;
        hasNextLevel = nextLevelExists;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (opened) return;

        if (other.TryGetComponent(out Hero hero))
        {
            opened = true;
            hero.EnterDoor();
            SfxPlayer.Instance.PlayJingle(SfxPlayer.Instance.Bank.win);
            SaveSystem.Data.CompleteLevel(levelIndex, Time.timeSinceLevelLoad, lives.Current);
            SaveSystem.Save();
            StartCoroutine(LoadNextScene());
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(loadDelaySeconds);

        if (hasNextLevel)
        {
            GameSession.levelIndex = levelIndex + 1;
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            SceneManager.LoadScene(menuSceneName);
        }
    }
}
