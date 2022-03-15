using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods
{
    public static Transform[] RemoveIndex(this Transform[] _array, int _removeIndex)
    {
        return _array.Where((_trans, _index) => _index != _removeIndex).ToArray();
    }
}