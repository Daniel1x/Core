namespace DL.Behaviours
{
    using UnityEngine;
    using UnityEngine.Events;

    public class OnDisableEventCaller : MonoBehaviour, IOnDisableEventCaller<OnDisableEventCaller>
    {
        public event UnityAction<OnDisableEventCaller> OnBehaviourDisabled = null;

        protected virtual void OnDisable()
        {
            OnBehaviourDisabled?.Invoke(this);
        }
    }

}