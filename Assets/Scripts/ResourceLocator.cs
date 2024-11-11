using System;
using UnityEngine;

public class ResourceLocator : MonoBehaviour
{
    public event Action<Resource> ResourceLocated;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Resource resource))
            ResourceLocated?.Invoke(resource);
    }
}
