using System;
using Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Vertices
{
    public class VertexUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Guid guid;
    
        [SerializeField] private GuidEvent vertexSelectEvent;
        [SerializeField] private GuidEvent vertexDeSelectEvent;

        [SerializeField] private Color normalColor;
        [SerializeField] private Color hoverColor;
        [SerializeField] private Color selectedColor;

        private Image image;
        private bool isSelected;

        public VertexUI Populate(Guid _guid)
        {
            guid = _guid;
            return this;
        }

        private void Awake()
        {
            image = GetComponent<Image>();
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