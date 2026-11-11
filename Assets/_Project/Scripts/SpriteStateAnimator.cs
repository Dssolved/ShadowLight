using System;
using UnityEngine;

public class SpriteStateAnimator : MonoBehaviour
{
    [Serializable]
    public class StateClip
    {
        public Sprite[] frames;
        public float framesPerSecond = 8f;
        public bool loop = true;
    }

    [SerializeField] SpriteRenderer target;
    [SerializeField] StateClip[] clips;
    [SerializeField] int autoPlayClip = -1;

    StateClip current;
    float clipTimer;

    void Start()
    {
        if (autoPlayClip >= 0) Play(autoPlayClip);
    }

    void Update()
    {
        if (current != null) ShowFrame();
        clipTimer += Time.deltaTime;
    }

    public void Play(int clipIndex)
    {
        current = clips[clipIndex];
        clipTimer = 0f;
        ShowFrame();
    }

    void ShowFrame()
    {
        int frame = Mathf.FloorToInt(clipTimer * current.framesPerSecond);
        if (current.loop) frame %= current.frames.Length;
        else frame = Mathf.Min(frame, current.frames.Length - 1);
        target.sprite = current.frames[frame];
    }
}
