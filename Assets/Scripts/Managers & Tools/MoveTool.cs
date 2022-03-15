using System;
using Enums;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveTool : MonoBehaviour
{
    [SerializeField] private InputActionReference leftClickInput;
    [SerializeField] private InputActionReference mousePositionInput;
    [SerializeField] private GameObject xArrow;
    [SerializeField] private GameObject yArrow;
    [SerializeField] private GameObject zArrow;
    [SerializeField] private Axis moveAxis;

    private MeshRenderer[] renderers;
    private Transform[] transformsToMove;

    private Transform trans;
    private Camera cam;
    
    private Vector3 offset;
    private float zCoord;
    private Vector2 startPos;
    private Vector2 mousePos;
    private bool isMouseDown;

    private void Awake()
    {
        trans = transform;
        cam = Camera.main;
        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    private void OnEnable()
    {
        leftClickInput.action.started += OnLeftMouseDown;
        leftClickInput.action.canceled += OnLeftMouseUp;
        mousePositionInput.action.performed += OnMousePosition;
    }

    private void OnDisable()
    {
        leftClickInput.action.started -= OnLeftMouseDown;
        leftClickInput.action.canceled -= OnLeftMouseUp;
        mousePositionInput.action.performed -= OnMousePosition;
    }

    private void OnLeftMouseDown(InputAction.CallbackContext _ctx)
    {
        if (Physics.Raycast(cam.ScreenPointToRay(mousePos), out var _hitInfo))
        {
            if (_hitInfo.transform.gameObject == xArrow)
            {
                moveAxis = Axis.X;
                offset = transform.position - GetMouseWorldPos();
                startPos = mousePos;
                isMouseDown = true;
            }
            else if (_hitInfo.transform.gameObject == yArrow)
            {
                moveAxis = Axis.Y;
                offset = transform.position - GetMouseWorldPos();
                startPos = mousePos;
                isMouseDown = true;
            }
            else if (_hitInfo.transform.gameObject == zArrow)
            {
                moveAxis = Axis.Z;
                offset = transform.position - GetMouseWorldPos();
                startPos = mousePos;
                isMouseDown = true;
            }
        }
    }

    private void OnLeftMouseUp(InputAction.CallbackContext _ctx)
    {
        isMouseDown = false;
    }

    private void OnMousePosition(InputAction.CallbackContext _ctx)
    {
        mousePos = _ctx.ReadValue<Vector2>();
        if (Math.Abs(startPos.x - mousePos.x) > Constants.DragTolerance || Math.Abs(startPos.y - mousePos.y) > Constants.DragTolerance)
        {
            if (!isMouseDown) return;

            Vector3 _newPos = GetMouseWorldPos() + offset;
            
            Vector3 _position = trans.position;
            switch (moveAxis)
            {
                case Axis.X:
                    _newPos.y = _position.y;
                    _newPos.z = _position.z;
                    break;
                case Axis.Y:
                    _newPos.x = _position.x;
                    _newPos.z = _position.z;
                    break;
                case Axis.Z:
                    _newPos.x = _position.x;
                    _newPos.y = _position.y;
                    break;
            }

            var _transPosition = trans.position;
            Vector3 _offset = _newPos - _transPosition;
            _transPosition += _offset;
            trans.position = _transPosition;
            foreach (Transform _move in transformsToMove)
            {
                _move.localPosition += _offset;
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        zCoord = cam.WorldToScreenPoint(transform.position).z;
        Vector3 _mousePoint = mousePos;
        _mousePoint.z = zCoord;

        return cam.ScreenToWorldPoint(_mousePoint);
    }

    public void SetObjects(Transform[] _transforms)
    {
        transformsToMove = _transforms;
        Show();
    }

    public void SetPos(Vector3 _pos)
    {
        trans.position = _pos;
    }

    public void Hide()
    {
        foreach (MeshRenderer _renderer in renderers)
        {
            _renderer.enabled = false;
        }
    }

    public void Show()
    {
        foreach (MeshRenderer _renderer in renderers)
        {
            _renderer.enabled = true;
        }
    }
}