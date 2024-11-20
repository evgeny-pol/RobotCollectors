using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ResourceStorage : MonoBehaviour
{
    [SerializeField, Min(0)] private int _resourceCount;

    public event Action<int> ResourceCountChanged;

    public int ResourceCount => _resourceCount;

    public void AddResource(Resource resource)
    {
        _resourceCount += resource.Value;
        ResourceCountChanged?.Invoke(_resourceCount);
        resource.Deactivate();
    }

    public void RemoveResources(int count)
    {
        Assert.IsTrue(_resourceCount >= count);
        _resourceCount -= count;
        ResourceCountChanged?.Invoke(_resourceCount);
    }
}
