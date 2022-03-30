using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference deleteButtonInput;

    private void OnEnable()
    {
        deleteButtonInput.ToInputAction().started += OnDeleteButtonDown;
    }

    private void OnDeleteButtonDown(InputAction.CallbackContext _ctx)
    {
        // Delete selected elements
    }
}
