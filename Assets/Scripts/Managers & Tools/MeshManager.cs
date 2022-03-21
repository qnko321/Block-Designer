using System;
using System.Collections.Generic;
using System.Linq;
using Triangles;
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

    private bool LeftControlPressed =>
        Math.Abs(leftControlInput.ToInputAction().ReadValue<float>() - 1) < float.Epsilon;

    private readonly Dictionary<Guid, Vertex> vertices = new();
    private readonly List<Vertex> selectedVertices = new();

    private readonly Dictionary<Guid, Triangle> triangles = new();
    private Triangle selectedTriangle;

    #region Events

    #region Vertices
    public void OnCreateVertex(Guid _guid)
    {
        CreateVertex(_guid);
    }

    public void OnVertexSelect(Guid _guid)
    {
        SelectVertex(vertices[_guid], LeftControlPressed);
    }

    public void OnVertexDeSelect(Guid _guid)
    {
        if (LeftControlPressed)
        {
            DeSelectVertex(vertices[_guid]);
        }
        else
        {
            DeselectAllVertices();
        }
    }
    #endregion

    #endregion

    #region Vertices

    private void CreateVertex(Guid _guid)
    {
        Vertex _vertex = Instantiate(vertexPrefab, transform).GetComponent<Vertex>();
        _vertex.guid = _guid;
        vertices.Add(_guid, _vertex);
        SelectVertex(_vertex);
    }
    
    private void SelectVertex(Vertex _vertex, bool _addToOthers = false)
    {
        if (!_addToOthers)
        {
            foreach (Vertex _sVertex in selectedVertices)
            {
                _sVertex.DeSelect();
            }

            selectedVertices.Clear();
            selectedVertices.Add(_vertex);
            _vertex.Select();

            Transform _trans = _vertex.transform;
            Vector3 _pos = _trans.position;

            moveTool.SetObjects(new[] {_trans});
            moveTool.SetPos(_pos);
        }
        else
        {
            selectedVertices.Add(_vertex);
            _vertex.Select();

            Vertex[] _vertices = selectedVertices.ToArray();
            List<Transform> _transforms = new List<Transform> {_vertices[0].transform};
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

    private void DeSelectVertex(Vertex _vertex)
    {
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

    private void DeselectAllVertices()
    {
        moveTool.Hide();
        foreach (Vertex _vertex in selectedVertices)
        {
            _vertex.DeSelect();
        }
        selectedVertices.Clear();
    }

    public bool IsVertexSelected(Vertex _vertex)
    {
        return selectedVertices.Contains(_vertex);
    }

    public Vertex GetVertex(Guid _guid)
    {
        return vertices[_guid];
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

    private void CreateTriangle()
    {
        
    }
    
    private void SelectTriangle()
    {
        
    }
    
    #endregion

    #region Serialization&Desirialization

    public string ToJson()
    {
        var _verticesValues = vertices.Values.ToArray();
        Vector3[] verticesArray = new Vector3[vertices.Count];
        for (int i = 0; i < verticesArray.Length; i++)
        {
            verticesArray[i] = _verticesValues[i].transform.position;
        }
    
        return JsonUtility.ToJson(new MeshData(verticesArray));
    }

    private struct MeshData
    {
        public Vector3[] vertices;

        public MeshData(Vector3[] _vertices)
        {
            vertices = _vertices;
        }
    }

    #endregion
}