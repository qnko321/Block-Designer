using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DriveUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TMP_Text title;

    private Action<string> openCallback;
    private DateTime firstClickTime = DateTime.UtcNow;
    
    public void Populate(string _title, Action<string> _openCallback)
    {
        title.text = _title;
        openCallback = _openCallback;
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        if ((firstClickTime.Ticks - DateTime.UtcNow.Ticks) / 10_000_000f > Constants.DOUBLE_CLICK_TIME)
            firstClickTime = DateTime.Now;
        else
            openCallback(title.text);
    }
}