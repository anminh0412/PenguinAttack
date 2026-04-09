using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GetUIInputEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action<PointerEventData> onPointerDownEvent;
    public Action<PointerEventData> onPointerUpEvent;
    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDownEvent?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUpEvent?.Invoke(eventData);
    }
}