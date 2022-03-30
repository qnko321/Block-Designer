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
    [SerializeField] private MeshRenderer meshRenderer;
    
    public readonly Dictionary<Guid, Vertex> vertices = new();
    public readonly List<Vertex> selectedVertices = new();

    public readonly Dictionary<Guid, Triangle> triangles = new();
    public Triangle selectedTriangle;

    private bool LeftControlPressed =>
        Math.Abs(leftControlInput.ToInputAction().ReadValue<float>() - 1) < float.Epsilon;

    #region Events

    public void OnDeleteSelected()
    {
        foreach (Vertex _vertex in selectedVertices)
        {
            _vertex.Delete();
            vertices.Remove(_vertex.guid);
        }
        selectedVertices.Clear();

        if (selectedTriangle != null)
        {
            triangles.Remove(selectedTriangle.guid);
            selectedTriangle = null;
        }
        
        ReRenderMesh();
    }
    
    #region Vertices
    /*public void OnVertexSelect(Guid _guid)
        {
            if (selectedTriangle != null)
            {
                selectedTriangle.SelectVertex(_guid);
                ReRenderMesh();
            }
            else
            {
                SelectVertex(vertices[_guid]);
            }
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
        }*/
    #endregion

    #region Triangles

    /*public void OnCreateTriangle(Guid _guid)
    {
        CreateTriangle(_guid);
    }

    public void OnTriangleSelect(Guid _guid)
    {
        SelectTriangle(_guid);
    }

    public void OnTriangleDeSelect(Guid _guid)
    {
        DeSelectTriangle(_guid);
    }*/

    #endregion

    #endregion

    #region Vertices

    public void CreateVertex(Guid _guid)
    {
        Vertex _vertex = Instantiate(vertexPrefab, transform).GetComponent<Vertex>();
        _vertex.guid = _guid;
        vertices.Add(_guid, _vertex);
        SelectVertex(_guid);
    }

    public void SelectVertex(Guid _guid)
    {
        if (selectedTriangle != null)
        {
            selectedTriangle.SelectVertex(_guid);
            ReRenderMesh();
            return;
        }
        
        Vertex _vertex = vertices[_guid];
        if (!LeftControlPressed)
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

    public void DeSelectAllVertices()
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

    public void CreateTriangle(Guid _guid)
    {
        Triangle _triangle = new Triangle(_guid);
        triangles.Add(_guid, _triangle);
        SelectTriangle(_guid);
    }

    public void SelectTriangle(Guid _guid)
    {
        selectedTriangle = triangles[_guid];
    }

    public void DeSelectTriangle(Guid _guid)
    {
        selectedTriangle = null;
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

    public void ToJson()
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

        string _json =
            JsonUtility.ToJson(new MeshInfo(_vertices.ToArray(), _triangles.ToArray(), _uvs.ToArray()), true);
        Debug.Log(_json);
        //return _json;
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
}