using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface INavigationItem<T> : INavigationItem where T : Selectable, INavigationItem
{
    public event UnityAction<int, T> OnItemSelected;
    public event UnityAction<int, T> OnItemDeselected;
    public event UnityAction<int, T> OnItemDestroyed;

    public int ItemIndex { get; set; }

    public T NavigationItem { get; }
}

public interface INavigationItem
{
    public static event UnityAction<INavigationItem> OnAnyItemSelected = null;

    public INavigationItemPart Owner { get; set; }

    bool InvokeOnAnyItemSelected { get; set; }
    bool AllowNavigationToOtherParent { get; set; }
    bool AllowNavigationToThisObject { get; set; }
    bool ForceSelectEventOnEnable { get; set; }
    bool ForceDeselectEventOnDisable { get; set; }
    bool ForceSelectOnPointerEnter { get; set; }
    bool ForceRefreshColorTransitionAfterFrame { get; set; }
    bool SelectInProgressTriggeredFromOnEnable { get; set; }

    SelectableExplicitNavigator ExplicitNavigator { get; }
    SelectionEventController SelectionEventController { get; }
    GraphicsTransitionController GraphicsTransitionController { get; }

    Selectable FindSelectableOnDown();
    Selectable FindSelectableOnLeft();
    Selectable FindSelectableOnRight();
    Selectable FindSelectableOnUp();
    Selectable FindClosestSelectable(bool _forcePermissionForOtherParent = false);
    void RefreshStateTransition(bool _instant = false);

    public virtual bool IsSelected => SelectionEventController.IsSelected;

    public static void OnNavigationItemSelected<T>(T _naviItem) where T : Selectable, INavigationItem
    {
        if (_naviItem != null && _naviItem.InvokeOnAnyItemSelected)
        {
            OnAnyItemSelected?.Invoke(_naviItem);
        }
    }

    public static void OnNavigationItemPointerEnter<T>(T _naviItem) where T : Selectable, INavigationItem
    {
        if (_naviItem != null && _naviItem.ForceSelectOnPointerEnter && _naviItem.interactable)
        {
            _naviItem.Select();
        }
    }

    public static Selectable FindSelectableOnDown<T>(T _naviItem) where T : Selectable, INavigationItem
    {
        return _naviItem.FindNextSelectable(MoveDirection.Down, _naviItem.navigation.mode, _naviItem.navigation.selectOnDown, _naviItem.ExplicitNavigator);
    }

    public static Selectable FindSelectableOnLeft<T>(T _naviItem) where T : Selectable, INavigationItem
    {
        return _naviItem.FindNextSelectable(MoveDirection.Left, _naviItem.navigation.mode, _naviItem.navigation.selectOnLeft, _naviItem.ExplicitNavigator);
    }

    public static Selectable FindSelectableOnRight<T>(T _naviItem) where T : Selectable, INavigationItem
    {
        return _naviItem.FindNextSelectable(MoveDirection.Right, _naviItem.navigation.mode, _naviItem.navigation.selectOnRight, _naviItem.ExplicitNavigator);
    }

    public static Selectable FindSelectableOnUp<T>(T _naviItem) where T : Selectable, INavigationItem
    {
        return _naviItem.FindNextSelectable(MoveDirection.Up, _naviItem.navigation.mode, _naviItem.navigation.selectOnUp, _naviItem.ExplicitNavigator);
    }

    public static Selectable FindClosestSelectable<T>(T _naviItem, bool _forcePermissionForOtherParent = false) where T : Selectable, INavigationItem
    {
        return SelectableNavigator.FindClosestSelectable(_naviItem, _forcePermissionForOtherParent);
    }

    public static void RefreshColorTransitionAfterFrame<T>(T _naviItem, ref Coroutine _colorRefreshCoroutine, bool _instant = false) where T : Selectable, INavigationItem
    {
        if (_naviItem == null
            || _naviItem.gameObject.activeInHierarchy == false)
        {
            return;
        }

        if (_colorRefreshCoroutine != null)
        {
            _naviItem.StopCoroutine(_colorRefreshCoroutine);
        }

        _colorRefreshCoroutine = _naviItem.StartCoroutine(_refreshAfterFrame());

        IEnumerator _refreshAfterFrame()
        {
            yield return null;
            _naviItem.RefreshStateTransition(_instant);
        }
    }
}
