using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class ActionButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var methods = target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttributes(typeof(ActionButtonAttribute), true).Length > 0
                        && m.GetParameters().Length == 0);

        foreach (var method in methods)
        {
            ActionButtonAttribute _attr = (ActionButtonAttribute)method.GetCustomAttributes(typeof(ActionButtonAttribute), true)[0];

            if (_attr.RuntimeOnly && Application.isPlaying == false)
            {
                continue;
            }

            string _buttonName = string.IsNullOrEmpty(_attr.ButtonName)
                ? method.Name
                : _attr.ButtonName;

            if (GUILayout.Button(_buttonName))
            {
                method.Invoke(target, null);
            }
        }
    }
}