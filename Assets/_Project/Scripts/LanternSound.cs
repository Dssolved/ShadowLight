using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LanternSound : MonoBehaviour
{
    [SerializeField] float maxVolume = 0.35f;
    [SerializeField] float fullVolumeSpeed = 4f;
    [SerializeField] float volumeChangePerSecond = 1.5f;
    [SerializeField] float minPitch = 0.95f;
    [SerializeField] float maxPitch = 1.1f;

    AudioSource source;
    Vector3 lastPosition;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.volume = 0f;
        lastPosition = transform.position;
    }

    void Update()
    {
        float speed = 0f;
        if (Time.deltaTime > 0f)
        {
            speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        }
        lastPosition = transform.position;

        float strength = Mathf.Clamp01(speed / fullVolumeSpeed);
        source.volume = Mathf.MoveTowards(source.volume, strength * maxVolume, volumeChangePerSecond * Time.unscaledDeltaTime);
        source.pitch = Mathf.Lerp(minPitch, maxPitch, strength);
    }
}
