using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UnityEvent onClick;

    public void OnPointerClick(PointerEventData _eventData)
    {
        onClick.Invoke();
    }
}
