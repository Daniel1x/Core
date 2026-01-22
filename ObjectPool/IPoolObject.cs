namespace DL.ObjectPool
{
    using UnityEngine;

    public interface IPoolObject<T> where T : Component, IPoolObject<T>
    {
        public event System.Action<IPoolEntry<T>> OnDestroyed;

        public IPoolEntry<T> Entry { get; }

        public void InitializePoolObject(IPoolEntry<T> _entry);
        public void ResetPoolObject();
    }
}
