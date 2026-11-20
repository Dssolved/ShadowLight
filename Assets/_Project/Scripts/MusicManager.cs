using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] AudioSource sourceA;
    [SerializeField] AudioSource sourceB;
    [SerializeField] float fadeSeconds = 1f;
    [SerializeField] float duckVolume = 0.15f;
    [SerializeField] float duckFadeSeconds = 0.3f;

    AudioSource current;
    AudioSource other;
    float fadeTimer;
    bool fading;
    float duckFactor = 1f;
    float duckTarget = 1f;
    float duckHoldTimer;

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

    public void Duck(float holdSeconds)
    {
        duckTarget = duckVolume;
        duckHoldTimer = holdSeconds;
    }

    void Update()
    {
        if (duckHoldTimer > 0f)
        {
            duckHoldTimer -= Time.unscaledDeltaTime;
            if (duckHoldTimer <= 0f) duckTarget = 1f;
        }
        duckFactor = Mathf.MoveTowards(duckFactor, duckTarget, Time.unscaledDeltaTime / duckFadeSeconds);

        float currentLevel = 1f;
        float otherLevel = 0f;

        if (fading)
        {
            fadeTimer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeSeconds);
            currentLevel = t;
            otherLevel = 1f - t;

            if (t >= 1f)
            {
                other.Stop();
                fading = false;
            }
        }

        current.volume = currentLevel * duckFactor;
        other.volume = otherLevel * duckFactor;
    }
}
