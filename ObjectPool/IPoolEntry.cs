namespace DL.ObjectPool
{
    using UnityEngine;

    [System.Serializable]
    public sealed class IPoolEntry<T> where T : Component, IPoolObject<T>
    {
        public GameObject GameObject { get; private set; } = null;
        public T Component { get; private set; } = null;

        public IPoolEntry(GameObject _go)
        {
            if (_go == null)
            {
                throw new System.ArgumentNullException(nameof(_go), "GameObject cannot be null.");
            }

            GameObject = _go;
            Component = _go.GetComponent<T>();

            if (Component == null)
            {
                throw new System.ArgumentException("GameObject does not contain the required component.", nameof(_go));
            }
        }
    }
}
