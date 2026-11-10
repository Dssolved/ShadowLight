using UnityEngine;
using UnityEngine.UI;

public class FlamesHud : MonoBehaviour
{
    [SerializeField] Lives lives;
    [SerializeField] Image[] flames;
    [SerializeField] Color litColor = new Color(0.949f, 0.722f, 0.294f, 1f);
    [SerializeField] Color spentColor = new Color(0.949f, 0.722f, 0.294f, 0.25f);

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
            flames[i].color = i < litCount ? litColor : spentColor;
        }
    }
}
