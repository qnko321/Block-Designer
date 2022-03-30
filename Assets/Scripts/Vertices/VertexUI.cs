using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Vertices
{
    public class VertexUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Guid guid;

        [Header("References")] 
        [SerializeField] private InputActionReference f2Input;

        [Header("Settings")]
        [SerializeField] private Color normalColor;
        [SerializeField] private Color hoverColor;
        [SerializeField] private Color selectedColor;

        private Image image;
        private bool isSelected;
        private bool isRenaming;
        private TMP_InputField inputField;
        private Action<Guid, bool> onClickCallback;
        private Action<Guid, string> renameCallback;

        public VertexUI Populate(Guid _guid, Action<Guid, bool> _onClickCallback, Action<Guid, string> _renameCallback)
        {
            guid = _guid;
            onClickCallback = _onClickCallback;
            renameCallback = _renameCallback;
            return this;
        }

        private void Awake()
        {
            image = GetComponent<Image>();
            inputField = GetComponentInChildren<TMP_InputField>();
            f2Input.ToInputAction().started += RenameEvent;
        }

        private void RenameEvent(InputAction.CallbackContext _ctx)
        {
            if (!isSelected) return;
            if (isRenaming) return;
            
            inputField.Select();
        }

        public void OnEndRename(string _value)
        {
            renameCallback(guid, _value);
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
                onClickCallback.Invoke(guid, false);
                isSelected = false;
            }
            else
            {
                onClickCallback.Invoke(guid, true);
                isSelected = true;
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

        public void Delete()
        {
            Destroy(gameObject);
        } 
    }
}