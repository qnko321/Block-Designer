using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")] 
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color clickColor;

    [Header("References")] 
    [SerializeField] private UnityEvent onClick;
    
    private Image image;
    private bool isHovered;
    private bool isClicked;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void OnPointerEnter(PointerEventData _eventData)
    {
        isHovered = true;
        ChangeColor();
    }

    public void OnPointerExit(PointerEventData _eventData)
    {
        isHovered = false;
        ChangeColor();
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        isClicked = true;
        ChangeColor();
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        isClicked = false;
        ChangeColor();
        
        onClick.Invoke();
    }

    private void ChangeColor()
    {
        image.color = isClicked ? clickColor : isHovered ? hoverColor : normalColor;
    }
}
