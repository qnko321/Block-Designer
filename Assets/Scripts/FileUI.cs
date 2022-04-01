using TMPro;
using UnityEngine;

public class FileUI : MonoBehaviour
{
    [SerializeField] private TMP_Text title;

    public void Populate(string _title)
    {
        title.text = _title;
    }
}