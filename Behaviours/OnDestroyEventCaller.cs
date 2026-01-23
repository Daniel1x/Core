namespace DL.Behaviours
{
    using UnityEngine;
    using UnityEngine.Events;

    public class OnDestroyEventCaller : MonoBehaviour, IOnDestroyEventCaller<OnDestroyEventCaller>
    {
        public event UnityAction<OnDestroyEventCaller> OnBehaviourDestroyed = null;

        protected virtual void OnDestroy()
        {
            OnBehaviourDestroyed?.Invoke(this);
        }
    }

}