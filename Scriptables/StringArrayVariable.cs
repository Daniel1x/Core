namespace DL.Scriptables
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "stringArray_New", menuName = CoreData.SCRIPTABLE_CREATION_TAB_NAME + "String Array Variable")]
    public class StringArrayVariable : ScriptableData<string[]> { }
}
