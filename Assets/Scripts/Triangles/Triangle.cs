using System;

namespace Triangles
{
    public class Triangle
    {
        public Guid guid;
        private Guid[] vertices = new Guid[3];
        private int next;
        public string triName;

        public Triangle(Guid _guid)
        {
            guid = _guid;
        }

        public void SelectVertex(Guid _guid)
        {
            if (vertices[0] == _guid || vertices[1] == _guid || vertices[2] == _guid)
            {
                vertices = new Guid[3];
            }
            else if (vertices[0] == Guid.Empty)
            {
                vertices[0] = _guid;
            } 
            else if (vertices[1] == Guid.Empty)
            {
                vertices[1] = _guid;
            }
            else if (vertices[2] == Guid.Empty)
            {
                vertices[2] = _guid;
            }
            else
            {
                vertices[next] = _guid;
                next++;
                if (next >= 3)
                    next = 0;
            }

            //Debug.Log($"{vertices[0].ToString()}, {vertices[1].ToString()}, {vertices[2].ToString()}");
        }

        public Guid[] GetVertices()
        {
            return vertices;
        }
    }
}