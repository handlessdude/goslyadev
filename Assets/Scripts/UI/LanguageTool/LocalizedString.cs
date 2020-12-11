using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//not written by goslyadev
[System.Serializable]
public struct LocalizedString
{
    public string key;

    public LocalizedString(string key)
    {
        this.key = key;
    }

    public string value
    {
        get
        {
            return key;
        }
    }

    public static implicit operator LocalizedString(string key)
    {
        return new LocalizedString(key);
    }
}
