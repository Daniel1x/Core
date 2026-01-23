namespace DL.ObjectPool
{
    using UnityEngine;

    public interface IPoolObject<T> where T : Component, IPoolObject<T>
    {
        public event System.Action<IPoolEntry<T>> OnDestroyed;

        public IPoolEntry<T> Entry { get; }
        public IObjectPool<T> Pool { get; }

        public void InitializePoolObject(IPoolEntry<T> _entry, IObjectPool<T> _pool);
        public void ResetPoolObject();
    }
}
