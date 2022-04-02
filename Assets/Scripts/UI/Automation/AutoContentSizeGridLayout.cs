using UnityEngine;
using UnityEngine.UI;

namespace UI.Automation
{
    public class AutoContentSizeGridLayout : AutoSizer
    {
        private RectTransform rTrans;
        private GridLayoutGroup layoutGroup;
        Transform trans;

        private void Awake()
        {
            rTrans = (RectTransform) transform;
            layoutGroup = GetComponent<GridLayoutGroup>();
        }

        public override void CorrectSize()
        {
            rTrans.sizeDelta = new Vector2(rTrans.sizeDelta.x,  dynamicChildSize ? CalculateHeight() : CalculateDynamicHeight());
        }

        protected override float CalculateHeight()
        {
            //Debug.Log("Width: " + rTrans.sizeDelta.x);
            //Debug.Log("Cell size + spacing: " + (layoutGroup.cellSize.x + layoutGroup.spacing.x));
            //Debug.Log(Mathf.CeilToInt(rTrans.sizeDelta.x / (layoutGroup.cellSize.x + layoutGroup.spacing.x)));
            int _childrenOnOneRow = Mathf.CeilToInt(rTrans.rect.width / (layoutGroup.cellSize.x + layoutGroup.spacing.x));
            int _rowCount = Mathf.CeilToInt((float)rTrans.childCount / _childrenOnOneRow);
            
            return _rowCount * layoutGroup.cellSize.y + (_rowCount - 1) * layoutGroup.spacing.y;
        }

        protected override float CalculateDynamicHeight()
        {
            return 0;
        }
    }
}
