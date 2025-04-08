using UnityEngine;
using UnityEngine.EventSystems;

public class ClickBlocker : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        eventData.Use();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        eventData.Use();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        eventData.Use();
    }
}
