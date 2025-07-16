using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class SelectableExplicitNavigator
{
    [SerializeField] public bool AddNewExplicitSelections = false;
    [SerializeField] public bool UseAutomaticSelectionIfExplicitSelectionIsNull = true;
    [SerializeField] private Selectable[] selectOnUp = new Selectable[] { };
    [SerializeField] private Selectable[] selectOnDown = new Selectable[] { };
    [SerializeField] private Selectable[] selectOnLeft = new Selectable[] { };
    [SerializeField] private Selectable[] selectOnRight = new Selectable[] { };

    public Selectable GetExplicitSelection(MoveDirection _moveDirection)
    {
        return _moveDirection switch
        {
            MoveDirection.Left => getFirstActiveAndInteractableSelectable(selectOnLeft),
            MoveDirection.Up => getFirstActiveAndInteractableSelectable(selectOnUp),
            MoveDirection.Right => getFirstActiveAndInteractableSelectable(selectOnRight),
            MoveDirection.Down => getFirstActiveAndInteractableSelectable(selectOnDown),
            _ => null,
        };
    }

    private Selectable getFirstActiveAndInteractableSelectable(Selectable[] _selectables)
    {
        foreach (Selectable _selectable in _selectables)
        {
            if (_selectable == null
                || _selectable.interactable == false
                || _selectable.gameObject.activeSelf == false)
            {
                continue;
            }

            return _selectable;
        }

        return null;
    }
}
