using System;
using System.Collections.Generic;
using Triangles;
using Vertices;

public class Face
{
    private Dictionary<string, Vertex> vertices = new();
    private Dictionary<string, Triangle> triangles = new();

    public void CreateVertex(Guid _guid, Vertex _vertex)
    {
        vertices.Add(_guid, _vertex);
    }

    public void CreateTriangle(Guid _guid)
    {
        triangles.Add(_guid, new Triangle());
    }

    public void SelectVertex(Guid _trianglesGuidGuid, Guid _vertexGuid)
    {
        triangles[_trianglesGuidGuid].SelectVertex(_vertexGuid);
    }

    public void DeleteTriangle(Guid _guid)
    {
        triangles.Remove(_guid);
    }

    public void DeleteVertex(string _id)
    {
        vertices[_id].Delete();
        vertices.Remove(_id);
    }
}