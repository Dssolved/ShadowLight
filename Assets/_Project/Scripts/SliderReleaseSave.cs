using UnityEngine;
using UnityEngine.EventSystems;

public class SliderReleaseSave : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        SaveSystem.Save();
    }
}
