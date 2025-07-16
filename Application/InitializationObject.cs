using System.Collections;
using UnityEngine;

public class InitializationObject : ScriptableObject, IInitializationModule
{
    public virtual IEnumerator HandleModuleInitialization()
    {
        yield break;
    }
}
