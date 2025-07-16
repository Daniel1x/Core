using DL.Structs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ComponentExtensions
{
    public static void SetLocalPositionX(this Transform _transform, float _newX)
    {
        _transform.localPosition = _transform.localPosition.WithX(_newX);
    }

    public static void SetLocalPositionY(this Transform _transform, float _newY)
    {
        _transform.localPosition = _transform.localPosition.WithY(_newY);
    }

    public static void SetLocalPositionZ(this Transform _transform, float _newZ)
    {
        _transform.localPosition = _transform.localPosition.WithZ(_newZ);
    }

    public static Transform FindChildTransformWithName(this Transform _parent, string _name)
    {
        if (_parent == null)
        {
            return null;
        }

        if (_parent.name == _name)
        {
            return _parent;
        }

        int _childCount = _parent.childCount;

        for (int i = 0; i < _childCount; i++)
        {
            Transform _child = FindChildTransformWithName(_parent.GetChild(i), _name);

            if (_child != null)
            {
                return _child;
            }
        }

        return null;
    }

    public static Transform FindChildTransformWithSimilarName(this Transform _parent, string _name)
    {
        if (_parent == null)
        {
            return null;
        }

        if (_parent.name.Contains(_name))
        {
            return _parent;
        }

        int _childCount = _parent.childCount;

        for (int i = 0; i < _childCount; i++)
        {
            Transform _child = FindChildTransformWithSimilarName(_parent.GetChild(i), _name);

            if (_child != null)
            {
                return _child;
            }
        }

        return null;
    }

    public static List<Transform> FindChildTransformsWithName(this Transform _parent, string _name)
    {
        List<Transform> _children = new List<Transform>();

        if (_parent == null)
        {
            return _children;
        }

        if (_parent.name == _name)
        {
            _children.Add(_parent);
        }

        int _childCount = _parent.childCount;

        for (int i = 0; i < _childCount; i++)
        {
            Transform _child = FindChildTransformWithName(_parent.GetChild(i), _name);

            if (_child != null)
            {
                _children.Add(_child);
            }
        }

        return _children;
    }

    public static List<Transform> FindChildTransformsWithSimilarName(this Transform _parent, string _name, string _nameToExclude = "")
    {
        List<Transform> _children = new List<Transform>();

        if (_parent == null)
        {
            return _children;
        }

        string _parentName = _parent.name;

        if (_nameToExclude.IsNullEmptyOrWhitespace())
        {
            if (_parentName.Contains(_name))
            {
                _children.Add(_parent);
            }
        }
        else
        {
            if (_parentName.Contains(_name) && _parentName.Contains(_nameToExclude) == false)
            {
                _children.Add(_parent);
            }
        }

        Transform[] _allTransforms = _parent.GetComponentsInChildren<Transform>(true);

        foreach (var _transform in _allTransforms)
        {
            if (_transform == null)
            {
                continue;
            }

            string _transformName = _transform.name;

            if (_nameToExclude.IsNullEmptyOrWhitespace())
            {
                if (_transformName.Contains(_name))
                {
                    _children.Add(_transform);
                }
            }
            else
            {
                if (_transformName.Contains(_name) && _transformName.Contains(_nameToExclude) == false)
                {
                    _children.Add(_transform);
                }
            }
        }

        return _children;
    }

    public static T GetOrAddComponent<T>(this GameObject _gameObject, out bool _added) where T : Component
    {
        if (_gameObject == null)
        {
            _added = false;
            return null;
        }

        T _component = _gameObject.GetComponent<T>();

        if (_component == null)
        {
            _component = _gameObject.AddComponent<T>();
            _added = true;
        }
        else
        {
            _added = false;
        }

        return _component;
    }

    public static T GetComponentHereOrInParent<T>(this GameObject _object, bool _includeInactive = true) where T : Component
    {
        if (_object == null)
        {
            return null;
        }

        T _component = _object.GetComponent<T>();

        if (_component == null)
        {
            _component = _object.GetComponentInParent<T>(_includeInactive);
        }

        return _component;
    }

    public static T GetComponentHereOrInParent<T>(this Component _object, bool _includeInactive = true) where T : Component
    {
        if (_object == null)
        {
            return null;
        }

        T _component = _object.GetComponent<T>();

        if (_component == null)
        {
            _component = _object.GetComponentInParent<T>(_includeInactive);
        }

        return _component;
    }

    public static void SetHideFlags(this GameObject _gameObject, HideFlags _flags, bool _recursive = true)
    {
        _gameObject.hideFlags = _flags;

        if (_recursive)
        {
            Transform[] _allChildObjects = _gameObject.GetComponentsInChildren<Transform>();

            foreach (var _child in _allChildObjects)
            {
                _child.gameObject.hideFlags = _flags;
            }
        }
    }

    public static Transform GetOrCreateChildTransform(this GameObject _parent, string _childName)
    {
        return _parent.transform.GetOrCreateChildTransform(_childName);
    }

    public static Transform GetOrCreateChildTransform<T>(this T _parent, string _childName) where T : Component
    {
        return _parent.transform.GetOrCreateChildTransform(_childName);
    }

    public static Transform GetOrCreateChildTransform(this Transform _parent, string _childName)
    {
        int _childCount = _parent.childCount;

        for (int i = 0; i < _childCount; i++)
        {
            Transform _child = _parent.GetChild(i);

            if (_child.name == _childName)
            {
                return _child;
            }
        }

        Transform _newTransform = new GameObject(_childName).transform;
        _newTransform.parent = _parent;
        _newTransform.localPosition = Vector3.zero;
        _newTransform.localRotation = Quaternion.identity;

        return _newTransform;
    }

    public static void SetWorldScale(this Transform _transform, Vector3 _worldScale)
    {
        _transform.SetWorldScaleAndNewParent(_worldScale, _transform.parent);
    }

    public static void SetWorldScaleAndNewParent(this Transform _transform, Vector3 _worldScale, Transform _newParent = null)
    {
        _transform.parent = null;
        _transform.localScale = _worldScale;
        _transform.parent = _newParent;
    }

    public static int GetActiveChildCount(this Transform _transform)
    {
        int _totalCount = _transform.childCount;

        for (int i = _totalCount - 1; i >= 0; i--)
        {
            if (_transform.GetChild(i).gameObject.activeSelf == false)
            {
                _totalCount--;
            }
        }

        return _totalCount;
    }

    public static int GetActiveChildCount(this Transform _transform, out List<Transform> _activeChilds)
    {
        int _totalCount = _transform.childCount;
        _activeChilds = new List<Transform>();

        for (int i = _totalCount - 1; i >= 0; i--)
        {
            if (_transform.GetChild(i).gameObject.activeSelf == false)
            {
                _totalCount--;
            }
            else
            {
                _activeChilds.Add(_transform.GetChild(i));
            }
        }

        return _totalCount;
    }

    public static int GetActiveChildCount<T>(this T _component) where T : Component
    {
        return _component.transform.GetActiveChildCount();
    }

    public static int GetInactiveChildCount(this Transform _transform)
    {
        int _totalCount = _transform.childCount;

        for (int i = _totalCount - 1; i >= 0; i--)
        {
            if (_transform.GetChild(i).gameObject.activeSelf)
            {
                _totalCount--;
            }
        }

        return _totalCount;
    }

    public static int GetInactiveChildCount<T>(this T _component) where T : Component
    {
        return _component.transform.GetInactiveChildCount();
    }

    public static T1 GetComponentHereOrInChildren<T1>(this Transform _transform, int _searchDepth) where T1 : Component
    {
        T1 _component = _transform.GetComponent<T1>();

        if (_component != null)
        {
            return _component;
        }

        if (_searchDepth <= 0)
        {
            return null;
        }

        return _getComponentInChilds(_transform, _searchDepth - 1);

        T1 _getComponentInChilds(Transform _parent, int _searchDepth)
        {
            for (int i = 0; i < _parent.childCount; i++)
            {
                _component = _parent.GetChild(i).GetComponent<T1>();

                if (_component != null)
                {
                    return _component;
                }
            }

            _searchDepth--;

            if (_searchDepth >= 0)
            {
                for (int i = 0; i < _parent.childCount; i++)
                {
                    _component = _getComponentInChilds(_parent.GetChild(i), _searchDepth);

                    if (_component != null)
                    {
                        return _component;
                    }
                }
            }

            return null;
        }
    }

    public static void SetEnabled<T>(this T[] _array, bool _enabled) where T : Behaviour
    {
        if (_array == null)
        {
            return;
        }

        int _count = _array.Length;

        for (int i = 0; i < _count; i++)
        {
            if (_array[i] != null)
            {
                _array[i].enabled = _enabled;
            }
        }
    }

    public static void SetEnabled<T>(this List<T> _list, bool _enabled) where T : Behaviour
    {
        if (_list == null)
        {
            return;
        }

        int _count = _list.Count;

        for (int i = 0; i < _count; i++)
        {
            if (_list[i] != null)
            {
                _list[i].enabled = _enabled;
            }
        }
    }

    public static string GetHierarchyPathFromRoot<T>(this T _component) where T : Component
    {
        string _path = _component.name;
        Transform _parent = _component.transform.parent;

        while (_parent != null)
        {
            _path = _parent.name + "/" + _path;
            _parent = _parent.parent;
        }

        return _path;
    }

    public static void ResetTransformValues(this GameObject _target)
    {
        _target.transform.ResetTransformValues();
    }

    public static void ResetTransformValues(this Transform _target)
    {
        _target.position = Vector3.zero;
        _target.rotation = Quaternion.identity;
        _target.localScale = Vector3.one;
    }

    public static void ResetLocalTransformValues(this GameObject _target)
    {
        _target.transform.ResetLocalTransformValues();
    }

    public static void ResetLocalTransformValues(this Transform _target)
    {
        _target.localPosition = Vector3.zero;
        _target.localRotation = Quaternion.identity;
        _target.localScale = Vector3.one;
    }

    public static void CopyTransformValuesFrom(this GameObject _destination, GameObject _source)
    {
        _destination.transform.CopyTransformValuesFrom(_source.transform);
    }

    public static void CopyTransformValuesFrom(this Transform _destination, Transform _source)
    {
        _destination.transform.position = _source.transform.position;
        _destination.transform.rotation = _source.transform.rotation;
        _destination.transform.localScale = _source.transform.localScale;
    }

    public static void CopyLocalTransformValuesFrom(this GameObject _destination, GameObject _source)
    {
        _destination.transform.CopyLocalTransformValuesFrom(_source.transform);
    }

    public static void CopyLocalTransformValuesFrom(this Transform _destination, Transform _source)
    {
        _destination.transform.localPosition = _source.transform.localPosition;
        _destination.transform.localRotation = _source.transform.localRotation;
        _destination.transform.localScale = _source.transform.localScale;
    }

    public static void EnsureIsInState(this GameObject _gameObject, bool _enabled)
    {
        if (_gameObject == null)
        {
            return;
        }

        if (_gameObject.activeSelf != _enabled)
        {
            _gameObject.SetActive(_enabled);
        }
    }

    public static void EnsureEnabled(this GameObject _gameObject)
    {
        _gameObject.EnsureIsInState(true);
    }

    public static void EnsureGameObjectIsInState<T>(this T _component, bool _enabled) where T : Component
    {
        _component.gameObject.EnsureIsInState(_enabled);
    }

    public static void EnsureDisabled(this GameObject _gameObject)
    {
        _gameObject.EnsureIsInState(false);
    }

    public static void EnsureGameObjectDisabled<T>(this T _component) where T : Component
    {
        _component.EnsureGameObjectIsInState(false);
    }

    public static void EnsureGameObjectEnabled<T>(this T _component) where T : Component
    {
        _component.EnsureGameObjectIsInState(true);
    }

    public static void EnsureBehaviourIsInState<T>(this T _behaviour, bool _enabled) where T : Behaviour
    {
        if (_behaviour == null)
        {
            return;
        }

        if (_behaviour.enabled != _enabled)
        {
            _behaviour.enabled = _enabled;
        }
    }

    public static void EnsureEnabled<T>(this T _behaviour) where T : Behaviour
    {
        _behaviour.EnsureBehaviourIsInState(true);
    }

    public static void EnsureDisabled<T>(this T _behaviour) where T : Behaviour
    {
        _behaviour.EnsureBehaviourIsInState(false);
    }

    public static void SetLayer(this Transform _transform, string _layer, bool _includeChildren = true)
    {
        _transform.SetLayer(LayerMask.NameToLayer(_layer), _includeChildren);
    }

    public static void SetLayer(this GameObject _gameObject, string _layer, bool _includeChildern = true)
    {
        _gameObject.SetLayer(LayerMask.NameToLayer(_layer), _includeChildern);
    }

    public static void SetLayer(this GameObject _gameObject, int _layer, bool _includeChildren = true)
    {
        _gameObject.transform.SetLayer(_layer, _includeChildren);
    }

    public static void SetLayer(this Transform _transform, int _layer, bool _includeChildren = true)
    {
        _transform.gameObject.layer = _layer;

        if (_includeChildren == false)
        {
            return;
        }

        for (int i = _transform.childCount - 1; i >= 0; i--)
        {
            _transform.GetChild(i).SetLayer(_layer);
        }
    }

    public static void SetAnchorMaxX(this RectTransform _rect, float _maxX, bool _setZeroSizeDeltaX = true)
    {
        _rect.anchorMax = _rect.anchorMax.WithX(_maxX);

        if (_setZeroSizeDeltaX)
        {
            _rect.sizeDelta = _rect.sizeDelta.WithX(0f);
        }
    }

    public static void SetAnchorMaxY(this RectTransform _rect, float _maxY, bool _setZeroSizeDeltaX = true)
    {
        _rect.anchorMax = _rect.anchorMax.WithY(_maxY);

        if (_setZeroSizeDeltaX)
        {
            _rect.sizeDelta = _rect.sizeDelta.WithX(0f);
        }
    }

    public static void SetAnchorMinX(this RectTransform _rect, float _minX, bool _setZeroSizeDeltaY = true)
    {
        _rect.anchorMin = _rect.anchorMin.WithX(_minX);

        if (_setZeroSizeDeltaY)
        {
            _rect.sizeDelta = _rect.sizeDelta.WithY(0f);
        }
    }

    public static void SetAnchorMinY(this RectTransform _rect, float _minY, bool _setZeroSizeDeltaY = true)
    {
        _rect.anchorMin = _rect.anchorMin.WithY(_minY);

        if (_setZeroSizeDeltaY)
        {
            _rect.sizeDelta = _rect.sizeDelta.WithY(0f);
        }
    }

    public static void SetAnchors(this RectTransform _rect, Vector2 _anchorMin, Vector2 _anchorMax, bool _resetAnchoredPosition = true, bool _resetSizeDelta = true, Vector2 _newSizeDelta = default)
    {
        _rect.anchorMin = _anchorMin;
        _rect.anchorMax = _anchorMax;

        if (_resetAnchoredPosition)
        {
            _rect.anchoredPosition = Vector2.zero;
        }

        if (_resetSizeDelta)
        {
            _rect.sizeDelta = _newSizeDelta;
        }
    }

    public static void SetAnchorsXY(this RectTransform _rect, MinMax _anchorX, MinMax _anchorY, bool _resetAnchoredPosition = true, bool _resetSizeDelta = true, Vector2 _newSizeDelta = default)
    {
        _rect.SetAnchors(new Vector2(_anchorX.Min, _anchorY.Min), new Vector2(_anchorX.Max, _anchorY.Max), _resetAnchoredPosition, _resetSizeDelta, _newSizeDelta);
    }

    public static void ResetAnchors(this RectTransform _rect, bool _resetAnchoredPosition = true, bool _resetSizeDelta = true, Vector2 _newSizeDelta = default)
    {
        _rect.SetAnchors(Vector2.zero, Vector2.one, _resetAnchoredPosition, _resetSizeDelta, _newSizeDelta);
    }

    public static T WithName<T>(this T _object, string _newName) where T : UnityEngine.Object
    {
        _object.name = _newName;
        return _object;
    }

    public static string GetNameIfNotNull(this UnityEngine.Object _go, string _nullDescription = "Null") => _go != null ? _go.name : _nullDescription;
    public static string GetHashCodeIfNotNull(this object _go, string _nullDescription = "Null") => _go != null ? _go.GetHashCode().ToString() : _nullDescription;

    public static Vector3 GetPointOnRectEdge(this RectTransform _rectTransform, Vector2 _searchDirectionFromCenter)
    {
        if (_searchDirectionFromCenter != Vector2.zero)
        {
            _searchDirectionFromCenter /= Mathf.Max(Mathf.Abs(_searchDirectionFromCenter.x), Mathf.Abs(_searchDirectionFromCenter.y));
        }

        Rect _rect = _rectTransform.rect;
        Vector3 _lossyScale = _rectTransform.lossyScale;

        Vector3 _halfOfSize = _rect.size * 0.5f;
        _halfOfSize.Scale(_lossyScale);

        Vector3 _position = _rectTransform.position;
        _position += (Vector3)_rect.center.MultiplyComponentwise(_lossyScale);

        return _position + _halfOfSize.MultiplyComponentwise((Vector3)_searchDirectionFromCenter);
    }

    public static void DestroyGameObject(this GameObject _gameObject)
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
            return;
        }
