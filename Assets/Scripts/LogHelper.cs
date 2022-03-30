using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static System.String;

public static class LogHelper
{
    public static void LogArray<T>(IEnumerable<T> _arr, string _name = "")
    {
        if (_name.Length > 0)
        {
            if (_name[0] == '_')
                _name = _name.Substring(1);
            _name = char.ToUpper(_name[0]) + _name.Substring(1);
        }
        
        string _log;
        if (!IsNullOrEmpty(_name))
            _log = _arr.Aggregate(_name + ": [ ", (_current, _item) => _current + (_item + ", "));
        else
            _log = _arr.Aggregate("[ ", (_current, _item) => _current + (_item + ", "));

        _log = _log[..^2];
        _log += " ]";
        
        Debug.Log(_log);
    }

    public static void LogDictionary<TKey, TValue>(Dictionary<TKey, TValue> _dict, string _name = "", bool _multiLine = true)
    {
        if (_name.Length > 0)
        {
            if (_name[0] == '_')
                _name = _name.Substring(1);
            _name = char.ToUpper(_name[0]) + _name.Substring(1);
        }

        string _log;
        if (!IsNullOrEmpty(_name))
            _log = _name + ": [ ";
        else
            _log = "[ ";
        
        _log = _dict.Aggregate(_log, (_current, _item) => _current + "{" + _item.Key + ": " + _item.Value + "}" + (_multiLine ? "\n" : ", "));
        _log = _log[..^2];
        _log += " ]";
        
        Debug.Log(_log);
    }
}