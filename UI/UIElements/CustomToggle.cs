using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomToggle : Toggle, INavigationItem<CustomToggle>, IToggleContainer
{
    public static UnityAction<INavigationItem> OnAnyToggleValueChanged = null;

    public event UnityAction<int, CustomToggle> OnItemSelected = null;
    public event UnityAction<int, CustomToggle> OnItemDeselected = null;
    public event UnityAction<int, CustomToggle> OnItemDestroyed = null;
    public event UnityAction<int, CustomToggle, bool> OnItemValueChanged = null;

    [SerializeField] private GameObject onObject = null;
    [SerializeField] private GameObject offObject = null;

    [SerializeField] protected bool invokeOnAnyItemSelected = true;
    [SerializeField] protected bool allowNavigationToOtherParent = true;
    [SerializeField] protected bool allowNavitationToThisObject = true;
    [SerializeField] protected bool forceSelectEventOnEnable = true;
    [SerializeField] protected bool forceDeselectEventOnDisable = true;
    [SerializeField] protected bool forceSelectOnPointerEnter = true;
    [SerializeField] protected bool forceRefreshColorTransitionAfterFrame = false;

    [SerializeField] protected SelectableExplicitNavigator explicitNavigator = new SelectableExplicitNavigator();
    [SerializeField] protected SelectionEventController selectionEventController = new SelectionEventController();
    [SerializeField] protected GraphicsTransitionController graphicsTransitionController = new GraphicsTransitionController();

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

    public CustomToggle Toggle => this;
    public CustomToggle NavigationItem => this;
    public INavigationItemPart Owner { get; set; } = null;
    #endregion

    protected Coroutine colorRefreshCoroutine = null;

    protected override void Awake()
    {
        //Forcing instant transitions of check marks! Fade is breaking cloned transitions of graphicsTransitionController.
        toggleTransition = ToggleTransition.None;

        base.Awake();
    }

    protected override void OnDestroy()
    {
        onValueChanged.RemoveListener(OnToggleValueChanged);
        OnItemDestroyed?.Invoke(ItemIndex, this);
        base.OnDestroy();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (ForceRefreshColorTransitionAfterFrame)
        {
            INavigationItem.RefreshColorTransitionAfterFrame(this, ref colorRefreshCoroutine);
        }

        selectionEventController.UpdateIsSelectedOnEnable(this);
        refreshValueVisualization(isOn);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selectionEventController.TryToForceOnDeselect(this);
    }

    public override Selectable FindSelectableOnDown() => INavigationItem.FindSelectableOnDown(this);
    public override Selectable FindSelectableOnLeft() => INavigationItem.FindSelectableOnLeft(this);
    public override Selectable FindSelectableOnRight() => INavigationItem.FindSelectableOnRight(this);
    public override Selectable FindSelectableOnUp() => INavigationItem.FindSelectableOnUp(this);
    public Selectable FindClosestSelectable(bool _forcePermissionForOtherParent = false) => INavigationItem.FindClosestSelectable(this, _forcePermissionForOtherParent);

    public override void OnSelect(BaseEventData _eventData)
    {
        selectionEventController.IsSelected = true;
        INavigationItem.OnNavigationItemSelected(this);

        base.OnSelect(_eventData);
        OnItemSelected?.Invoke(ItemIndex, this);
    }

    public override void OnDeselect(BaseEventData _eventData)
    {
        selectionEventController.IsSelected = false;
        base.OnDeselect(_eventData);
        OnItemDeselected?.Invoke(ItemIndex, this);
    }

    public override void OnPointerEnter(PointerEventData _eventData)
    {
        base.OnPointerEnter(_eventData);
        INavigationItem.OnNavigationItemPointerEnter(this);
    }

    public void Initialize()
    {
        selectionEventController.UpdateSelectionState();

        onValueChanged.AddListener(OnToggleValueChanged);
        graphicsTransitionController.SetSelectable(this);
    }

    public void ChangeAllowNavigationToOtherParent(bool _allow)
    {
        allowNavigationToOtherParent = _allow;
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

    public void OnToggleValueChanged(bool _value)
    {
        OnItemValueChanged?.Invoke(ItemIndex, this, _value);
        OnAnyToggleValueChanged?.Invoke(this);
        refreshValueVisualization(_value);
    }

    public void Refresh()
    {
        refreshValueVisualization(isOn);
    }

    private void refreshValueVisualization(bool _value)
    {
        INavigationItem.RefreshColorTransitionAfterFrame(this, ref colorRefreshCoroutine);

        if (Application.isPlaying == false)
        {
            return;
        }

        if (onObject != null)
        {
            onObject.SetActive(_value);
        }

        if (offObject != null)
        {
            offObject.SetActive(!_value);
        }
    }
}
