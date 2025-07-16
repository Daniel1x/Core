namespace DL.Scriptables
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "layerMask_New", menuName = CoreData.SCRIPTABLE_CREATION_TAB_NAME + "LayerMask Variable")]
    public class LayerMaskVariable : ScriptableData<LayerMask>
    {
        public int Mask => Value.value;

        public bool Contains(int _layer)
        {
            return Value == (Value | (1 << _layer));
        }
    }
}
