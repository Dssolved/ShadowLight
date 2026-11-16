using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] GameObject confirmPanel;

    void Start()
    {
        musicSlider.minValue = SaveData.MinVolume;
        sfxSlider.minValue = SaveData.MinVolume;
        musicSlider.maxValue = 1f;
        sfxSlider.maxValue = 1f;

        ShowSavedValues();
        confirmPanel.SetActive(false);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    void OnDestroy()
    {
        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
    }

    void ShowSavedValues()
    {
        musicSlider.SetValueWithoutNotify(SaveSystem.Data.musicVolume);
        sfxSlider.SetValueWithoutNotify(SaveSystem.Data.sfxVolume);
    }

    void SetMusicVolume(float value)
    {
        SaveSystem.Data.musicVolume = value;
    }

    void SetSfxVolume(float value)
    {
        SaveSystem.Data.sfxVolume = value;
    }

    public void AskReset()
    {
        confirmPanel.SetActive(true);
    }

    public void CancelReset()
    {
        confirmPanel.SetActive(false);
    }

    public void ConfirmReset()
    {
        SaveSystem.Reset();
        ShowSavedValues();
        confirmPanel.SetActive(false);
    }
}
