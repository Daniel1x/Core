using System.Collections;
using UnityEngine;

public class InitializationBehaviour : MonoBehaviour, IInitializationModule
{
    public virtual IEnumerator HandleModuleInitialization()
    {
        yield break;
    }
}
