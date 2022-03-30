using System;
using TMPro;
using UnityEngine;

namespace Vertices
{
    public class Vertex : MonoBehaviour
    {
        public Guid guid;

        private Outline outline;
        public string vertName = "Vertex";
        public Vector3 Position => transform.position;
        
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

        public void Delete()
        {
            Destroy(gameObject);
        }
    }
}