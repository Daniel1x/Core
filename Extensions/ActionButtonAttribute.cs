using UnityEngine;

// This attribute can be used to create a button in the Unity Inspector that invokes a method on the target object.
// It can be applied only to methods with no parameters.
[System.AttributeUsage(System.AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class ActionButtonAttribute : PropertyAttribute
{
    public string ButtonName { get; private set; }

    public ActionButtonAttribute(string buttonName = null)
    {
        ButtonName = buttonName;
    }
}
