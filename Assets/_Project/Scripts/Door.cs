using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] string nextSceneName = "MainMenu";
    [SerializeField] float loadDelaySeconds = 1f;

    bool opened;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (opened) return;

        if (other.TryGetComponent(out Hero hero))
        {
            opened = true;
            hero.EnterDoor();
            StartCoroutine(LoadNextScene());
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(loadDelaySeconds);
        SceneManager.LoadScene(nextSceneName);
    }
}
