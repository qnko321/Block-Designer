using UI.Automation;
using UnityEngine;

public class UpdateAutoSizersInOrder : MonoBehaviour
{
    [SerializeField] private bool correctSizesOnStart;
    [SerializeField] private AutoSizer[] autoSizers;

    private void Start()
    {
        if (correctSizesOnStart)
            CorrectSizes();
    }

    public void CorrectSizes()
    {
        foreach (AutoSizer _sizer in autoSizers)
        {
            _sizer.CorrectSize();
        }
    }
}
