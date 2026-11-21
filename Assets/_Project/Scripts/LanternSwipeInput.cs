using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LanternSwipeInput : MonoBehaviour, IDragHandler
{
    [SerializeField] Slider slider;
    [SerializeField] float railPerScreenWidth = 1.5f;

    public void OnDrag(PointerEventData eventData)
    {
        slider.normalizedValue += eventData.delta.x / Screen.width * railPerScreenWidth;
    }
}
