namespace DL.Scriptables
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "vector4Array_New", menuName = CoreData.SCRIPTABLE_CREATION_TAB_NAME + "Vector4 Array Variable")]
    public class Vector4ArrayVariable : ScriptableData<Vector4[]> { }
}
