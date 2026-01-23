namespace DL.Behaviours
{
    using UnityEngine;

    public interface IMonoBehaviourEventCaller<T> : IOnEnableEventCaller<T>, IOnDisableEventCaller<T>, IOnDestroyEventCaller<T> where T : MonoBehaviour { }

}