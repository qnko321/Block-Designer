using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Enums;
using Events;
using TMPro;
using Triangles;
using UnityEngine;
using UnityEngine.InputSystem;
using Vertices;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Events")] 
        [SerializeField] private GuidEvent vertexCreateEvent;
        [SerializeField] private GuidEvent triangleCreateEvent;

        [Header("Input")] 
        [SerializeField] private InputActionReference leftControlInput;

        [Header("Prefabs")] 
        [SerializeField] private GameObject vertexUIPrefab;
        [SerializeField] private GameObject triangleUIPrefab;

        [Header("References")] 
        [SerializeField] private MeshManager meshManager;
        [SerializeField] private GameObject verticesList;
        [SerializeField] private GameObject trianglesList;
        [SerializeField] private Transform verticesParent;
        [SerializeField] private Transform trianglesParent;
        [SerializeField] private AutoContentSize autoContentSizeVertices;
        [SerializeField] private AutoContentSize autoContentSizeTriangles;

        [Header("Inspector")]
        [SerializeField] private TMP_InputField xPosInput;
        [SerializeField] private TMP_InputField yPosInput;
        [SerializeField] private TMP_InputField zPosInput;

        private bool LeftControlPressed => Math.Abs(leftControlInput.ToInputAction().ReadValue<float>() - 1) < float.Epsilon;

        private readonly Dictionary<Guid, VertexUI> vertexUIs = new();
        private readonly Dictionary<Guid, TriangleUI> triangleUIs = new();
        private readonly Dictionary<Guid, VertexUI> selectedVertices = new();

        private CurrentMenu currentMenu;
        private GameObject currentList;

        private Vertex inspectorVertex;

        private void Awake()
        {
            currentMenu = CurrentMenu.Vertices;
            currentList = verticesList;
            verticesList.SetActive(true);
            trianglesList.SetActive(false);
        }

        private void FixedUpdate()
        {
            UpdateInspector();
        }

        private void UpdateInspector()
        {
            if (inspectorVertex == null)
            {
                xPosInput.text = "-";
                yPosInput.text = "-";
                zPosInput.text = "-";
            }
            else
            {
                var _position = inspectorVertex.transform.position;
                if (!xPosInput.isFocused)
                    xPosInput.text = _position.x.ToString(CultureInfo.InvariantCulture);
                if (!yPosInput.isFocused)
                    yPosInput.text = _position.y.ToString(CultureInfo.InvariantCulture);
                if (!zPosInput.isFocused)
                    zPosInput.text = _position.z.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void UpdateInspectorValues()
        {
            if (selectedVertices.Count == 1)
            {
                Guid _guid = selectedVertices.Values.ToArray()[0].guid;
                Vertex _vertex = meshManager.GetVertex(_guid);
                inspectorVertex = _vertex;
            }
            else
            {
                inspectorVertex = null;
            }
        }
    
        #region Events

        #region Vertices

        public void OnVertexCreate(Guid _guid)
        {
            VertexUI _vertexUI = Instantiate(vertexUIPrefab, verticesParent).GetComponent<VertexUI>().Populate(_guid);
            autoContentSizeVertices.CorrectSize();
            vertexUIs.Add(_guid, _vertexUI);
            DeselectAllVertices();
            _vertexUI.Select();
            selectedVertices.Add(_guid, _vertexUI);
                
            SelectVertex(_guid, false);
        }
            
        public void OnSelectVertex(Guid _guid)
        {
            SelectVertex(_guid, LeftControlPressed);
        }
            
        public void OnDeSelectVertex(Guid _guid)
        {
            if (LeftControlPressed)
            {
                DeSelectVertex(_guid);
            }
            else
            {
                DeselectAllVertices();
            }
        }

        #endregion

        #region Triangles

        public void OnTriangleCreate(Guid _guid)
        {
            GameObject _triangleUIObject = Instantiate(triangleUIPrefab, trianglesParent);
            TriangleUI _triUi = _triangleUIObject.GetComponent<TriangleUI>();
            _triUi.guid = _guid;
            autoContentSizeTriangles.CorrectSize();
            triangleUIs.Add(_guid, _triUi);
            DeselectAllVertices();
            _triUi.Select();

            SelectTriangle(_guid);
        }
        
        public void OnSelectTriangle(Guid _guid)
        {
            SelectTriangle(_guid);
        }
        
        public void OnDeSelectTriangle(Guid _guid)
        {
            DeSelectTriangle(_guid);
        }

        #endregion
        
        public void OnXPosValueChange(string _strValue)
        {
            if (inspectorVertex == null) return;
            if (float.TryParse(_strValue, out float _value))
            {
                meshManager.MoveVertexToX(inspectorVertex.guid, _value);
            }
        }

        public void OnXPosEndEdit(string _strValue)
        {
            if (inspectorVertex == null)
            {
                xPosInput.text = "-";
            }
            else
            {
                if (float.TryParse(_strValue, out float _value))
                {
                    xPosInput.text = _value.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    xPosInput.text = inspectorVertex.transform.position.x.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public void OnYPosValueChange(string _strValue)
        {
            if (inspectorVertex == null) return;
            if (float.TryParse(_strValue, out float _value))
            {
                meshManager.MoveVertexToY(inspectorVertex.guid, _value);
            }
        }
    
        public void OnYPosEndEdit(string _strValue)
        {
            if (inspectorVertex == null)
            {
                yPosInput.text = "-";
            }
            else
            {
                if (float.TryParse(_strValue, out float _value))
                {
                    yPosInput.text = _value.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    yPosInput.text = inspectorVertex.transform.position.y.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
    
        public void OnZPosValueChange(string _strValue)
        {
            if (inspectorVertex == null) return;
            if (float.TryParse(_strValue, out float _value))
            {
                meshManager.MoveVertexToZ(inspectorVertex.guid, _value);
            }
        }
    
        public void OnZPosEndEdit(string _strValue)
        {
            if (inspectorVertex == null)
            {
                zPosInput.text = "-";
            }
            else
            {
                if (float.TryParse(_strValue, out float _value))
                {
                    zPosInput.text = _value.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    zPosInput.text = inspectorVertex.transform.position.z.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        #endregion

        #region UI
    
        public void TrianglesClick()
        {
            if (currentList != null) currentList.SetActive(false);
            currentList = trianglesList;
            currentList.SetActive(true);
            currentMenu = CurrentMenu.Triangles;
        }
        
        public void VerticesClick()
        {
            if (currentList != null)  currentList.SetActive(false);
            currentList = verticesList;
            currentList.SetActive(true);
            currentMenu = CurrentMenu.Vertices;
        }

        public void Create()
        {
            switch (currentMenu)
            {
                case CurrentMenu.Vertices:
                    vertexCreateEvent.Invoke(Guid.NewGuid());
                    break;
                case CurrentMenu.Triangles:
                    triangleCreateEvent.Invoke(Guid.NewGuid());
                    break;
                default:
                    Debug.LogError("No Menu Selected");
                    break;
            }
        }

        #endregion

        #region Vertices

        private void SelectVertex(Guid _guid, bool _addToOthers = true)
        {
            if (_addToOthers)
            {
                selectedVertices.Add(_guid, vertexUIs[_guid]);
                selectedVertices[_guid].Select();
            }
            else
            {
                DeselectAllVertices();
                selectedVertices.Add(_guid, vertexUIs[_guid]);
                selectedVertices[_guid].Select();
            }

            UpdateInspectorValues();
        }
        
        private void DeSelectVertex(Guid _guid)
        {
            selectedVertices[_guid].DeSelect();
            selectedVertices.Remove(_guid);

            UpdateInspectorValues();
        }
        
        private void DeselectAllVertices()
        {
            foreach (VertexUI _vertexUI in selectedVertices.Values)
            {
                _vertexUI.DeSelect();
            }
            selectedVertices.Clear();
        }

        #endregion

        #region Triangles

        private void SelectTriangle(Guid _guid)
        {
            // TODO: select triangle
        }

        private void DeSelectTriangle(Guid _guid)
        {
            // TODO: de select triangle
        }

        #endregion
    }
}