using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer Instance { get; private set; }

    [SerializeField] SoundBank bank;
    [SerializeField] AudioSource[] voices;
    [SerializeField] float minPitch = 0.92f;
    [SerializeField] float maxPitch = 1.08f;

    int nextVoice;
    AudioClip lastClip;

    public SoundBank Bank => bank;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip[] clips)
    {
        PlayClip(clips, true);
    }

    public void PlayJingle(AudioClip[] clips)
    {
        PlayClip(clips, false);
        if (lastClip != null) MusicManager.Instance.Duck(lastClip.length);
    }

    void PlayClip(AudioClip[] clips, bool randomPitch)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        while (clips.Length > 1 && clip == lastClip)
        {
            clip = clips[Random.Range(0, clips.Length)];
        }
        lastClip = clip;

        AudioSource voice = voices[nextVoice];
        nextVoice = (nextVoice + 1) % voices.Length;

        voice.pitch = randomPitch ? Random.Range(minPitch, maxPitch) : 1f;
        voice.clip = clip;
        voice.Play();
    }
}
