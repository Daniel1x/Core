using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ReadOnlyProperty : PropertyAttribute
{
    public bool DisableScope = true;

    public ReadOnlyProperty(bool _disableScope = true)
    {
        DisableScope = _disableScope;
    }
}