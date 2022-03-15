using System;
using UnityEngine;

namespace Vertices
{
    public class Vertex : MonoBehaviour
    {
        public Guid guid;

        private Outline outline;

        private void Awake()
        {
            outline = GetComponent<Outline>();
        }

        public void Select()
        {
            outline.enabled = true;
        }

        public void DeSelect()
        {
            outline.enabled = false;
        }
    }
}