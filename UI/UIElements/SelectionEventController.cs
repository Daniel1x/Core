using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class SelectionEventController
{
    [System.Flags]
    public enum SelectionEventMode
    {
        None = 0,
        ObjectActive = 1,
        BehaviourEnabled = 2,
        ObjectDisabled = 4,
        BehaviourDisabled = 8
    }

    public event UnityAction<bool> OnSelectionUpdated = null;

    [SerializeField] private SelectionEventMode selectionEventMode = SelectionEventMode.None;
    [SerializeField] private GameObject[] objectsToEnableOnSelect = new GameObject[] { };
    [SerializeField] private Behaviour[] behavioursToEnableOnSelect = new Behaviour[] { };
    [SerializeField] private GameObject[] objectsToDisableOnSelect = new GameObject[] { };
    [SerializeField] private Behaviour[] behavioursToDisableOnSelect = new Behaviour[] { };

    private bool isSelected = false;

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            UpdateSelectionState();

            OnSelectionUpdated?.Invoke(isSelected);
        }
    }

    public void UpdateIsSelectedOnEnable<T>(T _selectable) where T : Selectable, INavigationItem
    {
        IsSelected = _selectable.IsObjectSelectedByEventSystem();

        if (IsSelected && _selectable.ForceSelectEventOnEnable)
        {
            _selectable.SelectInProgressTriggeredFromOnEnable = true;
            _selectable.OnSelect(null);
            _selectable.SelectInProgressTriggeredFromOnEnable = false;
        }
    }

    public void TryToForceOnDeselect<T>(T _selectable) where T : Selectable, INavigationItem
    {
        if (IsSelected && _selectable.ForceDeselectEventOnDisable)
        {
            _selectable.OnDeselect(null);
        }
    }

    public void UpdateSelectionState()
    {
        if (selectionEventMode == SelectionEventMode.None)
        {
            return;
        }

        if (isModeActive(SelectionEventMode.ObjectActive))
        {
            setObjectsEnabled(objectsToEnableOnSelect, isSelected);
        }

        if (isModeActive(SelectionEventMode.BehaviourEnabled))
        {
            setBehavioursEnabled(behavioursToEnableOnSelect, isSelected);
        }

        if (isModeActive(SelectionEventMode.ObjectDisabled))
        {
            setObjectsEnabled(objectsToDisableOnSelect, !isSelected);
        }

        if (isModeActive(SelectionEventMode.BehaviourDisabled))
        {
            setBehavioursEnabled(behavioursToDisableOnSelect, !isSelected);
        }
    }

    private bool isModeActive(SelectionEventMode _event)
    {
        return (selectionEventMode & _event) != 0;
    }

    private void setBehavioursEnabled(Behaviour[] _behaviours, bool _enabled)
    {
        foreach (Behaviour _behaviour in _behaviours)
        {
            if (_behaviour == null)
            {
                continue;
            }

            if (_behaviour.enabled != _enabled)
            {
                _behaviour.enabled = _enabled;
            }
        }
    }

    private void setObjectsEnabled(GameObject[] _objects, bool _enabled)
    {
        foreach (GameObject _gameObject in _objects)
        {
            if (_gameObject == null)
            {
                continue;
            }

            if (_gameObject.activeSelf != _enabled)
            {
                _gameObject.SetActive(_enabled);
            }
        }
    }
}
