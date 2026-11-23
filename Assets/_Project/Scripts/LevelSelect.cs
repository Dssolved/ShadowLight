using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] LevelList levelList;
    [SerializeField] Button[] levelButtons;
    [SerializeField] TMP_Text[] levelLabels;
    [SerializeField] GameObject[] lockIcons;
    [SerializeField] string gameSceneName = "Game";

    void OnEnable()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool unlocked = SaveSystem.Data.IsUnlocked(i);
            levelButtons[i].interactable = unlocked;
            levelLabels[i].text = unlocked ? LabelFor(i) : "LOCKED";
            lockIcons[i].SetActive(!unlocked);
        }
    }

    string LabelFor(int index)
    {
        LevelData level = levelList.levels[index];
        LevelRecord record = SaveSystem.Data.FindRecord(index);
        string best = record == null ? "-" : record.bestTimeSeconds.ToString("0.0") + " S";
        return (index + 1) + ". " + level.levelName.ToUpper() + "   BEST " + best;
    }

    public void PlayLevel(int index)
    {
        GameSession.levelIndex = index;
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }
}
