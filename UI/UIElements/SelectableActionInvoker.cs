using UnityEngine;
using UnityEngine.UI;

public class SelectableActionInvoker : MonoBehaviour
{
    public enum ExecutionMode
    {
        OnEnable = 0,
        OnDisable = 1,
    }

    public enum ActionType
    {
        Select = 0,
    }

    [System.Serializable]
    public class SelectableAction
    {
        [SerializeField] private ActionType actionType = ActionType.Select;
        [SerializeField] private Selectable selectable = null;

        public void Execute()
        {
            switch (actionType)
            {
                case ActionType.Select:

                    if (selectable != null)
                    {
                        selectable.Select();
                    }

                    break;
            }
        }
    }

    [System.Serializable]
    public class ActionSettings
    {
        [SerializeField] private ExecutionMode executionMode = ExecutionMode.OnEnable;
        [SerializeField] private SelectableAction[] actions = new SelectableAction[] { };

        public void Execute(ExecutionMode _executionMode)
        {
            if (executionMode != _executionMode)
            {
                return;
            }

            foreach (SelectableAction _action in actions)
            {
                _action.Execute();
            }
        }
    }

    [SerializeField] private ActionSettings[] actionSettings = new ActionSettings[] { };

    private void OnEnable()
    {
        execute(ExecutionMode.OnEnable);
    }

    private void OnDisable()
    {
        execute(ExecutionMode.OnDisable);
    }

    private void execute(ExecutionMode _executionMode)
    {
        foreach (ActionSettings _actionSetting in actionSettings)
        {
            _actionSetting.Execute(_executionMode);
        }
    }
}
