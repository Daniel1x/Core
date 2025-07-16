namespace DL.Scriptables
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "string_New", menuName = CoreData.SCRIPTABLE_CREATION_TAB_NAME + "String Variable")]
    public class StringVariable : ScriptableData<string> { }
}