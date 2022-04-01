using System.IO;
using TMPro;
using UI.Automation;
using UnityEngine;

public class FileDialog : MonoBehaviour
{
    public static FileDialog instance;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject fixedDrivePrefab;
    [SerializeField] private GameObject removableDrivePrefab;
    [SerializeField] private GameObject unspecifiedDrivePrefab;
    [SerializeField] private GameObject folderPrefab;
    [SerializeField] private GameObject filePrefab;

    [Header("References")]
    [SerializeField] private TMP_InputField pathInput;
    [SerializeField] private TMP_InputField fileNameInput;
    [SerializeField] private Transform content;
    [SerializeField] private AutoContentSizeGridLayout autoSize;
    
    private GameObject uiGO;
    private string currentPath;
    private string tempPath;
    private string saveString;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        
        uiGO = transform.GetChild(0).gameObject;
    }

    #region Events

    public void OnSelectPathInput(string _value)
    {
        tempPath = _value;
    }
    
    public void OnEndEditPathInput(string _value)
    {
        if (_value == currentPath) return;
        if (_value == tempPath) return;
        
        if (_value == "")
        {
            LoadContent();
        }
        else if (!Directory.Exists(_value))
        {
            SetCurrentPath(tempPath);
        }
        else
        {
            SetCurrentPath(_value.Replace("\\\\", "\\"));
            LoadContent(currentPath);
        }
    }

    #endregion
    
    private void SetCurrentPath(string _path)
    {
        currentPath = _path;
        pathInput.text = currentPath;
    }

    public void Close()
    {
        uiGO.SetActive(false);
    }

    public void Open()
    {
        uiGO.SetActive(true);
        LoadContent();
    }

    public void BackFolder()
    {
        if (currentPath == "") return;
        
        DirectoryInfo _parentDir = Directory.GetParent(currentPath.EndsWith("\\") ? currentPath : string.Concat(currentPath, "\\"));

        if (_parentDir == null)
        {
            SetCurrentPath("");
            LoadContent();
        }
        else
        {
            SetCurrentPath(_parentDir.Parent.FullName);
            LoadContent(currentPath);
        }
    }

    private void Open(string _path)
    {
        uiGO.SetActive(true);
        LoadContent(_path);
    }

    private void ClearContent()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    private void LoadContent()
    {
        ClearContent();
         DriveInfo[] _drives = DriveInfo.GetDrives();
         foreach (DriveInfo _drive in _drives)
         {
             switch (_drive.DriveType)
             {
                 case DriveType.Fixed:
                     CreateFixedDrive(_drive.Name);
                     break;
                 case DriveType.Removable:
                     CreateRemovableDrive(_drive.Name);
                     break;
                 default:
                     CreateUnspecifiedDrive(_drive.Name);
                     break;
             }
         }
         autoSize.CorrectSize();
    }
    
    private void LoadContent(string _path)
    {
        ClearContent();
        _path = _path.Replace("\\", "\\\\");
        SetCurrentPath(_path);
        DirectoryInfo _rootDirectory = new DirectoryInfo(_path);
        DirectoryInfo[] _directories = _rootDirectory.GetDirectories();
        FileInfo[] _files = _rootDirectory.GetFiles();
        
        foreach (DirectoryInfo _directory in _directories)
        {
            CreateFolder(_directory.FullName, _directory.Name);
        }

        foreach (FileInfo _file in _files)
        {
            CreateFile(_file.Name);
        }
        autoSize.CorrectSize();
    }
    
    private void CreateFixedDrive(string _name)
    {
        GameObject _drive = Instantiate(fixedDrivePrefab, content);
        _drive.GetComponent<DriveUI>().Populate(_name, Open);
    }
    
    private void CreateRemovableDrive(string _name)
    {
        GameObject _drive = Instantiate(removableDrivePrefab, content);
        _drive.GetComponent<DriveUI>().Populate(_name, Open);
    }
    
    private void CreateUnspecifiedDrive(string _name)
    {
        GameObject _drive = Instantiate(unspecifiedDrivePrefab, content);
        _drive.GetComponent<DriveUI>().Populate(_name, Open);
    }

    private void CreateFolder(string _fullPath, string _name)
    {
        GameObject _folder = Instantiate(folderPrefab, content);
        _folder.GetComponent<FolderUI>().Populate(_fullPath, _name, Open);
    }

    private void CreateFile(string _name)
    {
        GameObject _file = Instantiate(filePrefab, content);
        _file.GetComponent<FileUI>().Populate(_name);
    }

    public void SaveString(string _content)
    {
        Open();
        saveString = _content;
    }

    public void Save()
    {
        string path = Path.Join(currentPath, fileNameInput.text);

        if (File.Exists(path))
        {
            // TODO: Ask the user if he wants to override the already existing file or cancel
        }
        
        using StreamWriter _writer = new StreamWriter(path);
        _writer.Write(saveString);
        Close();
    }
}