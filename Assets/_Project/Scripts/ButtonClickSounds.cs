using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSounds : MonoBehaviour
{
    void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayClick);
        }
    }

    void PlayClick()
    {
        SfxPlayer.Instance.Play(SfxPlayer.Instance.Bank.uiClick);
    }
}
