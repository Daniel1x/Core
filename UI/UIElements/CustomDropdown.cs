using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomDropdown : TMP_Dropdown, INavigationItem<CustomDropdown>, IDropdownContainer
{
    public static UnityAction<INavigationItem> OnAnyDropdownValueChanged = null;

    ///<summary>(Dropdown ID, Dropdown, Selected Option ID)</summary>
    public event UnityAction<int, CustomDropdown, int> OnDropdownValueChanged = null;
    public event UnityAction<int, CustomDropdown> OnDropdownOpen = null;
    public event UnityAction<int, CustomDropdown> OnDropdownClose = null;
    public event UnityAction<int, CustomDropdown> OnItemSelected = null;
    public event UnityAction<int, CustomDropdown> OnItemDeselected = null;
    public event UnityAction<int, CustomDropdown> OnItemDestroyed = null;

    [SerializeField] protected bool invokeOnAnyItemSelected = true;
    [SerializeField] protected bool allowNavigationToOtherParent = true;
    [SerializeField] protected bool allowNavitationToThisObject = true;
    [SerializeField] protected bool forceSelectEventOnEnable = true;
    [SerializeField] protected bool forceDeselectEventOnDisable = true;
    [SerializeField] protected bool forceSelectOnPointerEnter = true;
    [SerializeField] protected bool forceSelectOnDropdownClose = false;
    [SerializeField] protected bool forceRefreshColorTransitionAfterFrame = false;

    [SerializeField] protected SelectableExplicitNavigator explicitNavigator = new SelectableExplicitNavigator();
    [SerializeField] protected SelectionEventController selectionEventController = new SelectionEventController();
    [SerializeField] protected GraphicsTransitionController graphicsTransitionController = new GraphicsTransitionController();

    protected List<DropdownItem> dropdownItems = new List<DropdownItem>();
    protected Coroutine colorRefreshCoroutine = null;

    public bool IsShowing { get; private set; }

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

    public CustomDropdown Dropdown => this;
    public CustomDropdown NavigationItem => this;
    public INavigationItemPart Owner { get; set; } = null;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        graphicsTransitionController.SetSelectable(this);
        selectionEventController.UpdateSelectionState();

        onValueChanged.AddListener(onDropdownValueChanged);
    }

    protected override void OnDestroy()
    {
        onValueChanged.RemoveListener(onDropdownValueChanged);

        base.OnDestroy();
        OnItemDestroyed?.Invoke(ItemIndex, this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (ForceRefreshColorTransitionAfterFrame)
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

    protected virtual void Update()
    {
        checkIfDropdownShouldBeClosed();
    }

    public override Selectable FindSelectableOnDown() => INavigationItem.FindSelectableOnDown(this);
    public override Selectable FindSelectableOnLeft() => INavigationItem.FindSelectableOnLeft(this);
    public override Selectable FindSelectableOnRight() => INavigationItem.FindSelectableOnRight(this);
    public override Selectable FindSelectableOnUp() => INavigationItem.FindSelectableOnUp(this);
    public Selectable FindClosestSelectable(bool _forcePermissionForOtherParent = false) => INavigationItem.FindClosestSelectable(this, _forcePermissionForOtherParent);

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        INavigationItem.OnNavigationItemPointerEnter(this);
    }

    protected override GameObject CreateDropdownList(GameObject _template)
    {
        IsShowing = true;
        OnDropdownOpen?.Invoke(ItemIndex, this);

        return base.CreateDropdownList(_template);
    }

    protected override void DestroyDropdownList(GameObject _dropdownList)
    {
        base.DestroyDropdownList(_dropdownList);
        IsShowing = false;

        if (forceSelectOnDropdownClose)
        {
            Select();
        }

        OnDropdownClose?.Invoke(ItemIndex, this);
    }

    protected override DropdownItem CreateItem(DropdownItem _itemTemplate)
    {
        DropdownItem _newItem = base.CreateItem(_itemTemplate);
        dropdownItems.Add(_newItem);
        return _newItem;
    }

    protected override void DestroyItem(DropdownItem _item)
    {
        dropdownItems.Remove(_item);
        base.DestroyItem(_item);
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

    protected void checkIfDropdownShouldBeClosed()
    {
        if (IsShowing == false
            || EventSystemReference.IsAvailable == false)
        {
            return;
        }

        GameObject _currentSelected = EventSystemReference.Instance.currentSelectedGameObject;

        if (_currentSelected == null
            || _currentSelected == gameObject
            || isDropdownItemSelected(_currentSelected))
        {
            return;
        }

        this.Hide();
    }

    protected bool isDropdownItemSelected(GameObject _currentSelected)
    {
        int _count = dropdownItems.Count;

        for (int i = 0; i < _count; i++)
        {
            if (dropdownItems[i] != null && _currentSelected == dropdownItems[i].gameObject)
            {
                return true;
            }
        }

        return false;
    }

    private void onDropdownValueChanged(int _optionID)
    {
        OnDropdownValueChanged?.Invoke(ItemIndex, this, _optionID);
        OnAnyDropdownValueChanged?.Invoke(this);
    }
}
