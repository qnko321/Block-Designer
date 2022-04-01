using UnityEngine;
using UnityEngine.UI;

namespace UI.Automation
{
    public class AutoContentSizeHorizontalLayout : MonoBehaviour
    {
        [SerializeField] private float childrenHeight;
    
        private RectTransform rTrans;
        private HorizontalLayoutGroup layoutGroup;

        private void Awake()
        {
            rTrans = (RectTransform) transform;
            layoutGroup = GetComponent<HorizontalLayoutGroup>();
            CorrectSize();
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
            int _childCount = transform.childCount;
            if (_childCount <= 0)
                return 0;
        
            return _childCount * childrenHeight + (_childCount - 1) * layoutGroup.spacing;
        }

        private float CalculateHeight(int _subtract)
        {
            int _childCount = transform.childCount - _subtract;
            if (_childCount <= 0)
                return 0;
        
            return _childCount * childrenHeight + (_childCount - 1) * layoutGroup.spacing;
        }
    }
}
