using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event UnityAction<ScrollRectAutoScroller> OnScrollingStart = null;
    public event UnityAction<ScrollRectAutoScroller> OnScrollingEnd = null;

    [Header("Settings")]
    [SerializeField] private bool resetPositionWhenEnabled = true;
    [SerializeField, Range(0f, 1f)] private float scrollDuration = 0.25f;
    [SerializeField, Range(1, 10)] private int checkParentingLayersForNestedSelectables = 1;
    [SerializeField] private bool disableWhenMouseIsHovering = true;
    [SerializeField] private bool refreshTargetPositionEveryTime = false;
    [SerializeField] private bool adjustValuesToLayoutGroup = false;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup = null;

    private ScrollRect scrollRect = null;
    private RectTransform targetRectTransform = null;
    private RectTransform scrollRectTransform = null;
    private RectTransform contentPanelRectTransform = null;
    private GameObject currentSelected = null;
    private GameObject lastSelected = null;
    private Coroutine scrollCoroutine = null;

    public bool IsMouseHovering { get; private set; } = false;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (resetPositionWhenEnabled)
        {
            ResetScrollPosition();
        }
    }

    private void Start()
    {
        checkForNewContentPanel();
    }

    private void Update()
    {
        if (disableWhenMouseIsHovering && IsMouseHovering)
        {
            return;
        }

        autoScroll();
    }

    public void OnPointerEnter(PointerEventData _eventData) => IsMouseHovering = true;
    public void OnPointerExit(PointerEventData _eventData) => IsMouseHovering = false;

    public void ResetScrollPosition(bool _stopCoroutine = true)
    {
        if (_stopCoroutine)
        {
            stopMovementCoroutine();
        }

        if (contentPanelRectTransform != null)
        {
            contentPanelRectTransform.anchoredPosition = contentPanelRectTransform.anchoredPosition.WithY(0f);
        }
    }

    /// <summary> Force scroll to rect. Target rect should be child of scroll view content. </summary>
    public void ScrollToRect(RectTransform _rect, bool _instantTransition = false)
    {
        if (scrollRectTransform == null)
        {
            scrollRectTransform = GetComponent<RectTransform>();
        }

        checkForNewContentPanel();

        targetRectTransform = _rect;
        Vector2 _contentPanelPosition = contentPanelRectTransform.anchoredPosition;

        float _viewportHight = scrollRectTransform.rect.height;
        float _viewRangeMin = _contentPanelPosition.y;
        float _viewRangeMax = _viewRangeMin + _viewportHight;

        float _halfOfSelectedRectHight = 0f;
        float _selectedRectPositionY = 0f;

        if (_rect.parent == contentPanelRectTransform)
        {
            _halfOfSelectedRectHight = 0.5f * _rect.sizeDelta.y;
            _selectedRectPositionY = _rect.localPosition.y;
        }
        else
        {
            _halfOfSelectedRectHight = 0.5f * contentPanelRectTransform.InverseTransformVector(_rect.rect.size).y;
            _selectedRectPositionY = contentPanelRectTransform.InverseTransformPoint(_rect.TransformPoint(_rect.anchoredPosition)).y;
        }

        if (-_selectedRectPositionY - _halfOfSelectedRectHight < _viewRangeMin)
        {
            Vector2 _targetPosition = new Vector2(_contentPanelPosition.x, -(_selectedRectPositionY + _halfOfSelectedRectHight));

            if (adjustValuesToLayoutGroup)
            {
                _targetPosition = _targetPosition.WithY(_targetPosition.y - verticalLayoutGroup.padding.top);
            }

            restartScrollMovement(_contentPanelPosition, _targetPosition, _instantTransition);
        }
        else if (-_selectedRectPositionY + _halfOfSelectedRectHight > _viewRangeMax)
        {
            Vector2 _targetPosition = new Vector2(_contentPanelPosition.x, _halfOfSelectedRectHight - (_selectedRectPositionY + _viewportHight));

            if (adjustValuesToLayoutGroup)
            {
                _targetPosition = _targetPosition.WithY(_targetPosition.y + verticalLayoutGroup.padding.bottom);
            }

            restartScrollMovement(_contentPanelPosition, _targetPosition, _instantTransition);
        }
        else
        {
            //Currently selected item is visible so there is no need to restart scrolling.
            OnScrollingStart?.Invoke(this);
            OnScrollingEnd?.Invoke(this);
        }
    }

    private bool getTarget(out Vector2 _targetPosition)
    {
        if (targetRectTransform == null)
        {
            _targetPosition = Vector2.zero;
            return false;
        }

        Vector2 _contentPanelPosition = contentPanelRectTransform.anchoredPosition;

        float _viewportHight = scrollRectTransform.rect.height;
        float _viewRangeMin = _contentPanelPosition.y;
        float _viewRangeMax = _viewRangeMin + _viewportHight;

        float _halfOfSelectedRectHight;
        float _selectedRectPositionY;

        if (targetRectTransform.parent == contentPanelRectTransform)
        {
            _halfOfSelectedRectHight = 0.5f * targetRectTransform.sizeDelta.y;
            _selectedRectPositionY = targetRectTransform.localPosition.y;
        }
        else
        {
            _halfOfSelectedRectHight = 0.5f * contentPanelRectTransform.InverseTransformVector(targetRectTransform.rect.size).y;
            _selectedRectPositionY = contentPanelRectTransform.InverseTransformPoint(targetRectTransform.TransformPoint(targetRectTransform.anchoredPosition)).y;
        }

        if (-_selectedRectPositionY - _halfOfSelectedRectHight < _viewRangeMin)
        {
            _targetPosition = new Vector2(_contentPanelPosition.x, -(_selectedRectPositionY + _halfOfSelectedRectHight));
            return true;
        }
        else if (-_selectedRectPositionY + _halfOfSelectedRectHight > _viewRangeMax)
        {
            _targetPosition = new Vector2(_contentPanelPosition.x, _halfOfSelectedRectHight - (_selectedRectPositionY + _viewportHight));
            return true;
        }

        _targetPosition = Vector2.zero;
        return false;
    }

    private void autoScroll()
    {
        if (EventSystemReference.IsAvailable == false)
        {
            return;
        }

        currentSelected = EventSystemReference.Instance.currentSelectedGameObject;

        if (currentSelected == null)
        {
            return;
        }

        if (checkIfIsNotChildOfContent(currentSelected.transform, contentPanelRectTransform.transform))
        {
            lastSelected = null;
            return;
        }

        if (currentSelected == lastSelected)
        {
            return;
        }

        ScrollToRect((RectTransform)currentSelected.transform);
        lastSelected = currentSelected;
    }

    private bool checkIfIsNotChildOfContent(Transform _currentSelected, Transform _content, int _callID = 0)
    {
        return _currentSelected.parent == null
            || _callID >= checkParentingLayersForNestedSelectables
            || (_currentSelected.parent != _content && checkIfIsNotChildOfContent(_currentSelected.parent, _content, _callID + 1));
    }

    private void checkForNewContentPanel()
    {
        if (contentPanelRectTransform == null && scrollRect != null)
        {
            contentPanelRectTransform = scrollRect.content;
        }
    }

    private void stopMovementCoroutine()
    {
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
            scrollCoroutine = null;
        }
    }

    private void restartScrollMovement(Vector2 _startPosition, Vector2 _targetPosition, bool _instantTransition = false)
    {
        stopMovementCoroutine();

        if (refreshTargetPositionEveryTime == false) //Default
        {
            scrollCoroutine = StartCoroutine(handleScrollMovement(_startPosition, _targetPosition, _instantTransition ? 0f : scrollDuration));
        }
        else
        {
            scrollCoroutine = StartCoroutine(handleScrollMovementWithRefreshedTarget(_startPosition, _targetPosition, _instantTransition ? 0f : scrollDuration));
        }
    }
    private IEnumerator handleScrollMovement(Vector2 _startPosition, Vector2 _targetPosition, float _scrollTime, System.Action _onCompleteCallback = null)
    {
        OnScrollingStart?.Invoke(this);

        float _timer = 0f;

        while (_timer < _scrollTime)
        {
            yield return null;
            _timer += Time.unscaledDeltaTime;

            contentPanelRectTransform.anchoredPosition = Vector2.Lerp(_startPosition, _targetPosition, _timer / _scrollTime);
        }

        contentPanelRectTransform.anchoredPosition = _targetPosition;
        OnScrollingEnd?.Invoke(this);
        _onCompleteCallback?.Invoke();
    }

    private IEnumerator handleScrollMovementWithRefreshedTarget(Vector2 _startPosition, Vector2 _targetPosition, float _scrollTime, System.Action _onCompleteCallback = null)
    {
        OnScrollingStart?.Invoke(this);

        float _timer = 0f;

        while (_timer < _scrollTime)
        {
            yield return null;
            _timer += Time.unscaledDeltaTime;

            if (getTarget(out var _newTarget))
            {
                _targetPosition = _newTarget;
            }

            contentPanelRectTransform.anchoredPosition = Vector2.Lerp(_startPosition, _targetPosition, _timer / _scrollTime);
        }

        contentPanelRectTransform.anchoredPosition = _targetPosition;
        OnScrollingEnd?.Invoke(this);
        _onCompleteCallback?.Invoke();
    }
}
