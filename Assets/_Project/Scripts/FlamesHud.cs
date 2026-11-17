using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlamesHud : MonoBehaviour
{
    [SerializeField] Lives lives;
    [SerializeField] TMP_Text countText;
    [SerializeField] RectTransform flameIcon;
    [SerializeField] Image flameImage;
    [SerializeField] Image[] embers;
    [SerializeField] Sprite emberFull;
    [SerializeField] Sprite emberEmpty;
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color lowColor = new Color(1f, 0.45f, 0.35f, 1f);
    [SerializeField] Color healColor = new Color(1f, 1f, 0.6f, 1f);
    [SerializeField] int lowThreshold = 2;
    [SerializeField] float trembleDegrees = 6f;
    [SerializeField] float trembleSpeed = 35f;
    [SerializeField] float effectSeconds = 0.45f;
    [SerializeField] float popScale = 1.35f;

    int shown = -1;
    int lostIndex = -1;
    bool healing;
    float effectTimer;

    void OnEnable()
    {
        lives.Changed += ShowFlames;
    }

    void OnDisable()
    {
        lives.Changed -= ShowFlames;
    }

    void Start()
    {
        ShowFlames(lives.Current);
    }

    void ShowFlames(int count)
    {
        if (shown >= 0 && count < shown)
        {
            lostIndex = count;
            healing = false;
            effectTimer = effectSeconds;
        }
        else if (shown >= 0 && count > shown)
        {
            lostIndex = -1;
            healing = true;
            effectTimer = effectSeconds;
        }

        shown = count;
        countText.text = "x" + count;

        for (int i = 0; i < embers.Length; i++)
        {
            embers[i].sprite = i < count ? emberFull : emberEmpty;
            embers[i].color = Color.white;
        }
    }

    void Update()
    {
        if (shown < 0) return;

        effectTimer = Mathf.Max(0f, effectTimer - Time.unscaledDeltaTime);
        float k = effectTimer / effectSeconds;
        bool low = shown <= lowThreshold;

        Color baseColor = low ? lowColor : normalColor;
        flameImage.color = healing ? Color.Lerp(baseColor, healColor, k) : baseColor;

        float angle = low ? Mathf.Sin(Time.unscaledTime * trembleSpeed) * trembleDegrees : 0f;
        flameIcon.localRotation = Quaternion.Euler(0f, 0f, angle);
        flameIcon.localScale = Vector3.one * Mathf.Lerp(1f, popScale, k);

        if (lostIndex >= 0 && lostIndex < embers.Length)
        {
            embers[lostIndex].color = Color.Lerp(Color.white, lowColor, k);
        }
    }
}
