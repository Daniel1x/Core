namespace DL.Scriptables
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "texture_New", menuName = CoreData.SCRIPTABLE_CREATION_TAB_NAME + "Texture Variable")]
    public class TextureVariable : ScriptableData<Texture> { }
}