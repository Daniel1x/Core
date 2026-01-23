namespace DL.Behaviours
{
    using UnityEngine;
    using UnityEngine.Events;

    public class MonoBehaviourEventCaller : MonoBehaviour, IMonoBehaviourEventCaller<MonoBehaviourEventCaller>
    {
        public event UnityAction<MonoBehaviourEventCaller> OnBehaviourEnabled = null;
        public event UnityAction<MonoBehaviourEventCaller> OnBehaviourDisabled = null;
        public event UnityAction<MonoBehaviourEventCaller> OnBehaviourDestroyed = null;

        protected virtual void OnEnable()
        {
            OnBehaviourEnabled?.Invoke(this);
        }

        protected virtual void OnDisable()
        {
            OnBehaviourDisabled?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            OnBehaviourDestroyed?.Invoke(this);
        }
    }
}