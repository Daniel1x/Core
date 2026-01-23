namespace DL.Behaviours
{
    using UnityEngine;
    using UnityEngine.Events;

    public class OnEnableEventCaller : MonoBehaviour, IOnEnableEventCaller<OnEnableEventCaller>
    {
        public event UnityAction<OnEnableEventCaller> OnBehaviourEnabled = null;

        protected virtual void OnEnable()
        {
            OnBehaviourEnabled?.Invoke(this);
        }
    }

}