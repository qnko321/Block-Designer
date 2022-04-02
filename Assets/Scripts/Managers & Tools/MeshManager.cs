using System;
using System.Collections.Generic;
using System.Linq;
using Managers___Tools;
using Triangles;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Vertices;

public class MeshManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference leftControlInput;

    [Header("Prefabs")]
    [SerializeField] private GameObject vertexPrefab;

    [Header("References")] 
    [SerializeField] private MoveTool moveTool;
    [SerializeField] private MeshFilter meshFilter;

    public readonly Dictionary<string, Face> faces = new();
    private readonly List<string> selectedElements = new();
    
    private bool LeftControlPressed =>
        Math.Abs(leftControlInput.ToInputAction().ReadValue<float>() - 1) < float.Epsilon;

    #region Events

    public void OnDeleteSelected()
    {
        foreach (string _id in selectedElements)
        {
            string[] _idElements = _id.Split('-');
            string _type = _idElements[0];
            switch (_type)
            {
                case "0":
                    faces.Remove(_idElements[1]);
                    break;
                case "1":
                    faces[_idElements[1]].DeleteVertex(_idElements[2]);
                    break;
                case ElementType.Triangle:
                    faces[_selectedElement.faceGuid].DeleteTriangle(_guid);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        selectedElements.Clear();
    
        ReRenderMesh();
    }

    #endregion

    #region Faces

    public void CreateFace(Guid _guid)
    {
        faces.Add(_guid, new Face());
    }

    public void SelectFace(Guid _guid)
    {
        DeSelectAll();
        selectedElements.Add(new SelectedElement(_guid, ElementType.Face));
    }

    public void DeSelectFace(Guid _guid)
    {
        selectedElements.Remove(new SelectedElement(_guid, ElementType.Face));
    }

    #endregion
    
    #region Vertices

    public void CreateVertex(Guid _guid)
    {
        if (!IsFaceSelected(out Guid _faceGuid)) return;

        Vertex _vertex = Instantiate(vertexPrefab, transform).GetComponent<Vertex>();
        _vertex.guid = _guid;
        faces[_faceGuid].CreateVertex(_guid, _vertex);

        DeSelectAll();
        selectedElements.Add(new SelectedElement(selectedElements[0].guid, _guid, ElementType.Vertex));
    }

    public void SelectVertex(Guid _guid)
    {
        if (IsTriangleSelected(out Guid _faceGuidT, out Guid _triangleGuidT))
        {
            faces[_faceGuidT].SelectVertex(_triangleGuidT, _guid);
            ReRenderMesh();
            return;
        }
        
        Vertex _vertex = vertices[_guid];
        if (!LeftControlPressed)
        {
            DeSelectAll();
            _vertex.Select();

            Transform _trans = _vertex.transform;
            Vector3 _pos = _trans.position;

            moveTool.SetObjects(new[] {_trans});
            moveTool.SetPos(_pos);
        }
        else
        {
            selectedElements.Add(new SelectedElement(_faceGuid));
            _vertex.Select();

            Vertex[] _vertices = selectedVertices.ToArray();
            List<Transform> _transforms = new() {_vertices[0].transform};
            Vector3 _pos = new Vector3(_vertices[0].transform.position.x, _vertices[0].transform.position.y,
                _vertices[0].transform.position.z);
            
            for (int i = 1; i < _vertices.Length; i++)
            {
                Transform _trans = _vertices[i].transform;
                _transforms.Add(_trans);
                var _position = _trans.position;
                _pos.x = (_pos.x + _position.x) / 2f;
                _pos.y = (_pos.y + _position.y) / 2f;
                _pos.z = (_pos.z + _position.z) / 2f;
            }

            moveTool.SetObjects(_transforms.ToArray());
            moveTool.SetPos(_pos);
        }
    }

    public void DeSelectVertex(Guid _guid)
    {
        Vertex _vertex = vertices[_guid];
        selectedVertices.Remove(_vertex);
        _vertex.DeSelect();
        Vector3 _pos = default;
        List<Transform> _transforms = new List<Transform>();

        if (selectedVertices.Count == 0)
        {
            moveTool.Hide();
        }
        else if (selectedVertices.Count == 1)
        {
            Transform _vertTrans = selectedVertices[0].transform;
            _transforms.Add(_vertTrans);
            _pos = _vertTrans.position;
        }
        else if (selectedVertices.Count > 1)
        {
            Vertex[] _vertices = selectedVertices.ToArray();
            _pos.x = _vertices[0].transform.position.x;
            _pos.y = _vertices[0].transform.position.y;
            _pos.z = _vertices[0].transform.position.z;

            for (int i = 1; i < _vertices.Length; i++)
            {
                Transform _trans = _vertices[i].transform;
                _transforms.Add(_trans);
                var _position = _trans.position;
                _pos.x = (_pos.x + _position.x) / 2f;
                _pos.y = (_pos.y + _position.y) / 2f;
                _pos.z = (_pos.z + _position.z) / 2f;
            }
        }

        moveTool.SetObjects(_transforms.ToArray());
        moveTool.SetPos(_pos);
    }

    public void MoveVertexToX(Guid _guid, float _xValue)
    {
        Transform _vertTrans = vertices[_guid].transform;
        Vector3 _vertPos = _vertTrans.position;
        _vertTrans.position = new Vector3(_xValue, _vertPos.y, _vertPos.z);
        if (_vertPos == moveTool.transform.position)
            moveTool.transform.position = _vertTrans.position;
    }
    
    public void MoveVertexToY(Guid _guid, float _yValue)
    {
        Transform _vertTrans = vertices[_guid].transform;
        Vector3 _vertPos = _vertTrans.position;
        _vertTrans.position = new Vector3(_vertPos.x, _yValue, _vertPos.z);
        if (_vertPos == moveTool.transform.position)
            moveTool.transform.position = _vertTrans.position;
    }
    
    public void MoveVertexToZ(Guid _guid, float _zValue)
    {
        Transform _vertTrans = vertices[_guid].transform;
        Vector3 _vertPos = _vertTrans.position;
        _vertTrans.position = new Vector3(_vertPos.x, _vertPos.y, _zValue);
        if (_vertPos == moveTool.transform.position)
            moveTool.transform.position = _vertTrans.position;
    }

    #endregion
    
    #region Triangles

    public void CreateTriangle(Guid _guid)
    {
        if (!IsFaceSelected(out Guid _faceGuid)) return;
        
        faces[_faceGuid].CreateTriangle(_guid);
        SelectTriangle(_faceGuid, _guid);
    }

    public void SelectTriangle(Guid _faceGuid, Guid _guid)
    {
        DeSelectAll();
        selectedElements.Add(new SelectedElement(_faceGuid, _guid, ElementType.Triangle));
    }

    public void DeSelectTriangle(Guid _faceGuid, Guid _guid)
    {
        selectedElements.Remove(new SelectedElement(_faceGuid, _guid, ElementType.Triangle));
    }
    
    #endregion

    #region MeshPreview

    public void ReRenderMesh()
    {
        Vertex[] _verticesScript = vertices.Values.ToArray();
        Guid[] _vertGuids = new Guid[_verticesScript.Length];
        for (int i = 0; i < _verticesScript.Length; i++)
        {
            _vertGuids[i] = _verticesScript[i].guid;
        }

        List<Vector3> _vertices = vertices.Values.Select(_vertex => _vertex.Position).ToList();
        List<int> _triangles = new List<int>();
        List<Vector2> _uvs = new List<Vector2>();
        
        foreach (Triangle _triangle in triangles.Values)
        {
            Guid[] _trisVerts = _triangle.GetVertices();
            
            if (_trisVerts[0] == Guid.Empty || _trisVerts[1] == Guid.Empty || _trisVerts[2] == Guid.Empty)
                continue;
                
            for (int i = 0; i < 3; i++)
            {
                int _vertIndex = Array.IndexOf(_vertGuids, _trisVerts[i]);
                _triangles.Add(_vertIndex);
                if (selectedTriangle == _triangle)
                    _uvs.Add(new Vector2(0, 0));
                else
                    _uvs.Add(new Vector2(0, 1));
            }
        }
        

        if (_vertices.Count == 0 || _triangles.Count == 0)
        {
            meshFilter.mesh = null;
        }
        else
        {
            Mesh _mesh = new Mesh
            {
                vertices = _vertices.ToArray(),
                triangles = _triangles.ToArray()
            };
            meshFilter.mesh = _mesh;
            /*LogHelper.LogArray(_triangles, nameof(_triangles));
            LogHelper.LogArray(_vertices, nameof(_vertices));*/
        }
    }

    #endregion

    #region Serialization&Desirialization

    public void Export()
    {
        FileDialog.instance.SaveString(ToJson(true));
    }

    private string ToJson(bool _prettyJson)
    {
        Vertex[] _verticesScript = vertices.Values.ToArray();
        Guid[] _vertGuids = new Guid[_verticesScript.Length];
        for (int i = 0; i < _verticesScript.Length; i++)
        {
            _vertGuids[i] = _verticesScript[i].guid;
        }

        List<Vector3> _vertices = vertices.Values.Select(_vertex => _vertex.Position).ToList();
        List<int> _triangles = new List<int>();
        List<Vector2> _uvs = new List<Vector2>();
        
        foreach (Triangle _triangle in triangles.Values)
        {
            Guid[] _trisVerts = _triangle.GetVertices();
            
            if (_trisVerts[0] == Guid.Empty || _trisVerts[1] == Guid.Empty || _trisVerts[2] == Guid.Empty)
                continue;
                
            for (int i = 0; i < 3; i++)
            {
                int _vertIndex = Array.IndexOf(_vertGuids, _trisVerts[i]);
                _triangles.Add(_vertIndex);
                // TODO: Fix uvs
                /*if (selectedTriangle == _triangle)
                    _uvs.Add(new Vector2(0, 0));
                else
                    _uvs.Add(new Vector2(0, 1));*/
            }
        }

        string _json =
            JsonUtility.ToJson(new MeshInfo(_vertices.ToArray(), _triangles.ToArray(), _uvs.ToArray()), _prettyJson);
        return _json;
    }

    private struct MeshInfo
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        public MeshInfo(Vector3[] _vertices, int[] _triangles, Vector2[] _uvs)
        {
            vertices = _vertices;
            triangles = _triangles;
            uvs = _uvs;
        }
    }

    #endregion

    #region Selection

    private void DeSelectAll()
    {
        foreach (SelectedElement _element in selectedElements)
        {
            switch (_element.elementType)
            {
                case ElementType.Face:
                    DeSelectFace(_element.guid);
                    break;
                case ElementType.Vertex:
                    DeSelectVertex(_element.guid);
                    break;
                case ElementType.Triangle:
                    DeSelectTriangle(_element.guid);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private bool IsFaceSelected(out Guid _guid)
    {
        foreach (var _element in selectedElements.Where(_element => _element.elementType == ElementType.Face))
        {
            _guid = _element.guid;
            return true;
        }

        _guid = Guid.Empty;
        return false;
    }
    
    private bool IsTriangleSelected(out Guid _faceGuid, out Guid _guid)
    {
        foreach (var _element in selectedElements.Where(_element => _element.elementType == ElementType.Triangle))
        {
            _guid = _element.guid;
            _faceGuid = _element.faceGuid;
            return true;
        }

        _guid = Guid.Empty;
        _faceGuid = Guid.Empty;
        return false;
    }
    
    private bool IsVertexSelected()
    {
        return selectedElements.Any(_element => _element.elementType == ElementType.Vertex);
    }

    #endregion
}

public struct SelectedElement
{
    public readonly Guid faceGuid;
    public readonly Guid guid;
    public readonly ElementType elementType;

    public SelectedElement(Guid _faceGuid, Guid _guid, ElementType _elementType)
    {
        faceGuid = _faceGuid;
        guid = _guid;
        elementType = _elementType;
    }

    public SelectedElement(Guid _guid, ElementType _elementType) : this()
    {
        faceGuid = _guid;
        guid = _guid;
        elementType = _elementType;
    }
}

public enum ElementType
{
    Face = 0,
    Vertex,
    Triangle
}