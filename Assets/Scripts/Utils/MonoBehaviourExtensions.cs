using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static void TryStopCoroutine(this MonoBehaviour monoBehaviour, Coroutine coroutine)
    {
        if (coroutine != null)
            monoBehaviour.StopCoroutine(coroutine);
    }
}
