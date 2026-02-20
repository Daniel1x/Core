using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSlider : Slider, INavigationItem<CustomSlider>, ISliderContainer
{
    protected const float STEP_MULTIPLIER_RESET_DELAY = 0.3f; //When the button is pressed, the Event System fires an OnMove event every 0.22-0.25s, so 0.3s should be safe.
    protected const float STEP_MULTIPLIER_INCREASE_RATIO = 1.2f; //Exponent base, Ratio^X where X is the number of triggered value changes.
    protected const char PERCENTAGE = '%';
    protected const char SECONDS = 's';

    public event UnityAction<int, CustomSlider> OnItemSelected = null;
    public event UnityAction<int, CustomSlider> OnItemDeselected = null;
    public event UnityAction<int, CustomSlider> OnItemDestroyed = null;

    // Settings
    [SerializeField] protected bool invokeOnAnyItemSelected = true;
    [SerializeField] protected bool allowNavigationToOtherParent = true;
    [SerializeField] protected bool allowNavitationToThisObject = true;
    [SerializeField] protected bool forceSelectEventOnEnable = true;
    [SerializeField] protected bool forceDeselectEventOnDisable = true;
    [SerializeField] protected bool forceSelectOnPointerEnter = true;
    [SerializeField] protected bool forceRefreshColorTransitionAfterFrame = false;
    [SerializeField] protected float sliderStepSize = 1f;
    [SerializeField] protected bool displayDecimal = false;
    [SerializeField] protected int decimalPlaces = 2;

    // Text
    [SerializeField] protected bool updateTextOnValueChange = false;
    [SerializeField] protected bool addPercentageSign = true;
    [SerializeField] protected bool addSecondsSign = false;
    [SerializeField] protected TMP_Text valueTMProTextField = null;

    [SerializeField] protected SelectableExplicitNavigator explicitNavigator = new SelectableExplicitNavigator();
    [SerializeField] protected SelectionEventController selectionEventController = new SelectionEventController();
    [SerializeField] protected GraphicsTransitionController graphicsTransitionController = new GraphicsTransitionController();

    protected bool isLastStepPositive = true;
    protected float stepMultiplier = 1;
    protected Coroutine stepMultiplierResetCoroutine = null;

    protected bool isHorizontal = false;
    protected string textFieldFormat = null;

    public TMP_Text TextReference => valueTMProTextField;


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

    public CustomSlider Slider => this;
    public CustomSlider NavigationItem => this;
    public INavigationItemPart Owner { get; set; } = null;
    #endregion

    public override float value
    {
        get => base.value;
        set
        {
            int _steps = Mathf.RoundToInt((value - minValue) / sliderStepSize);
            base.value = (minValue + (_steps * sliderStepSize)).ClampMax(maxValue);
        }
    }

    public float StepSize
    {
        get => sliderStepSize;
        set
        {
            sliderStepSize = value;
            this.value = this.value;
        }
    }

    public float MinValue
    {
        get => minValue;
        set
        {
            minValue = value;
            this.value = this.value;
        }
    }

    public float MaxValue
    {
        get => maxValue;
        set
        {
            maxValue = value;
            this.value = this.value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        selectionEventController.UpdateSelectionState();

        if (updateTextOnValueChange)
        {
            onValueChanged.AddListener(updateTextField);
        }

        isHorizontal = direction == Direction.LeftToRight || direction == Direction.RightToLeft;
        textFieldFormat = createTextFormat();

        graphicsTransitionController.SetSelectable(this);
    }

    private Vector3 lastAnchoredPosition = Vector3.zero;

    protected override void Update()
    {
        base.Update();

        if (EventSystemReference.IsObjectSelected(gameObject) && EventSystemReference.IsMouseDown)
        {
            if (lastAnchoredPosition != handleRect.localPosition)
            {
                lastAnchoredPosition = handleRect.localPosition;
            }
        }
    }

    protected override void OnDestroy()
    {
        if (updateTextOnValueChange)
        {
            onValueChanged.RemoveListener(updateTextField);
        }

        OnItemDestroyed?.Invoke(ItemIndex, this);

        base.OnDestroy();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (updateTextOnValueChange)
        {
            updateTextField(value);
        }

        if (forceRefreshColorTransitionAfterFrame)
        {
            RefreshColorTransitionAfterFrame();
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
        return SelectableNavigator.IsNavigatorBlocked == true
                ? null
                : isHorizontal == true
                    ? this.FindNextSelectable(MoveDirection.Down, navigation.mode, navigation.selectOnDown, explicitNavigator)
                    : base.FindSelectableOnDown();
    }

    public override Selectable FindSelectableOnLeft()
    {
        return SelectableNavigator.IsNavigatorBlocked == true
                ? null
                : isHorizontal == false
                    ? this.FindNextSelectable(MoveDirection.Left, navigation.mode, navigation.selectOnLeft, explicitNavigator)
                    : base.FindSelectableOnLeft();
    }

    public override Selectable FindSelectableOnRight()
    {
        return SelectableNavigator.IsNavigatorBlocked == true
                ? null
                : isHorizontal == false
                    ? this.FindNextSelectable(MoveDirection.Right, navigation.mode, navigation.selectOnRight, explicitNavigator)
                    : base.FindSelectableOnRight();
    }

    public override Selectable FindSelectableOnUp()
    {
        return SelectableNavigator.IsNavigatorBlocked == true
                ? null
                : isHorizontal == true
                    ? this.FindNextSelectable(MoveDirection.Up, navigation.mode, navigation.selectOnUp, explicitNavigator)
                    : base.FindSelectableOnUp();
    }

    public Selectable FindClosestSelectable(bool _forcePermissionForOtherParent = false)
    {
        return SelectableNavigator.FindClosestSelectable(this, _forcePermissionForOtherParent);
    }

    #endregion

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

        stopStepMultiplierResetCoroutine();
        resetStepMultiplier();
    }

    public override void OnPointerEnter(PointerEventData _eventData)
    {
        base.OnPointerEnter(_eventData);

        if (forceSelectOnPointerEnter && interactable)
        {
            Select();
        }
    }

    public override void OnMove(AxisEventData _eventData)
    {
        float _value = value;
        base.OnMove(_eventData);

        switch (_eventData.moveDir)
        {
            case MoveDirection.Up:
                FindSelectableOnUp()?.Select();
                return;

            case MoveDirection.Right:
                increaseStepMultiplier(true);
                value = Mathf.Clamp(_value + (sliderStepSize * getRoundedStepMultiplier()), minValue, maxValue);
                return;

            case MoveDirection.Down:
                FindSelectableOnDown()?.Select();
                return;

            case MoveDirection.Left:
                increaseStepMultiplier(false);
                value = Mathf.Clamp(_value - (sliderStepSize * getRoundedStepMultiplier()), minValue, maxValue);
                return;
        }
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

    public void ChangeAllowNavigationToOtherParent(bool _allow)
    {
        allowNavigationToOtherParent = _allow;
    }

    public void RefreshColorTransitionAfterFrame(bool _instant = false)
    {
        if (gameObject.activeInHierarchy == false)
        {
            return;
        }

        StartCoroutine(_refreshAfterFrame());

        IEnumerator _refreshAfterFrame()
        {
            yield return null;
            RefreshStateTransition(_instant);
        }
    }

    public void SetSliderEnabled(bool _enable)
    {
        base.enabled = _enable;
    }

    public void SetSliderInteractable(bool _enable)
    {
        base.interactable = _enable;
    }

    public override void SetValueWithoutNotify(float _sliderRawValue)
    {
        base.SetValueWithoutNotify(_sliderRawValue);
        ForceUpdateTextField(_sliderRawValue);
    }

    public void ForceUpdateTextField(float _sliderRawValue)
    {
        if (updateTextOnValueChange)
        {
            updateTextField(_sliderRawValue);
        }
    }

    protected string createTextFormat()
    {
        string _textFormat = "0";

        if (displayDecimal && decimalPlaces > 0)
        {
            _textFormat += ".";

            for (int i = 0; i < decimalPlaces; i++)
            {
                _textFormat += "0";
            }
        }

        return _textFormat;
    }

    protected void updateTextField(float _sliderRawValue)
    {
        if (valueTMProTextField == null)
        {
            MyLog.Log("There is no text field to update!", Color.yellow);
            return;
        }

        valueTMProTextField.text = getValueText(_sliderRawValue);
    }

    protected string getValueText(float _sliderRawValue)
    {
        if (addPercentageSign)
        {
            return _sliderRawValue.ToString(textFieldFormat) + PERCENTAGE;
        }
        else if (addSecondsSign)
        {
            return _sliderRawValue.ToString(textFieldFormat) + SECONDS;
        }
        else
        {
            return _sliderRawValue.ToString(textFieldFormat);
        }
    }

    protected int getRoundedStepMultiplier()
    {
        return Mathf.FloorToInt(stepMultiplier);
    }

    protected void resetStepMultiplier()
    {
        stepMultiplier = 1f;
    }

    protected void stopStepMultiplierResetCoroutine()
    {
        if (stepMultiplierResetCoroutine != null)
        {
            StopCoroutine(stepMultiplierResetCoroutine);
        }
    }

    protected void increaseStepMultiplier(bool _positiveDirection)
    {
        if (isLastStepPositive != _positiveDirection)
        {
            resetStepMultiplier();
            isLastStepPositive = _positiveDirection;
        }

        stopStepMultiplierResetCoroutine();

        stepMultiplier *= STEP_MULTIPLIER_INCREASE_RATIO;
        stepMultiplierResetCoroutine = StartCoroutine(_delayMultiplierReset());

        IEnumerator _delayMultiplierReset()
        {
            yield return new WaitForSecondsRealtime(STEP_MULTIPLIER_RESET_DELAY);
            resetStepMultiplier();
        }
    }
}
