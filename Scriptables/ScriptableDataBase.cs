namespace DL.Scriptables
{
    using UnityEngine;

    public abstract class ScriptableDataBase<T> : ScriptableObject
    {
        public static implicit operator T(ScriptableDataBase<T> _data) => _data.GetValue();

        public abstract T GetValue();
    }
}
