using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BehaviourCollection<T> where T : Behaviour
{
    public enum Mode
    {
        ToggleGameObject = 0,
        SpawnAndDestroy = 1,
    }

    public event UnityAction<T, int> OnNewItemCreated = null;
    public event UnityAction<T, int> OnBeforeItemDestroyed = null;
    public event UnityAction<Mode, bool, int> OnItemVisibilityChanged = null;

    [SerializeField] protected T prefab = null;
    [SerializeField] protected Transform parent = null;
    [SerializeField] protected Mode mode = Mode.SpawnAndDestroy;

    [System.NonSerialized] public List<T> AllItems = new List<T>();

    protected int visibleItems = 0;

    public T this[int _index] => AllItems[_index];
    public T this[System.Index _index] => AllItems[_index];

    public T Prefab { get => prefab; set => prefab = value; }
    public Transform Parent { get => parent; set => parent = value; }
    public Mode CollectionMode { get => mode; set => mode = value; }

    public int VisibleItems
    {
        get => visibleItems;
        set
        {
            UpdateVisibleItemsCount(value);
        }
    }

    public void UpdateVisibleItemsCount(int _count)
    {
        visibleItems = _count;
        int _currentCount = AllItems.Count;

        if (_count > _currentCount)
        {
            int _itemsToSpawn = _count - _currentCount;

            for (int i = 0; i < _itemsToSpawn; i++)
            {
                T _newItem = Object.Instantiate(prefab, parent);
                AllItems.Add(_newItem);
                OnNewItemCreated?.Invoke(_newItem, _currentCount + i);
            }

            _currentCount = _count;
        }

        for (int i = _currentCount - 1; i >= 0; i--)
        {
            setItemVisible(i < _count, i);
        }
    }

    private void setItemVisible(bool _visible, int _itemID)
    {
        if (_visible)
        {
            AllItems[_itemID].gameObject.SetActive(true);
        }
        else
        {
            if (mode is Mode.SpawnAndDestroy)
            {
                OnBeforeItemDestroyed?.Invoke(AllItems[_itemID], _itemID);
#if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    Object.DestroyImmediate(AllItems[_itemID].gameObject);
                }
                else
#endif
                {
                    Object.Destroy(AllItems[_itemID].gameObject);
                }

                AllItems.RemoveAt(_itemID);
            }
            else //ToggleGameObject
            {
                AllItems[_itemID].gameObject.SetActive(false);
            }
        }

        OnItemVisibilityChanged?.Invoke(mode, _visible, _itemID);
    }
}
