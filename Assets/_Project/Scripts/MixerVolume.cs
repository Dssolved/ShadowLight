using UnityEngine;
using UnityEngine.Audio;

public class MixerVolume : MonoBehaviour
{
    const string MusicParameter = "MusicVol";
    const string SfxParameter = "SFXVol";

    [SerializeField] AudioMixer mixer;

    void Start()
    {
        mixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
        Apply(mixer, SaveSystem.Data.musicVolume, SaveSystem.Data.sfxVolume);
    }

    public static void Apply(AudioMixer targetMixer, float musicVolume, float sfxVolume)
    {
        targetMixer.SetFloat(MusicParameter, ToDecibels(musicVolume));
        targetMixer.SetFloat(SfxParameter, ToDecibels(sfxVolume));
    }

    static float ToDecibels(float value)
    {
        return Mathf.Log10(Mathf.Max(value, SaveData.MinVolume)) * 20f;
    }
}
