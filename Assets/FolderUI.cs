using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FolderUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TMP_Text title;

    private string fullPath;
    private Action<string> openCallback;
    private DateTime firstClickTime = DateTime.UtcNow;

    public void Populate(string _fullPath, string _title, Action<string> _openCallback)
    {
        fullPath = _fullPath;
        title.text = _title;
        openCallback = _openCallback;
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        if ((firstClickTime.Ticks - DateTime.UtcNow.Ticks) / 10_000_000f > Constants.DOUBLE_CLICK_TIME)
            firstClickTime = DateTime.Now;
        else
            openCallback(fullPath);
    }
}
