using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollbar : Scrollbar, INavigationItem<CustomScrollbar>, IScrollbarContainer
{
    public event UnityAction<int, CustomScrollbar> OnItemSelected = null;
    public event UnityAction<int, CustomScrollbar> OnItemDeselected = null;
    public event UnityAction<int, CustomScrollbar> OnItemDestroyed = null;

    [SerializeField] protected bool invokeOnAnyItemSelected = true;
    [SerializeField] protected bool allowNavigationToOtherParent = true;
    [SerializeField] protected bool allowNavitationToThisObject = true;
    [SerializeField] protected bool forceSelectEventOnEnable = true;
    [SerializeField] protected bool forceDeselectEventOnDisable = true;
    [SerializeField] protected bool forceSelectOnPointerEnter = true;
    [SerializeField] protected bool forceClickOnPointerDown = false;
    [SerializeField] protected bool forceRefreshColorTransitionAfterFrame = false;

    [SerializeField] protected SelectableExplicitNavigator explicitNavigator = new SelectableExplicitNavigator();
    [SerializeField] protected SelectionEventController selectionEventController = new SelectionEventController();
    [SerializeField] protected GraphicsTransitionController graphicsTransitionController = new GraphicsTransitionController();

    protected bool isHorizontal = false;
    protected Coroutine colorRefreshCoroutine = null;

    #region INavigationItem Interface
    protected int itemIndex = -1;
    public int ItemIndex { get => itemIndex < 0 ? transform.GetSiblingIndex() : itemIndex; set => itemIndex = value; }

    public bool InvokeOnAnyItemSelected { get => invokeOnAnyItemSelected; set => invokeOnAnyItemSelected = value; }
    public bool AllowNavigationToOtherParent { get => allowNavigationToOtherParent; set => allowNavigationToOtherParent = value; }
    public bool AllowNavigationToThisObject { get => allowNavitationToThisObject; set => allowNavitationToThisObject = value; }
    public bool ForceSelectEventOnEnable { get => forceSelectEventOnEnable; set => forceSelectEventOnEnable = value; }
    public bool ForceDeselectEventOnDisable { get => forceDeselectEventOnDisable; set => forceDeselectEventOnDisable = value; }
    public bool ForceSelectOnPointerEnter { get => forceSelectOnPointerEnter; set => forceSelectOnPointerEnter = value; }
    public bool ForceRefreshColorTransitionAfterFrame { get => forceRefreshColorTransitionAfterFrame; set => forceRefreshColorTransitionAfterFrame = value; }
    public bool SelectInProgressTriggeredFromOnEnable { get; set; } = false;

    public SelectableExplicitNavigator ExplicitNavigator => explicitNavigator;
    public SelectionEventController SelectionEventController => selectionEventController;
    public GraphicsTransitionController GraphicsTransitionController => graphicsTransitionController;

    public CustomScrollbar Scrollbar => this;
    public CustomScrollbar NavigationItem => this;
    public INavigationItemPart Owner { get; set; } = null;
    #endregion

    protected override void Awake()
    {
        selectionEventController.UpdateSelectionState();
        base.Awake();

        isHorizontal = direction == Direction.LeftToRight || direction == Direction.RightToLeft;

        graphicsTransitionController.SetSelectable(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OnItemDestroyed?.Invoke(ItemIndex, this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (forceRefreshColorTransitionAfterFrame)
        {
            INavigationItem.RefreshColorTransitionAfterFrame(this, ref colorRefreshCoroutine);
        }

        selectionEventController.UpdateIsSelectedOnEnable(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selectionEventController.TryToForceOnDeselect(this);
    }

    #region Navigation override

    public override Selectable FindSelectableOnDown()
    {
        return SelectableNavigator.IsNavigatorBlocked
                ? null
                : isHorizontal == true
                    ? INavigationItem.FindSelectableOnDown(this)
                    : base.FindSelectableOnDown();
    }

    public override Selectable FindSelectableOnLeft()
    {
        return SelectableNavigator.IsNavigatorBlocked
                ? null
                : isHorizontal == false
                    ? INavigationItem.FindSelectableOnLeft(this)
                    : base.FindSelectableOnLeft();
    }

    public override Selectable FindSelectableOnRight()
    {
        return SelectableNavigator.IsNavigatorBlocked
                ? null
                : isHorizontal == false
                    ? INavigationItem.FindSelectableOnRight(this)
                    : base.FindSelectableOnRight();
    }

    public override Selectable FindSelectableOnUp()
    {
        return SelectableNavigator.IsNavigatorBlocked
                ? null
                : isHorizontal == true
                    ? INavigationItem.FindSelectableOnUp(this)
                    : base.FindSelectableOnUp();
    }

    public Selectable FindClosestSelectable(bool _forcePermissionForOtherParent = false) => INavigationItem.FindClosestSelectable(this, _forcePermissionForOtherParent);

    #endregion

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        INavigationItem.OnNavigationItemPointerEnter(this);
    }

    public override void OnSelect(BaseEventData _eventData)
    {
        selectionEventController.IsSelected = true;
        base.OnSelect(_eventData);
        OnItemSelected?.Invoke(ItemIndex, this);
    }

    public override void OnDeselect(BaseEventData _eventData)
    {
        selectionEventController.IsSelected = false;
        base.OnDeselect(_eventData);
        OnItemDeselected?.Invoke(ItemIndex, this);
    }

    protected override void DoStateTransition(SelectionState _state, bool _instant)
    {
        base.DoStateTransition(_state, _instant);
        graphicsTransitionController.DoStateTransition((GraphicsTransitionController.SelectionState)_state, _instant);
    }

    public void RefreshStateTransition(bool _instant = false)
    {
        DoStateTransition(currentSelectionState, _instant);
    }
}
