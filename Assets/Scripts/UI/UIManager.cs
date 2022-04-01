using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Enums;
using TMPro;
using Triangles;
using UI.Automation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vertices;
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Input")] 
        [SerializeField] private InputActionReference leftControlInput;
        [SerializeField] private InputActionReference deleteInput;

        [Header("Prefabs")] 
        [SerializeField] private GameObject vertexUIPrefab;
        [SerializeField] private GameObject triangleUIPrefab;

        [Header("References")] 
        [SerializeField] private MeshManager meshManager;
        [SerializeField] private GameObject verticesList;
        [SerializeField] private Image verticesListButtonImage;
        [SerializeField] private GameObject trianglesList;
        [SerializeField] private Image trianglesListButtonImage;
        [SerializeField] private Transform verticesParent;
        [SerializeField] private Transform trianglesParent;
        [SerializeField] private AutoContentSizeVerticalLayout autoContentSizeVertices;
        [SerializeField] private AutoContentSizeVerticalLayout autoContentSizeTriangles;

        [Header("Vertex Inspector")] 
        [SerializeField] private GameObject vertexInspector;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_InputField xPosInput;
        [SerializeField] private TMP_InputField yPosInput;
        [SerializeField] private TMP_InputField zPosInput;

        [Header("Triangle Inspector")] 
        [SerializeField] private GameObject triangleInspector;
        [SerializeField] private TMP_Text firstVertName;
        [SerializeField] private TMP_Text secondVertName;
        [SerializeField] private TMP_Text thirdVertName;

        [Header("Settings")] 
        [SerializeField] private Color normalMenuColor;
        [SerializeField] private Color selectedMenuColor;
        
        
        private bool leftControlPressed = false;

        private readonly Dictionary<Guid, VertexUI> vertexUis = new();
        private readonly Dictionary<Guid, VertexUI> selectedVertices = new();
        
        private readonly Dictionary<Guid, TriangleUI> triangleUIs = new();
        private TriangleUI selectedTriangle;

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

        private void OnEnable()
        {
            leftControlInput.ToInputAction().started += OnLeftControlDown;
            leftControlInput.ToInputAction().canceled += OnLeftControlUp;

            deleteInput.ToInputAction().started += OnDeleteSelected;
        }

        private void OnDisable()
        {
            leftControlInput.ToInputAction().started -= OnLeftControlDown;
            leftControlInput.ToInputAction().canceled -= OnLeftControlUp;
            
            deleteInput.ToInputAction().started -= OnDeleteSelected;
        }
        
        private void OnLeftControlDown(InputAction.CallbackContext _ctx) => leftControlPressed = true;

        private void OnLeftControlUp(InputAction.CallbackContext _ctx) => leftControlPressed = false;

        private void FixedUpdate()
        {
            UpdateInspector();
        }

        private void UpdateInspector()
        {
            if (vertexInspector.activeSelf)
            {
                if (selectedVertices.Count != 1)
                {
                    inspectorVertex = null;
                    
                    xPosInput.text = "-";
                    yPosInput.text = "-";
                    zPosInput.text = "-";
                }
                else
                {
                    Guid _guid = selectedVertices.Values.ToArray()[0].guid;
                    inspectorVertex = meshManager.vertices[_guid];
                    var _position = inspectorVertex.transform.position;
                    nameText.text = inspectorVertex.vertName;
                    if (!xPosInput.isFocused)
                        xPosInput.text = _position.x.ToString(CultureInfo.InvariantCulture);
                    if (!yPosInput.isFocused)
                        yPosInput.text = _position.y.ToString(CultureInfo.InvariantCulture);
                    if (!zPosInput.isFocused)
                        zPosInput.text = _position.z.ToString(CultureInfo.InvariantCulture);
                }
            }

            if (triangleInspector.activeSelf)
            {
                if (selectedTriangle == null)
                {
                    firstVertName.text = "-";
                    secondVertName.text = "-";
                    thirdVertName.text = "-";
                }
                else
                {
                    Triangle _triangle = meshManager.triangles[selectedTriangle.guid];
                    Guid[] _vertices = _triangle.GetVertices();
                    if (_vertices[0] != Guid.Empty)
                    {
                        firstVertName.text = meshManager.vertices[_vertices[0]].vertName;
                    }
                    else
                    {
                        firstVertName.text = "-";
                    }
                    if (_vertices[1] != Guid.Empty)
                    {
                        secondVertName.text = meshManager.vertices[_vertices[1]].vertName;
                    }
                    else
                    {
                        secondVertName.text = "-";
                    }
                    if (_vertices[2] != Guid.Empty)
                    {
                        thirdVertName.text = meshManager.vertices[_vertices[2]].vertName;
                    }
                    else
                    {
                        thirdVertName.text = "-";
                    }
                }
            }
        }

        #region Events

        public void OnDeleteSelected(InputAction.CallbackContext _ctx)
        {
            meshManager.OnDeleteSelected();
            foreach (VertexUI _vertex in selectedVertices.Values)
            {
                _vertex.Delete();
                vertexUis.Remove(_vertex.guid);
            }

            int _subtract = selectedVertices.Count;
            selectedVertices.Clear();
            autoContentSizeVertices.CorrectSize(_subtract);

            if (selectedTriangle != null)
            {
                triangleUIs.Remove(selectedTriangle.guid);
                selectedTriangle.Delete();
                selectedTriangle = null;
                autoContentSizeTriangles.CorrectSize(1);
            }
        }

        #region Inspector

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

        #endregion

        #region UI
    
        public void TrianglesClick()
        {
            if (currentList != null) currentList.SetActive(false);
            currentList = trianglesList;
            currentList.SetActive(true);
            currentMenu = CurrentMenu.Triangles;
            verticesListButtonImage.color = normalMenuColor;
            trianglesListButtonImage.color = selectedMenuColor;
            /*DeselectAllVertices();
            meshManager.DeSelectAllVertices();*/
        }
        
        public void VerticesClick()
        {
            if (currentList != null) currentList.SetActive(false);
            currentList = verticesList;
            currentList.SetActive(true);
            currentMenu = CurrentMenu.Vertices;
            trianglesListButtonImage.color = normalMenuColor;
            verticesListButtonImage.color = selectedMenuColor;
            /*if (selectedTriangle != null)
            {
                Guid _triGuid = selectedTriangle.guid;
                DeSelectTriangle(_triGuid);
                meshManager.DeSelectTriangle(_triGuid);
            }*/
        }

        public void Create()
        {
            Guid _guid = Guid.NewGuid();
            switch (currentMenu)
            {
                case CurrentMenu.Vertices:
                    CreateVertex(_guid);
                    meshManager.CreateVertex(_guid);
                    break;
                case CurrentMenu.Triangles:
                    CreateTriangle(_guid);
                    meshManager.CreateTriangle(_guid);
                    break;
                default:
                    Debug.LogError("No Menu Selected");
                    break;
            }
        }

        private void CreateVertex(Guid _guid)
        {
            VertexUI _vertexUI = Instantiate(vertexUIPrefab, verticesParent).GetComponent<VertexUI>().Populate(_guid, OnClickVertex, RenameVertex);
            autoContentSizeVertices.CorrectSize();
            vertexUis.Add(_guid, _vertexUI);
            DeselectAllVertices();
            _vertexUI.Select();
            selectedVertices.Add(_guid, _vertexUI);
                
            SelectVertex(_guid, false);
        }

        private void CreateTriangle(Guid _guid)
        {
            GameObject _triangleUIObject = Instantiate(triangleUIPrefab, trianglesParent);
            TriangleUI _triUi = _triangleUIObject.GetComponent<TriangleUI>();
            _triUi.Populate(_guid, OnTriangleClick, RenameTriangle);
            autoContentSizeTriangles.CorrectSize();
            triangleUIs.Add(_guid, _triUi);

            DeselectAllVertices();
            SelectTriangle(_guid);
        }
        
        #endregion

        #region Vertices

        private void RenameVertex(Guid _guid, string _name)
        {
            meshManager.vertices[_guid].vertName = _name;
        }
        
        private void OnClickVertex(Guid _guid, bool _isSelected)
        {
            if (_isSelected)
            {
                SelectVertex(_guid, leftControlPressed);
                meshManager.SelectVertex(_guid);
            }
            else
            {
                DeSelectVertex(_guid);
                meshManager.DeSelectVertex(_guid);
            }
        }

        public void SelectVertex(Guid _guid, bool _addToOthers = true)
        {
            if (selectedTriangle != null) return;
            // 20, 20, 255 - blue
            // 255, 20, 20 - red
            if (_addToOthers)
            {
                selectedVertices.Add(_guid, vertexUis[_guid]);
                selectedVertices[_guid].Select();
            }
            else
            {
                DeselectAllVertices();
                selectedVertices.Add(_guid, vertexUis[_guid]);
                selectedVertices[_guid].Select();
            }

            triangleInspector.SetActive(false);
            vertexInspector.SetActive(true);
        }

        public void DeSelectVertex(Guid _guid)
        {
            selectedVertices[_guid].DeSelect();
            selectedVertices.Remove(_guid);
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

        private void RenameTriangle(Guid _guid, string _name)
        {
            meshManager.triangles[_guid].triName = _name;
        }
        
        private void OnTriangleClick(Guid _guid, bool _isSelected)
        {
            if (_isSelected)
            {
                SelectTriangle(_guid);
                meshManager.SelectTriangle(_guid);
            }
            else
            {
                DeSelectTriangle(_guid);
                meshManager.DeSelectTriangle(_guid);
            }
        }
        
        private void SelectTriangle(Guid _guid)
        {
            DeselectAllVertices();
            if (selectedTriangle != null)
                selectedTriangle.DeSelect();
            
            selectedTriangle = triangleUIs[_guid];
            selectedTriangle.Select();
            
            vertexInspector.SetActive(false);
            triangleInspector.SetActive(true);
        }

        private void DeSelectTriangle(Guid _guid)
        {
            triangleUIs[_guid].DeSelect();
            selectedTriangle = null;
        }

        #endregion
    }
}