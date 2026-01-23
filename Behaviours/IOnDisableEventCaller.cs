namespace DL.Behaviours
{
    using UnityEngine;
    using UnityEngine.Events;

    public interface IOnDisableEventCaller<T> where T : MonoBehaviour
    {
        event UnityAction<T> OnBehaviourDisabled;
    }

}