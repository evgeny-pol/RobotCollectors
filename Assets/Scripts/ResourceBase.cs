using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(ResourceStorage))]
public class ResourceBase : MonoBehaviour, IColliderOwner
{
    [SerializeField] private ResourceLocator _resourceLocator;
    [SerializeField] private List<ResourceCollector> _resourceCollectors;

    private readonly List<Resource> _unassignedResources = new();
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

        foreach (ResourceCollector resourceCollector in _resourceCollectors)
        {
            resourceCollector.ResourceDelivered += OnResourceCollected;
            resourceCollector.BecameIdle += OnCollectorIdle;
        }
    }

    private void OnDisable()
    {
        _resourceLocator.ResourceLocated -= OnResourceLocated;

        foreach (ResourceCollector resourceCollector in _resourceCollectors)
        {
            resourceCollector.ResourceDelivered -= OnResourceCollected;
            resourceCollector.BecameIdle -= OnCollectorIdle;
        }
    }

    public void OnResourceCollected(Resource resource)
    {
        _assignedResources.Remove(resource);
        _resourceStorage.AddResource(resource);
    }

    private void OnResourceLocated(Resource resource)
    {
        if (_unassignedResources.Contains(resource) || _assignedResources.Contains(resource))
            return;

        _unassignedResources.Add(resource);
        ResourceCollector resourceCollector = _resourceCollectors.Where(collector => collector.IsIdle)
            .OrderBy(collector => collector.transform.SquaredDistanceTo(resource.transform))
            .FirstOrDefault();

        if (resourceCollector == null)
            return;

        CollectResource(resourceCollector, resource);
    }

    private void OnCollectorIdle(ResourceCollector resourceCollector)
    {
        if (_unassignedResources.Count == 0)
            return;

        Resource resource = _unassignedResources.OrderBy(resource => resource.transform.SquaredDistanceTo(resourceCollector.transform))
            .First();

        CollectResource(resourceCollector, resource);
    }

    private void CollectResource(ResourceCollector resourceCollector, Resource resource)
    {
        resourceCollector.Collect(resource, this);
        _unassignedResources.Remove(resource);
        _assignedResources.Add(resource);
    }
}
