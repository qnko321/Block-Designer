using System;

namespace Triangles
{
    public class Triangle
    {
        private Guid guid;
        private Guid[] vertices = new Guid[3];
        private int next = -1;

        public void SelectVertex(Guid _guid)
        {
            if (vertices[0] == Guid.Empty)
            {
                vertices[0] = _guid;
            } 
            else
            {
                if (vertices[1] == Guid.Empty)
                {
                    vertices[1] = _guid;
                }
                else
                {
                    if (vertices[2] == Guid.Empty)
                    {
                        vertices[2] = _guid;
                    }
                    else
                    {
                        vertices[next++] = _guid;
                    }
                }
            }
        }
    }
}