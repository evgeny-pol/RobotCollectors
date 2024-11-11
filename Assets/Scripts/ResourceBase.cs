using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ResourceBase : MonoBehaviour
{
    [SerializeField, Min(0)] private int _resources;
    [SerializeField] private ResourceLocator _resourceLocator;
    [SerializeField] private TextMeshPro _resourcesCountText;
    [SerializeField] private List<ResourceCollector> _resourceCollectors;

    private readonly List<Resource> _assignedResources = new();

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
        _resources += resource.Value;
        _resourcesCountText.text = _resources.ToString();
        resource.Deactivate();
        _assignedResources.Remove(resource);
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
