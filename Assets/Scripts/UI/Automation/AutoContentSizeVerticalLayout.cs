using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Automation
{   
    [ExecuteInEditMode]
    public class AutoContentSizeVerticalLayout : AutoSizer
    {
        private VerticalLayoutGroup layoutGroup;
        private RectTransform rTrans;

        private void Awake()
        {
            rTrans = (RectTransform) transform;
            layoutGroup = GetComponent<VerticalLayoutGroup>();
        }

        private void Update()
        {
            CorrectSize();
        }

        public override void CorrectSize()
        {
            rTrans.sizeDelta = new Vector2(rTrans.sizeDelta.x, dynamicChildSize ? CalculateDynamicHeight() : CalculateHeight());
        }
    
        public void CorrectSize(int _subtract)
        {
            rTrans.sizeDelta = new Vector2(rTrans.sizeDelta.x, CalculateHeight(_subtract));
        }

        protected override float CalculateHeight()
        {
            int _childCount = transform.childCount;
            if (_childCount <= 0)
                return 0;
        
            return _childCount * childHeight + (_childCount - 1) * layoutGroup.spacing;
        }
        
        protected override float CalculateDynamicHeight()
        {
            int _childCount = transform.childCount;
            if (_childCount <= 0)
                return 0;

            int _height = 0; 
            for (int i = 0; i < _childCount; i++)
            {
                _height += Mathf.RoundToInt(((RectTransform) rTrans.GetChild(i).transform).sizeDelta.y);
            }
        
            return _height + (_childCount - 1) * layoutGroup.spacing;
        }

        private float CalculateHeight(int _subtract)
        {
            int _childCount = transform.childCount - _subtract;
            if (_childCount <= 0)
                return 0;
        
            return _childCount * childHeight + (_childCount - 1) * layoutGroup.spacing;
        }
    }
}
