using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TopMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Settings")] 
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color clickColor;

    [Header("References")]
    [SerializeField] private GameObject menu;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private bool isHovered;
    private bool isClicked;
    
    public void OnPointerEnter(PointerEventData _eventData)
    {
        isHovered = true;
        ChangeColor();
    }

    public void OnPointerExit(PointerEventData _eventData)
    {
        isHovered = false;
        isClicked = false;
        ChangeColor();
        
        menu.SetActive(false);
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        isClicked = !isClicked;
        ChangeColor();
        
        if (isHovered)
        {
            menu.SetActive(isClicked);
        }
    }
    
    public void Close()
    {
        menu.SetActive(false);
    }

    private void ChangeColor()
    {
        image.color = isClicked ? clickColor : isHovered ? hoverColor : normalColor;
    }
}
