using TMPro;
using UnityEngine;

public class StateLabelSwitch : MonoBehaviour
{
    [SerializeField] bool showStateLabels = true;
    [SerializeField] TMP_Text[] stateLabels;

    void Awake()
    {
        foreach (TMP_Text label in stateLabels)
        {
            label.gameObject.SetActive(showStateLabels);
        }
    }
}
