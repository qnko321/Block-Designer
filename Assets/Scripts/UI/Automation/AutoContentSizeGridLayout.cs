using UnityEngine;
using UnityEngine.UI;

namespace UI.Automation
{
    public class AutoContentSizeGridLayout : MonoBehaviour
    {
        private RectTransform rTrans;
        private GridLayoutGroup layoutGroup;
        Transform trans;

        private void Awake()
        {
            rTrans = (RectTransform) transform;
            layoutGroup = GetComponent<GridLayoutGroup>();
            //CorrectSize();
        }

        public void CorrectSize()
        {
            rTrans.sizeDelta = new Vector2(0, CalculateHeight());
        }
    
        public void CorrectSize(int _subtract)
        {
            rTrans.sizeDelta = new Vector2(0, CalculateHeight(_subtract));
        }

        private float CalculateHeight()
        {
            //Debug.Log("Width: " + rTrans.sizeDelta.x);
            //Debug.Log("Cell size + spacing: " + (layoutGroup.cellSize.x + layoutGroup.spacing.x));
            //Debug.Log(Mathf.CeilToInt(rTrans.sizeDelta.x / (layoutGroup.cellSize.x + layoutGroup.spacing.x)));
            int _childrenOnOneRow = Mathf.CeilToInt(rTrans.rect.width / (layoutGroup.cellSize.x + layoutGroup.spacing.x));
            int _rowCount = Mathf.CeilToInt((float)rTrans.childCount / _childrenOnOneRow);
            
            return _rowCount * layoutGroup.cellSize.y + (_rowCount - 1) * layoutGroup.spacing.y;
        }

        private float CalculateHeight(int _subtract)
        {
            int _childrenOnOneRow = (rTrans.childCount - _subtract) / Mathf.RoundToInt(rTrans.sizeDelta.x / (layoutGroup.cellSize.x + layoutGroup.spacing.x));
            int _rowCount = Mathf.CeilToInt(((float)rTrans.childCount - _subtract) / _childrenOnOneRow);
            
            return _rowCount * layoutGroup.cellSize.y + (_rowCount - 1) * layoutGroup.spacing.y;
        }
    }
}
