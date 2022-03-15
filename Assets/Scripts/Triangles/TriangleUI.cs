using System;
using Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Triangles
{
    public class TriangleUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Guid guid;

        [SerializeField] private GuidEvent vertexSelectEvent;
        [SerializeField] private GuidEvent vertexDeSelectEvent;

        [SerializeField] private Color normalColor;
        [SerializeField] private Color hoverColor;
        [SerializeField] private Color selectedColor;

        private Image image;
        private bool isSelected;
    
        private void Awake()
        {
            image = GetComponent<Image>();
        }
        
        public TriangleUI Populate(Guid _guid)
        {
            guid = _guid;
            return this;
        }

        public void Select()
        {
            image.color = selectedColor;
            isSelected = true;
        }

        public void DeSelect()
        {
            image.color = normalColor;
            isSelected = false;
        }
        
        public void OnPointerClick(PointerEventData _eventData)
        {
            if (isSelected)
            {            
                vertexDeSelectEvent.Invoke(guid);
            }
            else
            {
                vertexSelectEvent.Invoke(guid);
            }
        }

        public void OnPointerEnter(PointerEventData _eventData)
        {
            if (!isSelected)
                image.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData _eventData)
        {
            if (!isSelected)
                image.color = normalColor;
        }
    }
}