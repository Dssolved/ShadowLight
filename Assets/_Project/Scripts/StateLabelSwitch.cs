using TMPro;
using UnityEngine;

public class StateLabelSwitch : MonoBehaviour
{
    [SerializeField] bool showStateLabels = true;

    public void Apply(GameObject enemy)
    {
        foreach (TMP_Text label in enemy.GetComponentsInChildren<TMP_Text>(true))
        {
            label.gameObject.SetActive(showStateLabels);
        }
    }
}
