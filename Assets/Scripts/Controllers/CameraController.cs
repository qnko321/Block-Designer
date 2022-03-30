using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Vertices;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MeshManager meshManager;
        [SerializeField] private UIManager uiManager;

        [Header("Settings")] 
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float rotateSpeed = 3f;
        [SerializeField] private float smoothTime = 0.003f;


        private float rotationY;
        private float rotationX;
        private Vector3 currentRotation;
        private Vector3 smoothVelocity = Vector3.zero;
        private Vector2 mousePos;
        private Vector2 prevMousePos;    
        private float distanceFromTarget = 5f;

        private Transform trans;
        private Transform target;
        private Camera cam;
        private bool rightButtonPressed;
        private bool leftShiftPressed;
    

        private void Awake()
        {
            trans = transform;
            cam = GetComponent<Camera>();
            InstantiateTarget();
        }

        private void InstantiateTarget()
        {
            target = new GameObject("CameraTarget").transform;
            target.SetPositionAndRotation(trans.position + trans.forward * distanceFromTarget, Quaternion.identity);
        }

        public void GetLookInput(InputAction.CallbackContext _ctx)
        {
            Vector2 mouseDelta = _ctx.ReadValue<Vector2>();
            Look(mouseDelta);
        } 
    
        public void GetRightButtonInput(InputAction.CallbackContext _ctx)
        {
            if (_ctx.started)
            {
                // Down
                rightButtonPressed = true;
            }
            else if (_ctx.canceled)
            {
                // Up
                rightButtonPressed = false;
            }
        }    

        public void GetLeftButtonClick(InputAction.CallbackContext _ctx)
        {
            if (_ctx.canceled)
            {
                Select();
            }
        }

        public void GetScrollInput(InputAction.CallbackContext _ctx)
        {
            float _z = _ctx.ReadValue<float>();
            Zoom(_z);
        }

        public void GetMousePosition(InputAction.CallbackContext _ctx)
        {
            prevMousePos = mousePos;
            mousePos = _ctx.ReadValue<Vector2>();
        
            Move();
        }

        public void GetLeftShiftButton(InputAction.CallbackContext _ctx)
        {
            if (_ctx.started)
            {
                // Down
                leftShiftPressed = true;
            }
            else if (_ctx.canceled)
            {
                // Up
                leftShiftPressed = false;
            }
        }

        private void Zoom(float _z)
        {
            if (_z > 0)
            {
                if (!(Vector3.Distance(trans.position, Vector3.zero) > 1.1f)) return;
            
                distanceFromTarget -= 1;
                trans.position = target.position - trans.forward * distanceFromTarget;
            }
            else if (_z < 0)
            {
                if (!(Vector3.Distance(trans.position, Vector3.zero) < 9.9f)) return;
            
                distanceFromTarget += 1;
                trans.position = target.position - trans.forward * distanceFromTarget;
            }
        }

        private void Select()
        {
            bool _hit = Physics.Raycast(cam.ScreenPointToRay(mousePos), out var _hitInfo);
            if (_hit)
            {
                // ReSharper disable once Unity.UnknownTag
                if (_hitInfo.transform.gameObject.CompareTag("Vertex"))
                {
                    Vertex _vertex = _hitInfo.transform.gameObject.GetComponent<Vertex>();
                    if (meshManager.IsVertexSelected(_vertex))
                    {
                        meshManager.DeSelectVertex(_vertex.guid);
                        uiManager.DeSelectVertex(_vertex.guid);
                    }
                    else
                    {
                        meshManager.SelectVertex(_vertex.guid);
                        uiManager.SelectVertex(_vertex.guid, leftShiftPressed);
                    }
                }
            }
        }

        private void Look(Vector2 _mouseDelta)
        {
            if (!rightButtonPressed || leftShiftPressed) return;
        
            rotationY += _mouseDelta.x * rotateSpeed;
            rotationX -= _mouseDelta.y * rotateSpeed;

            Vector3 _nextRotation = new Vector3(rotationX, rotationY);
            currentRotation = Vector3.SmoothDamp(currentRotation, _nextRotation, ref smoothVelocity, smoothTime);
        
            trans.localEulerAngles = currentRotation;
            trans.position = target.position - trans.forward * distanceFromTarget;
        }

        private void Move()
        {
            if (!rightButtonPressed || !leftShiftPressed) return;
            if (prevMousePos == mousePos) return;

            Vector3 _move = cam.ScreenToViewportPoint(prevMousePos) - cam.ScreenToViewportPoint(mousePos);
            trans.Translate(_move * moveSpeed);
            target.position = trans.position + trans.forward * distanceFromTarget;
        }
    }
}
