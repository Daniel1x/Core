namespace DL.Scriptables
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "materialArray_New", menuName = CoreData.SCRIPTABLE_CREATION_TAB_NAME + "Material Array Variable")]
    public class MaterialArrayVariable : ScriptableData<Material[]> { }
}
