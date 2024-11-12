using System;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    [SerializeField, Min(0)] private int _resourcesCount;

    public event Action<int> ResourceCountChanged;

    public void AddResource(Resource resource)
    {
        _resourcesCount += resource.Value;
        ResourceCountChanged?.Invoke(_resourcesCount);
        resource.Deactivate();
    }
}
