namespace DL.Structs
{
    using System;
    using UnityEngine;

    [System.Serializable]
    public struct SerializableGUID : IGUIDProvider
    {
        [SerializeField] private string guid;

        public Guid GUID => this;

        public SerializableGUID(string _guid)
        {
            guid = _guid;
        }

        public SerializableGUID(System.Guid _guid)
        {
            guid = _guid.ToString();
        }

        public void GenerateNewGUID()
        {
            guid = Guid.NewGuid().ToString();
        }

        public override string ToString() => guid;

        public static implicit operator string(SerializableGUID _serializedGUID) => _serializedGUID.guid;
        public static implicit operator SerializableGUID(string _guid) => new SerializableGUID(_guid);
        public static implicit operator SerializableGUID(Guid _guid) => new SerializableGUID(_guid.ToString());
        public static implicit operator Guid(SerializableGUID _serializedGUID) => Guid.Parse(_serializedGUID.guid);

        public static SerializableGUID NewGUID() => new SerializableGUID(Guid.NewGuid());
    }

    public interface IGUIDProvider
    {
        Guid GUID { get; }
    }

    public static class GUIDProviderExtensions
    {
        public static void LogGUID<T>(this T _guidProvider, string _message = null) where T : IGUIDProvider
        {
            string _guidString = _guidProvider != null
                ? _guidProvider.GUID.ToString() 
                : $"Missing Object To Type: {typeof(T)}";

            Debug.Log($"GUID: {_guidString} :: {_message}");
        }
    }
}
