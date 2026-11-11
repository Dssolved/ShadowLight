using UnityEngine;
using UnityEngine.UI;

public class FlamesHud : MonoBehaviour
{
    [SerializeField] Lives lives;
    [SerializeField] Image[] flames;
    [SerializeField] Sprite litSprite;
    [SerializeField] Sprite spentSprite;

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

    void ShowFlames(int litCount)
    {
        for (int i = 0; i < flames.Length; i++)
        {
            flames[i].sprite = i < litCount ? litSprite : spentSprite;
        }
    }
}
