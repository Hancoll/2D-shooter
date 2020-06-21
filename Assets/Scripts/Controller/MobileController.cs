using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileController : MonoBehaviour,IDragHandler,IPointerUpHandler,IPointerDownHandler
{
    public RectTransform background;
    public RectTransform handle;
    Vector2 joystickBGzeroPosition;
    public event System.Action<Vector2> eventOnChangeInputDirection;
    public event System.Action eventOnPointerUp;
    public event System.Action eventOnPointerDown;
    protected Vector2 inputDirection;

    protected virtual void Start()
    {
        GameManager.GameUIManager.onInventoryChangeActive += ChangeJoystickVisible;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        joystickBGzeroPosition = eventData.position;
        OnDrag(eventData);
        eventOnPointerDown?.Invoke();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        handle.localPosition = Vector2.zero;
        inputDirection = Vector2.zero;
        eventOnChangeInputDirection?.Invoke(inputDirection);
        eventOnPointerUp?.Invoke();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out Vector2 position))
        {
            float radius = background.sizeDelta.x / 2;
            inputDirection = position.normalized;
            float joystickRange = Mathf.Clamp(position.magnitude, -radius, radius);
            Vector2 joystickPosition = inputDirection * joystickRange;
            handle.anchoredPosition = joystickPosition;

            eventOnChangeInputDirection?.Invoke(inputDirection);
        }
    }

    void ChangeJoystickVisible(bool active)
    {
        background.gameObject.SetActive(!active);
        handle.gameObject.SetActive(!active);
    }
}
