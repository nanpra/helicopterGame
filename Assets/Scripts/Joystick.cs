using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform joystickHandle;
    public RectTransform joystickBase;
    public float joystickRange = 200f;

    private Vector2 joystickStartPos;

    private Vector2 inputVector = Vector2.zero;
    private Vector2 joystickStartPosition;

    public Vector2 InputVector => inputVector;

    private void Start()
    {
        joystickStartPos = joystickHandle.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase, eventData.position, eventData.pressEventCamera, out pos);
        pos = Vector2.ClampMagnitude(pos, joystickRange);
        joystickHandle.anchoredPosition = pos;
        inputVector = pos / joystickRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetJoystick();
    }

    private void ResetJoystick()
    {
        joystickHandle.anchoredPosition = joystickStartPos;
        inputVector = Vector2.zero;
    }

    public Vector2 GetJoystickInput()
    {
        return inputVector;
    }
}
