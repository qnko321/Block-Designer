using UnityEngine;
using UnityEngine.UI;

public class AutoContentSize : MonoBehaviour
{
    [SerializeField] private float childrenHeight;
    
    private RectTransform rTrans;
    private VerticalLayoutGroup layoutGroup;

    private void Awake()
    {
        rTrans = (RectTransform) transform;
        layoutGroup = GetComponent<VerticalLayoutGroup>();
        CorrectSize();
    }

    public void CorrectSize()
    {
        rTrans.sizeDelta = new Vector2(0, CalculateHeight());
    }

    private float CalculateHeight()
    {
        var _childCount = transform.childCount;
        return _childCount * childrenHeight + (_childCount - 1) * layoutGroup.spacing;
    }
}
