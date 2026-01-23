namespace DL.Behaviours
{
    using UnityEngine;
    using UnityEngine.Events;

    public interface IOnEnableEventCaller<T> where T : MonoBehaviour
    {
        event UnityAction<T> OnBehaviourEnabled;
    }

}