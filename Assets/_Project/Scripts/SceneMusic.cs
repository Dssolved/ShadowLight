using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] AudioClip music;

    void Start()
    {
        MusicManager.Instance.Play(music);
    }
}
