namespace DL.Scriptables
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "colorHDR_New", menuName = CoreData.SCRIPTABLE_CREATION_TAB_NAME + "Color HDR Variable")]
    public class ColorHDRVariable : ScriptableDataBase<Color>
    {
        [ColorUsage(true, true)] public Color Value = default;

        public override Color GetValue() => Value;
    }
}
