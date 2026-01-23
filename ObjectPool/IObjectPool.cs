namespace DL.ObjectPool
{
    using UnityEngine;
    using DL.Interfaces;

    public interface IObjectPool<IComponent, Key> : IObjectPool<IComponent>, IKeyContainer<Key>
        where IComponent : Component, IPoolObject<IComponent>
    {

    }

    public interface IObjectPool<IComponent>
        where IComponent : Component, IPoolObject<IComponent>
    {
        public IPoolEntry<IComponent> GetObject(bool _getActive);
        public void ReturnObject(IPoolEntry<IComponent> _poolEntry);

        public void AdjustPoolSize(int _newMinimumSize);
        public void PreloadObjects(int _count);
        public void UnloadObjects(bool _destroyUsedObjects);
        public void ClearPool();
        public void ReturnAllObjects();
    }
}
