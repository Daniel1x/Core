using UnityEngine;

// This attribute can be used to create a button in the Unity Inspector that invokes a method on the target object.
// It can be applied only to methods with no parameters.
[System.AttributeUsage(System.AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class ActionButtonAttribute : PropertyAttribute
{
    public string ButtonName { get; private set; }
    public bool RuntimeOnly { get; private set; }

    public ActionButtonAttribute(string _buttonName = null, bool _runtimeOnly = true)
    {
        ButtonName = _buttonName;
        RuntimeOnly = _runtimeOnly;
    }
}
