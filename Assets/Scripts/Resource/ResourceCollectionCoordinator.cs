using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceCollectionCoordinator : MonoBehaviour
{
    [SerializeField] private List<ResourceBase> _resourceBases;

    private readonly List<Resource> _unassignedResources = new();
    private readonly List<Resource> _assignedResources = new();

    private void OnEnable()
    {
        foreach (ResourceBase resourceBase in _resourceBases)
            SubscribeToEvents(resourceBase);
    }

    private void OnDisable()
    {
        foreach (ResourceBase resourceBase in _resourceBases)
            UnsubscribeFromEvents(resourceBase);
    }

    private void OnResourceLocated(Resource resource)
    {
        if (_unassignedResources.Contains(resource) || _assignedResources.Contains(resource))
            return;

        _unassignedResources.Add(resource);
        ResourceBase idleBase = _resourceBases.Where(resourceBase => resourceBase.HasIdleWorkers())
            .OrderBy(resourceBase => resourceBase.transform.position.SquaredDistanceTo(resource.transform.position))
            .FirstOrDefault();

        if (idleBase == null)
            return;

        CollectResource(idleBase, resource);
    }

    public void OnResourceCollected(Resource resource)
    {
        _assignedResources.Remove(resource);
    }

    private void OnIdleWorker(ResourceBase resourceBase)
    {
        Resource resource = _unassignedResources.OrderBy(
            resource => resource.transform.position.SquaredDistanceTo(resourceBase.transform.position))
            .FirstOrDefault();

        if (resource != null)
            CollectResource(resourceBase, resource);
    }

    private void OnResourceBaseBuilt(ResourceBase resourceBase)
    {
        SubscribeToEvents(resourceBase);
        _resourceBases.Add(resourceBase);
    }

    private void SubscribeToEvents(ResourceBase resourceBase)
    {
        resourceBase.ResourceLocated += OnResourceLocated;
        resourceBase.ResourceCollected += OnResourceCollected;
        resourceBase.WorkerIsIdle += OnIdleWorker;
        resourceBase.ResourceBaseBuilt += OnResourceBaseBuilt;
    }

    private void UnsubscribeFromEvents(ResourceBase resourceBase)
    {
        resourceBase.ResourceLocated -= OnResourceLocated;
        resourceBase.ResourceCollected -= OnResourceCollected;
        resourceBase.WorkerIsIdle -= OnIdleWorker;
        resourceBase.ResourceBaseBuilt -= OnResourceBaseBuilt;
    }

    private void CollectResource(ResourceBase resourceBase, Resource resource)
    {
        resourceBase.CollectResource(resource);
        _unassignedResources.Remove(resource);
        _assignedResources.Add(resource);
    }
}
