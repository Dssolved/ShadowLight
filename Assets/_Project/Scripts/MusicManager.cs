using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] AudioSource sourceA;
    [SerializeField] AudioSource sourceB;
    [SerializeField] float fadeSeconds = 1f;

    AudioSource current;
    AudioSource other;
    float fadeTimer;
    bool fading;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        current = sourceA;
        other = sourceB;
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        if (current.isPlaying && current.clip == clip) return;

        AudioSource fadingOut = current;
        current = other;
        other = fadingOut;

        current.clip = clip;
        current.loop = true;
        current.volume = 0f;
        current.Play();

        fadeTimer = 0f;
        fading = true;
    }

    void Update()
    {
        if (!fading) return;

        fadeTimer += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(fadeTimer / fadeSeconds);
        current.volume = t;
        other.volume = 1f - t;

        if (t >= 1f)
        {
            other.Stop();
            fading = false;
        }
    }
}
