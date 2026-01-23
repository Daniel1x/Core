namespace DL.ObjectPool
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using DL.Behaviours;

    [System.Serializable]
    public class ComponentObjectPool<IComponent, IKey> : IObjectPool<IComponent, IKey>
        where IComponent : Component, IPoolObject<IComponent>
    {
        protected Transform parent;
        protected AssetReference assetReference;
        protected Queue<IPoolEntry<IComponent>> availableObjects;
        protected HashSet<IPoolEntry<IComponent>> inUseObjects;
        protected IKey key;

        public IKey Key => key;

        public ComponentObjectPool(IKey _key, AssetReference _assetReference, int _initialSize = 0)
        {
            if (_assetReference == null || !_assetReference.RuntimeKeyIsValid())
            {
                throw new System.ArgumentException("Invalid AssetReference provided for pool initialization.", nameof(_assetReference));
            }

            GameObject _parentObject = new GameObject($"ComponentObjectPool<{typeof(IComponent).Name}>[{_key}]");
            Object.DontDestroyOnLoad(_parentObject);
            parent = _parentObject.transform;
            var _behaviour = _parentObject.AddComponent<OnDestroyEventCaller>();
            _behaviour.OnBehaviourDestroyed += onParentBehaviourDestroyed;

            key = _key;
            assetReference = _assetReference;
            availableObjects = new Queue<IPoolEntry<IComponent>>(_initialSize);
            inUseObjects = new HashSet<IPoolEntry<IComponent>>(_initialSize);

            PreloadObjects(_initialSize);
        }

        public IPoolEntry<IComponent> GetObject(bool _getActive)
        {
            IPoolEntry<IComponent> _poolEntry = availableObjects.Count > 0
                ? availableObjects.Dequeue()
                : createNewPoolEntry();

            if (_poolEntry.GameObject.transform.parent != parent)
            {
                _poolEntry.GameObject.transform.SetParent(parent, false);
            }

            _poolEntry.GameObject.SetActive(_getActive);

            inUseObjects.Add(_poolEntry);
            return _poolEntry;
        }

        public void ReturnObject(IPoolEntry<IComponent> _poolEntry)
        {
            if (_poolEntry == null)
            {
                return;
            }

            if (inUseObjects.Remove(_poolEntry))
            {
                if (_poolEntry.GameObject.transform.parent != parent)
                {
                    _poolEntry.GameObject.transform.SetParent(parent, false);
                }

                _poolEntry.GameObject.SetActive(false);
                _poolEntry.Component.ResetPoolObject();

                availableObjects.Enqueue(_poolEntry);
            }
        }

        public void AdjustPoolSize(int _newMinimumSize)
        {
            if (_newMinimumSize <= 0)
            {
                return;
            }

            int _currentTotalSize = availableObjects.Count + inUseObjects.Count;

            if (_newMinimumSize > _currentTotalSize)
            {
                int _objectsToAdd = _newMinimumSize - _currentTotalSize;
                PreloadObjects(_objectsToAdd);
            }
        }

        public void PreloadObjects(int _count)
        {
            for (int i = 0; i < _count; i++)
            {
                availableObjects.Enqueue(createNewPoolEntry());
            }
        }

        public void UnloadObjects(bool _destroyUsedObjects)
        {
            foreach (IPoolEntry<IComponent> _poolEntry in availableObjects)
            {
                if (_poolEntry.Component != null)
                {
                    _poolEntry.Component.OnDestroyed -= onEntryDestroyed;
                }

                if (_poolEntry.GameObject != null)
                {
                    Object.Destroy(_poolEntry.GameObject);
                }
            }

            availableObjects.Clear();

            if (!_destroyUsedObjects)
            {
                return;
            }

            foreach (IPoolEntry<IComponent> _poolEntry in inUseObjects)
            {
                if (_poolEntry.Component != null)
                {
                    _poolEntry.Component.OnDestroyed -= onEntryDestroyed;
                }

                if (_poolEntry.GameObject != null)
                {
                    Object.Destroy(_poolEntry.GameObject);
                }
            }

            inUseObjects.Clear();
        }

        public void ClearPool()
        {
            UnloadObjects(true);
            inUseObjects.Clear();
        }

        public void ReturnAllObjects()
        {
            foreach (IPoolEntry<IComponent> _poolEntry in inUseObjects)
            {
                if (_poolEntry.GameObject.transform.parent != parent)
                {
                    _poolEntry.GameObject.transform.SetParent(parent, false);
                }

                _poolEntry.GameObject.SetActive(false);
                _poolEntry.Component.ResetPoolObject();

                availableObjects.Enqueue(_poolEntry);
            }

            inUseObjects.Clear();
        }

        private void onParentBehaviourDestroyed(OnDestroyEventCaller _caller)
        {
            ClearPool();

            if (_caller != null)
            {
                _caller.OnBehaviourDestroyed -= onParentBehaviourDestroyed;
            }
        }

        protected IPoolEntry<IComponent> createNewPoolEntry()
        {
            GameObject _loadedGameObject = assetReference.InstantiateAsync().WaitForCompletion();
            _loadedGameObject.transform.SetParent(parent, false);
            _loadedGameObject.SetActive(false);

            var _newEntry = new IPoolEntry<IComponent>(_loadedGameObject);
            _newEntry.Component.InitializePoolObject(_newEntry, this);
            _newEntry.Component.OnDestroyed += onEntryDestroyed;

            return _newEntry;
        }

        private void onEntryDestroyed(IPoolEntry<IComponent> _entry)
        {
            if (_entry == null)
            {
                return;
            }

            inUseObjects.Remove(_entry);

            if (_entry.Component != null)
            {
                _entry.Component.OnDestroyed -= onEntryDestroyed;
            }
        }
    }
}
