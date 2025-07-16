namespace DL.Scriptables
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "curve_New", menuName = CoreData.SCRIPTABLE_CREATION_TAB_NAME + "Animation Curve Variable")]
    public class CurveVariable : ScriptableData<AnimationCurve> { }
}
