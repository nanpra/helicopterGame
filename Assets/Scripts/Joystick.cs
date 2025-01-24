using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler
{
    public RectTransform joystickHandle;
    public RectTransform joystickBase;

    private Vector2 inputVector = Vector2.zero;
    private Vector2 joystickStartPosition;

    public Vector2 InputVector => inputVector; 

    private void Start()
    {
        joystickStartPosition = joystickBase.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragPosition = eventData.position;
        Vector2 offset = dragPosition - joystickStartPosition;
        float radius = joystickBase.sizeDelta.x / 2;
        inputVector = Vector2.ClampMagnitude(offset / radius, 1);
        joystickHandle.anchoredPosition = inputVector * radius;
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    print(1);
    //    OnDrag(eventData);
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    inputVector = Vector2.zero;
    //    joystickHandle.anchoredPosition = Vector2.zero;
    //}
}
