using TMPro;
using UnityEngine;

public class FlamesHud : MonoBehaviour
{
    [SerializeField] Lives lives;
    [SerializeField] TMP_Text countText;

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
        countText.text = "x" + count;
    }
}