#endif

        UnityEngine.Object.Destroy(_gameObject);
    }

    public static void DestroyComponent<T>(this T _component) where T : Component
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            UnityEngine.Object.DestroyImmediate(_component);
            return;
        }
#endif

        UnityEngine.Object.Destroy(_component);
    }

    public static void DestroyObject<T>(this T _object) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            UnityEngine.Object.DestroyImmediate(_object);
            return;
        }
#endif

        UnityEngine.Object.Destroy(_object);
    }

    public static void DestroyGameObject<T>(this T _component) where T : Component
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            UnityEngine.Object.DestroyImmediate(_component.gameObject);
            return;
        }
#endif

        UnityEngine.Object.Destroy(_component.gameObject);
    }

    public static void SetExplicitNavigationInNeighbours<T>(this T _thisSelectable) where T : Selectable
    {
        if (_thisSelectable == null)
        {
            return;
        }

        Navigation _navi = _thisSelectable.navigation;
        Selectable _neighbourSelectable = _navi.selectOnUp;

        if (_neighbourSelectable != null)
        {
            Navigation _itemNavi = _neighbourSelectable.navigation;
            _itemNavi.selectOnDown = _thisSelectable;
            _neighbourSelectable.navigation = _itemNavi;
        }

        _neighbourSelectable = _navi.selectOnDown;

        if (_neighbourSelectable != null)
        {
            Navigation _itemNavi = _neighbourSelectable.navigation;
            _itemNavi.selectOnUp = _thisSelectable;
            _neighbourSelectable.navigation = _itemNavi;
        }

        _neighbourSelectable = _navi.selectOnRight;

        if (_neighbourSelectable != null)
        {
            Navigation _itemNavi = _neighbourSelectable.navigation;
            _itemNavi.selectOnLeft = _thisSelectable;
            _neighbourSelectable.navigation = _itemNavi;
        }

        _neighbourSelectable = _navi.selectOnLeft;

        if (_neighbourSelectable != null)
        {
            Navigation _itemNavi = _neighbourSelectable.navigation;
            _itemNavi.selectOnRight = _thisSelectable;
            _neighbourSelectable.navigation = _itemNavi;
        }
    }

    public static void SetExplicitNavigation(this Selectable _selectable, Selectable _up = null, Selectable _down = null, Selectable _right = null, Selectable _left = null)
    {
        if (_selectable == null)
        {
            return;
        }

        Navigation _navi = _selectable.navigation;

        if (_up != null)
        {
            _navi.selectOnUp = _up;
        }

        if (_down != null)
        {
            _navi.selectOnDown = _down;
        }

        if (_right != null)
        {
            _navi.selectOnRight = _right;
        }

        if (_left != null)
        {
            _navi.selectOnLeft = _left;
        }

        _selectable.navigation = _navi;
    }

    public static List<T> FindInterfacesInAllScenes<T>(bool _includeInactive)
    {
        List<T> _found = new List<T>();
        int _sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < _sceneCount; i++)
        {
            GameObject[] _roots = SceneManager.GetSceneAt(i).GetRootGameObjects();

            foreach (var _root in _roots)
            {
                Behaviour[] _behaviours = _root.GetComponentsInChildren<Behaviour>(_includeInactive);

                foreach (var _behaviour in _behaviours)
                {
                    if (_behaviour is T _interface)
                    {
                        _found.Add(_interface);
                    }
                }
            }
        }

        return _found;
    }

    public static List<T> FindInterfacesInScene<T>(this Scene _scene, bool _includeInactive)
    {
        List<T> _found = new List<T>();
        GameObject[] _roots = _scene.GetRootGameObjects();

        foreach (var _root in _roots)
        {
            Behaviour[] _behaviours = _root.GetComponentsInChildren<Behaviour>(_includeInactive);

            foreach (var _behaviour in _behaviours)
            {
                if (_behaviour is T _interface)
                {
                    _found.Add(_interface);
                }
            }
        }

        return _found;
    }

    public static List<T> FindInterfacesInObjectsWithTag<T>(string _tag)
    {
        List<T> _found = new List<T>();
        GameObject[] _objectsWithTag = UnityEngine.GameObject.FindGameObjectsWithTag(_tag);

        foreach (var _obj in _objectsWithTag)
        {
            T[] _interfaces = _obj.GetComponents<T>();

            for (int i = 0; i < _interfaces.Length; i++)
            {
                _found.Add(_interfaces[i]);
            }
        }

        return _found;
    }

    public static void SetPreferedSize<T>(this T _layoutElement, Vector2 _size) where T : LayoutElement
    {
        _layoutElement.preferredWidth = _size.x;
        _layoutElement.preferredHeight = _size.y;
    }

    public static void EnableIfSpriteExist(this Image _image)
    {
        if (_image != null)
        {
            _image.enabled = _image.sprite != null;
        }
    }

    public static T GetComponentInChildren<T>(this Component _root, bool _includeInactive, bool _breadthFirst = false) where T : Component
    {
        return _root?.gameObject.GetComponentInChildren<T>(_includeInactive, _breadthFirst);
    }

    public static T GetComponentInChildren<T>(this GameObject _root, bool _includeInactive = false, bool _breadthFirst = false) where T : Component
    {
        if (_root == null)
        {
            return null;
        }

        if (_breadthFirst == false) //Unit GetComponentInChildren is using depth-first algorithm
        {
            return _root.GetComponentInChildren<T>(_includeInactive);
        }

        Queue<GameObject> _queue = new Queue<GameObject>();
        _queue.Enqueue(_root);

        T _foundComponent = null;
        GameObject _objToCheck;

        while (_queue.Count > 0)
        {
            _objToCheck = _queue.Dequeue();

            if (_includeInactive == false && _objToCheck.activeSelf == false)
            {
                continue;
            }

            if (_objToCheck.TryGetComponent(out _foundComponent))
            {
                break;
            }

            for (int i = 0; i < _objToCheck.transform.childCount; i++)
            {
                _queue.Enqueue(_objToCheck.transform.GetChild(i).gameObject);
            }
        }

        return _foundComponent;
    }
}
