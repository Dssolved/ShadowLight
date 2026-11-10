using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HeroAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] Sprite standingFrame;
    [SerializeField] Sprite[] walkFrames;
    [SerializeField] float walkFramesPerSecond = 6f;
    [SerializeField] float movingSpeedThreshold = 0.1f;

    Rigidbody2D body;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool walking = Mathf.Abs(body.linearVelocity.x) > movingSpeedThreshold;
        if (walking)
        {
            int frameIndex = (int)(Time.time * walkFramesPerSecond) % walkFrames.Length;
            bodyRenderer.sprite = walkFrames[frameIndex];
        }
        else
        {
            bodyRenderer.sprite = standingFrame;
        }
    }
}
