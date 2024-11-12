using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(ResourceStorage))]
public class ResourceBase : MonoBehaviour, IColliderOwner
{
    [SerializeField] private ResourceLocator _resourceLocator;
    [SerializeField] private List<ResourceCollector> _resourceCollectors;

    private readonly List<Resource> _assignedResources = new();

    private Collider _collider;
    private ResourceStorage _resourceStorage;

    public Collider Collider => _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _resourceStorage = GetComponent<ResourceStorage>();
    }

    private void OnEnable()
    {
        _resourceLocator.ResourceLocated += OnResourceLocated;
    }

    private void OnDisable()
    {
        _resourceLocator.ResourceLocated -= OnResourceLocated;
    }

    public void Collect(Resource resource)
    {
        _assignedResources.Remove(resource);
        _resourceStorage.AddResource(resource);
    }

    private void OnResourceLocated(Resource resource)
    {
        if (_assignedResources.Contains(resource))
            return;

        ResourceCollector freeCollector = _resourceCollectors.Where(collector => collector.IsIdle)
            .OrderBy(collector => (resource.transform.position - collector.transform.position).sqrMagnitude)
            .FirstOrDefault();

        if (freeCollector == null)
            return;

        freeCollector.Collect(resource, this);
        _assignedResources.Add(resource);
    }
}
