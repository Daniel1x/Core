using System.Collections;
using System.Collections.Generic;

public interface IInitializationModule
{
    public abstract IEnumerator HandleModuleInitialization();
}

public static class IInitializationModuleUtilities
{
    public static IEnumerator HandleInitialization<T>(this T[] _modules) where T : IInitializationModule
    {
        if (_modules == null)
        {
            yield break;
        }

        for (int i = 0; i < _modules.Length; i++)
        {
            if (_modules[i] == null)
            {
                continue;
            }

            yield return _modules[i].HandleModuleInitialization();
        }
    }

    public static IEnumerator HandleInitialization<T>(this List<T> _modules) where T : IInitializationModule
    {
        if (_modules == null)
        {
            yield break;
        }

        for (int i = 0; i < _modules.Count; i++)
        {
            if (_modules[i] == null)
            {
                continue;
            }

            yield return _modules[i].HandleModuleInitialization();
        }
    }
}
