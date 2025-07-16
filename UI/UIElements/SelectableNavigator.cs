using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class SelectableNavigator
{
    private const float ANGLE_PENAULTY_RATIO = 0.9f;
    private const float MAX_SEARCH_ANGLE = 80f;
    private const float MAX_SEARCH_VECTOR_LENGTH = 100000f;
    private const int DEFAULT_SPACE_ALLOCATED_FOR_SELECTABLES = 100;

    public static bool IsNavigatorBlocked { get; set; } = false;

    private static Selectable[] selectablesArray = new Selectable[DEFAULT_SPACE_ALLOCATED_FOR_SELECTABLES];

    public static Selectable FindNextSelectable<T>(this T _caller, MoveDirection _searchDirection, Navigation.Mode _mode, Selectable _explicitSelection, SelectableExplicitNavigator _explicitNavigator) where T : Selectable, INavigationItem
    {
        if (IsNavigatorBlocked)
        {
            return null;
        }

        if (_mode != Navigation.Mode.Explicit)
        {
            return findNextSelectable(_caller, getSearchVector(_searchDirection));
        }

        if (_explicitSelection != null && _explicitSelection.gameObject.activeSelf && _explicitSelection.interactable)
        {
            return _explicitSelection;
        }

        if (_explicitNavigator.AddNewExplicitSelections)
        {
            Selectable _selection = _explicitNavigator.GetExplicitSelection(_searchDirection);

            if (_selection == null && _explicitNavigator.UseAutomaticSelectionIfExplicitSelectionIsNull)
            {
                return findNextSelectable(_caller, getSearchVector(_searchDirection));
            }

            return _selection;
        }

        if (_explicitNavigator.UseAutomaticSelectionIfExplicitSelectionIsNull)
        {
            return findNextSelectable(_caller, getSearchVector(_searchDirection));
        }

        return null;
    }

    public static void TryToRestoreSelection(this Selectable _selectable)
    {
        if (_selectable != null
            && (_selectable.gameObject.activeSelf == false || _selectable.interactable == false)
            && _selectable.IsObjectSelectedByEventSystem())
        {
            Selectable _closest = _selectable.FindClosestSelectable();

            if (_closest != null)
            {
                _closest.Select();
            }
        }
    }

    public static void TryToRestoreSelection(this GameObject _go)
    {
        if (_go != null && _go.activeSelf == false
            && _go.IsObjectSelectedByEventSystem()
            && _go.TryGetComponent(out Selectable _selectable))
        {
            Selectable _closest = _selectable.FindClosestSelectable();

            if (_closest != null)
            {
                _closest.Select();
            }
        }
    }

    public static void SafeSetActive(this Selectable _selectable, bool _active)
    {
        if (_selectable == null || _selectable.gameObject.activeSelf == _active)
        {
            return;
        }

        _selectable.gameObject.SetActive(_active);

        if (_active == false)
        {
            _selectable.TryToRestoreSelection();
        }
    }

    public static Selectable FindAnySelectableAtCenter()
    {
        int _selectableCount = updateSelectablesArray();
        Resolution _resolution = Screen.currentResolution;
        Vector2 _screenCenter = 0.5f * new Vector2(_resolution.width, _resolution.height);

        Selectable _closestSelectable = null;
        float _sqrDistanceToClosestSelectable = float.MaxValue;

        for (int i = 0; i < _selectableCount; i++)
        {
            Selectable _currentSelectable = selectablesArray[i];

            if (_currentSelectable == null
                || _currentSelectable.interactable == false
                || _currentSelectable.transform.lossyScale == Vector3.zero
                || _currentSelectable.navigation.mode == Navigation.Mode.None
                || _currentSelectable is not INavigationItem _naviItem
                || _naviItem.AllowNavigationToThisObject == false)
            {
                continue;
            }

            float _sqrDistanceToCurrentSelectableCenter = ((Vector2)_currentSelectable.transform.position - _screenCenter).sqrMagnitude;

            if (_sqrDistanceToCurrentSelectableCenter < _sqrDistanceToClosestSelectable)
            {
                _sqrDistanceToClosestSelectable = _sqrDistanceToCurrentSelectableCenter;
                _closestSelectable = selectablesArray[i];
            }
        }

        clearSelectablesArray();

        return _closestSelectable;
    }

    public static Selectable FindClosestSelectable(this Selectable _callerSelectable, bool _forcePermissionForOtherParent = false)
    {
        if ((_callerSelectable is INavigationItem) == false)
        {
            return null;
        }

        Transform _callerTransform = _callerSelectable.transform;
        RectTransform _callerRectTransform = _callerTransform as RectTransform;

        if (_callerRectTransform == null)
        {
            return null;
        }

        bool _allowNavigationToOtherParent = ((INavigationItem)_callerSelectable).AllowNavigationToOtherParent;
        Transform _callerParent = _callerTransform.parent;
        int _selectableCount = updateSelectablesArray();

        Vector2 _searchStartPoint = _callerRectTransform.rect.center;

        Selectable _closestSelectable = null;
        float _sqrDistanceToClosestSelectable = float.MaxValue;

        for (int i = 0; i < _selectableCount; i++)
        {
            Selectable _currentSelectable = selectablesArray[i];

            if (_currentSelectable == null)
            {
                continue;
            }

            if (_currentSelectable is INavigationItem _naviItem)
            {
                if (_naviItem.AllowNavigationToThisObject == false)
                {
                    continue;
                }
            }
            else
            {
                continue;
            }

            RectTransform _currentRectTransform = _currentSelectable.transform as RectTransform;

            if (checkContinueConditions(_callerSelectable, _callerParent, _allowNavigationToOtherParent, _currentSelectable, _currentRectTransform, _forcePermissionForOtherParent))
            {
                continue;
            }

            Vector2 _currentSelectableCenter = TransformPoint(_currentRectTransform, _callerTransform, _currentRectTransform.rect.center);
            float _sqrDistanceToCurrentSelectableCenter = (_currentSelectableCenter - _searchStartPoint).sqrMagnitude;

            if (_sqrDistanceToCurrentSelectableCenter < _sqrDistanceToClosestSelectable)
            {
                _sqrDistanceToClosestSelectable = _sqrDistanceToCurrentSelectableCenter;
                _closestSelectable = _currentSelectable;
            }
        }

        clearSelectablesArray();

        return _closestSelectable;
    }

    public static Vector3 TransformPoint(this Transform _from, Transform _to, Vector3 _position)
    {
        return _to.InverseTransformPoint(_from.TransformPoint(_position));
    }

    private static Selectable findNextSelectable(Selectable _callerSelectable, Vector2 _searchDirection)
    {
        if ((_callerSelectable is INavigationItem) == false)
        {
            return null;
        }

        Transform _callerTransform = _callerSelectable.transform;
        RectTransform _callerRectTransform = _callerTransform as RectTransform;

        if (_callerRectTransform == null)
        {
            return null;
        }

        bool _allowNavigationToOtherParent = ((INavigationItem)_callerSelectable).AllowNavigationToOtherParent;
        Transform _callerParent = _callerTransform.parent;
        int _selectableCount = updateSelectablesArray();

        Vector2 _searchStartPoint = getPointOnRectEdge(_callerRectTransform, _searchDirection);
        Vector2 _searchLineEndPoint = _searchStartPoint + (_searchDirection * MAX_SEARCH_VECTOR_LENGTH);
        Vector2 _searchLineEndPointBackward = _searchStartPoint - (_searchDirection * MAX_SEARCH_VECTOR_LENGTH);

        float _minCenterDistanceSquaredMagnitude = Mathf.Infinity;
        float _minDirectLineSquaredMagnitude = Mathf.Infinity;
        float _maxDirectLineSquaredMagnitude = Mathf.NegativeInfinity;

        Selectable _bestCenterSelectable = null;
        Selectable _bestDirectSelectable = null;
        Selectable _worstDirectSelectable = null;

        for (int i = 0; i < _selectableCount; i++)
        {
            Selectable _currentSelectable = selectablesArray[i];

            if (_currentSelectable == null)
            {
                continue;
            }

            RectTransform _currentRectTransform = _currentSelectable.transform as RectTransform;

            #region Continue Exceptions

            if (_currentSelectable is INavigationItem _naviItem)
            {
                if (_naviItem.AllowNavigationToThisObject == false)
                {
                    continue;
                }
            }
            else
            {
                continue;
            }

            if (checkContinueConditions(_callerSelectable, _callerParent, _allowNavigationToOtherParent, _currentSelectable, _currentRectTransform))
            {
                continue;
            }

            #endregion

            Rect _currentSelectableRect = transformRectToTargetAndInvertY(_currentRectTransform, _callerTransform, _currentRectTransform.rect);
            RectLineCaster.LineCastEntry _forwardSearch = RectLineCaster.CastLineEntryIntersection(_searchStartPoint, _searchLineEndPoint, _currentSelectableRect);

            if (_forwardSearch.Intersects)
            {
                float _directLineSquaredMagnitude = ((_searchLineEndPoint - _searchStartPoint) * _forwardSearch.Entry).sqrMagnitude;

                if (_directLineSquaredMagnitude < _minDirectLineSquaredMagnitude)
                {
                    _minDirectLineSquaredMagnitude = _directLineSquaredMagnitude;
                    _bestDirectSelectable = _currentSelectable;
                }
            }

            RectLineCaster.LineCastEntry _backwardSearch = RectLineCaster.CastLineEntryIntersection(_searchStartPoint, _searchLineEndPointBackward, _currentSelectableRect);

            if (_backwardSearch.Intersects)
            {
                float _backwardLineSquaredMagnitude = ((_searchLineEndPointBackward - _searchStartPoint) * _backwardSearch.Entry).sqrMagnitude;

                if (_backwardLineSquaredMagnitude > _maxDirectLineSquaredMagnitude)
                {
                    _maxDirectLineSquaredMagnitude = _backwardLineSquaredMagnitude;
                    _worstDirectSelectable = _currentSelectable;
                }
            }

            Vector2 _currentSelectableCenter = TransformPoint(_currentRectTransform, _callerTransform, _currentRectTransform.rect.center);
            Vector2 _searchVectorToCurrentSelectableCenter = _currentSelectableCenter - _searchStartPoint;

            float _deviationAngle = Mathf.Abs(Vector2.Angle(_searchDirection, _searchVectorToCurrentSelectableCenter));

            if (_deviationAngle > MAX_SEARCH_ANGLE)
            {
                continue;
            }

            // Magnitude multiplied because direct selection is better. Multiplier = (1 + (0-1) percentage penaulty)
            float _score = _searchVectorToCurrentSelectableCenter.sqrMagnitude * (1f + 1f - ANGLE_PENAULTY_RATIO + (ANGLE_PENAULTY_RATIO * _deviationAngle / MAX_SEARCH_ANGLE));

            if (_score < _minCenterDistanceSquaredMagnitude)
            {
                _minCenterDistanceSquaredMagnitude = _score;
                _bestCenterSelectable = _currentSelectable;
            }
        }

        clearSelectablesArray();

        if (_bestDirectSelectable != null && _bestCenterSelectable != null)
        {
            return _minDirectLineSquaredMagnitude > _minCenterDistanceSquaredMagnitude
                ? _bestCenterSelectable
                : _bestDirectSelectable;
        }

        if (_bestDirectSelectable != null)
        {
            return _bestDirectSelectable;
        }

        if (_bestCenterSelectable != null)
        {
            return _bestCenterSelectable;
        }

        return _worstDirectSelectable;
    }

    private static bool checkContinueConditions(Selectable _callerSelectable, Transform _callerParent, bool _allowNavigationToOtherParent, Selectable _currentSelectable, RectTransform _currentRectTransform, bool _forcePermissionForOtherParent = false)
    {
        return _currentSelectable == _callerSelectable
            || _currentSelectable.interactable == false
            || _currentSelectable.navigation.mode == Navigation.Mode.None
            || _currentRectTransform == null
            || (_forcePermissionForOtherParent == false && _allowNavigationToOtherParent == false && _callerParent != _currentSelectable.transform.parent);
    }

    private static Vector3 getPointOnRectEdge(RectTransform _rect, Vector2 _searchDirectionFromCenter)
    {
        if (_rect == null)
        {
            return Vector3.zero;
        }

        if (_searchDirectionFromCenter != Vector2.zero)
        {
            _searchDirectionFromCenter /= Mathf.Max(Mathf.Abs(_searchDirectionFromCenter.x), Mathf.Abs(_searchDirectionFromCenter.y));
        }

        return _rect.rect.center + Vector2.Scale(_rect.rect.size, _searchDirectionFromCenter * 0.5f);
    }

    private static Rect transformRectToTargetAndInvertY(Transform _from, Transform _to, Rect _rect)
    {
        return invertY(transformRectTo(_from, _to, _rect));
    }

    private static Rect invertY(Rect _rect)
    {
        return new Rect(_rect.xMin, _rect.yMin, _rect.width, -_rect.height);
    }

    private static Rect transformRectTo(Transform _from, Transform _to, Rect _rect)
    {
        Vector3 _topLeftCorner;
        Vector3 _bottomLeftCorner;
        Vector3 _topRightCorner;

        if (_from != null)
        {
            _topLeftCorner = _from.TransformPoint(new Vector2(_rect.xMin, _rect.yMin));
            _bottomLeftCorner = _from.TransformPoint(new Vector2(_rect.xMin, _rect.yMax));
            _topRightCorner = _from.TransformPoint(new Vector2(_rect.xMax, _rect.yMin));
        }
        else
        {
            _topLeftCorner = new Vector2(_rect.xMin, _rect.yMin);
            _bottomLeftCorner = new Vector2(_rect.xMin, _rect.yMax);
            _topRightCorner = new Vector2(_rect.xMax, _rect.yMin);
        }

        if (_to != null)
        {
            _topLeftCorner = _to.InverseTransformPoint(_topLeftCorner);
            _bottomLeftCorner = _to.InverseTransformPoint(_bottomLeftCorner);
            _topRightCorner = _to.InverseTransformPoint(_topRightCorner);
        }

        return new Rect(_topLeftCorner.x, _topLeftCorner.y, _topRightCorner.x - _topLeftCorner.x, _topLeftCorner.y - _bottomLeftCorner.y);
    }

    private static Vector2 getSearchVector(MoveDirection _searchDirection)
    {
        return _searchDirection switch
        {
            MoveDirection.Left => Vector2.left,
            MoveDirection.Up => Vector2.up,
            MoveDirection.Right => Vector2.right,
            MoveDirection.Down => Vector2.down,
            _ => Vector2.down,
        };
    }

    private static int updateSelectablesArray()
    {
        int _newSelectablesCount = Selectable.allSelectableCount;

        if (_newSelectablesCount > selectablesArray.Length) //More space required?
        {
            selectablesArray = new Selectable[_newSelectablesCount];
        }

        int _selectableCount = Selectable.AllSelectablesNoAlloc(selectablesArray);

        return _selectableCount;
    }

    private static void clearSelectablesArray()
    {
        System.Array.Clear(selectablesArray, 0, selectablesArray.Length);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init()
    {
        selectablesArray = new Selectable[DEFAULT_SPACE_ALLOCATED_FOR_SELECTABLES];
    }
}
