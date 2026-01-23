namespace DL.Behaviours
{
    using UnityEngine;
    using UnityEngine.Events;

    public interface IOnDestroyEventCaller<T> where T : MonoBehaviour
    {
        event UnityAction<T> OnBehaviourDestroyed;
    }

}