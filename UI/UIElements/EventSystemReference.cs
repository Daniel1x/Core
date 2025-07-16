using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public static class EventSystemReference
{
    //Only one event system should be available, and this should be the only place that references it!
    //The reference is cached because when eventSystem is disabled for any reason, EventSystem.current points to null.
    private static EventSystem eventSystem = null;

    public static EventSystem Instance
    {
        get
        {
            if (eventSystem == null)
            {
                eventSystem = EventSystem.current;
            }

            return eventSystem;
        }
    }

    public static bool IsAvailable => Instance != null;
    public static bool IsAvailableAndEnabled => IsAvailable && eventSystem.enabled;
    public static bool SendNavigationEvents
    {
        get => IsAvailable && eventSystem.sendNavigationEvents;
        set
        {
            if (IsAvailable)
            {
                eventSystem.sendNavigationEvents = value;
            }
        }
    }

    public static bool IsMouseDown => Mouse.current?.leftButton is ButtonControl _button && _button.isPressed;

    public static GameObject SelectedGameObject
    {
        get => IsAvailable ? eventSystem.currentSelectedGameObject : null;
        set
        {
            if (IsAvailable)
            {
                eventSystem.SetSelectedGameObject(value);
            }
        }
    }

    public static void InitializeEventSystem(EventSystem _eventSystem)
    {
        if (_eventSystem != null)
        {
            eventSystem = _eventSystem;
        }
    }

    public static bool IsObjectSelected(GameObject _object)
    {
        return IsAvailable && Instance.currentSelectedGameObject == _object;
    }

    public static bool IsAnyObjectSelected(params GameObject[] _objects)
    {
        if (IsAvailable == false
            || eventSystem.currentSelectedGameObject == null
            || _objects == null
            || _objects.Length == 0)
        {
            return false;
        }

        GameObject _selected = eventSystem.currentSelectedGameObject;

        for (int i = 0; i < _objects.Length; i++)
        {
            if (_objects[i] == _selected)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsAnyObjectSelected<C>(params C[] _components) where C : Component
    {
        if (IsAvailable == false
            || eventSystem.currentSelectedGameObject == null
            || _components == null
            || _components.Length == 0)
        {
            return false;
        }

        GameObject _selected = eventSystem.currentSelectedGameObject;

        for (int i = 0; i < _components.Length; i++)
        {
            if (_components[i] != null && _components[i].gameObject == _selected)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsAnyObjectSelected<C>(List<C> _components) where C : Component
    {
        if (IsAvailable == false
            || eventSystem.currentSelectedGameObject == null
            || _components == null
            || _components.Count == 0)
        {
            return false;
        }

        GameObject _selected = eventSystem.currentSelectedGameObject;

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] != null && _components[i].gameObject == _selected)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsSelectingComponent<C>(out C _component) where C : Component
    {
        if (IsAvailable == false || eventSystem.currentSelectedGameObject == null)
        {
            _component = null;
            return false;
        }

        return eventSystem.currentSelectedGameObject.TryGetComponent(out _component);
    }

    public static bool IsObjectSelectedByEventSystem(this GameObject _go) => IsObjectSelected(_go);

    public static bool IsObjectSelectedByEventSystem<C>(this C _component) where C : Component => IsObjectSelected(_component.gameObject);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init()
    {
        eventSystem = null;
    }
}
