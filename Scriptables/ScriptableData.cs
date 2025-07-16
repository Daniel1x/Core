namespace DL.Scriptables
{
    public class ScriptableData<T> : ScriptableDataBase<T>
    {
        public T Value = default;
        public override T GetValue() => Value;
    }
}
