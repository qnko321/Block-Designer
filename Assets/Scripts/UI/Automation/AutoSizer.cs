using System;
using UnityEngine;

namespace UI.Automation
{
    public abstract class AutoSizer : MonoBehaviour
    {
        [SerializeField] private bool correctOnAwake;
        [SerializeField] protected bool dynamicChildSize;
        [SerializeField] protected float childHeight;

        private void Awake()
        {
            if (correctOnAwake)
            {
                CorrectSize();
            }
        }

        public abstract void CorrectSize();
        protected abstract float CalculateHeight();
        protected abstract float CalculateDynamicHeight();
    }
}