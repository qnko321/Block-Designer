using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Triangles
{
    public class TriangleTest : MonoBehaviour
    {
        private Guid guid;
        public List<string> vertices;
        [SerializeField] private int next;

        private void Start()
        {
            vertices.Add("");
            vertices.Add("");
            vertices.Add("");
            StartCoroutine(TestSelecting());
        }

        private IEnumerator TestSelecting()
        {
            SelectVertex(Guid.NewGuid());
            yield return new WaitForSeconds(2);
            StartCoroutine(TestSelecting());
        }
        
        public void SelectVertex(Guid _guid)
        {
            // "" = Guid.Empty 
            // _guid.ToString() = _guid
            if (vertices[0] == "")
            {
                vertices[0] = _guid.ToString();
            } 
            else
            {
                if (vertices[1] == "")
                {
                    vertices[1] = _guid.ToString();
                }
                else
                {
                    if (vertices[2] == "")
                    {
                        vertices[2] = _guid.ToString();
                    }
                    else
                    {
                        vertices[next] = _guid.ToString();
                        next++;
                        if (next >= 2)
                            next = 0;
                    }
                }
            }
        }
    }
}