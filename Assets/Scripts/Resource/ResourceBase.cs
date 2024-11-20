using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(ResourceStorage))]
public class ResourceBase : Building, IColliderOwner, ISelectable
{
    [SerializeField, Min(1)] private int _workerCost = 3;
    [SerializeField, Min(1)] private int _newBaseCost = 5;
    [SerializeField] private Transform _unitSpawnPoint;
    [SerializeField] private Worker _workerPrefab;
    [SerializeField] private ResourceLocator _resourceLocator;
    [SerializeField] private ParticleSystem _highlightParticles;
    [SerializeField] private LayerMask _buildingLayers;
    [SerializeField] private BuildingPlan _resourceBasePlanPrefab;
    [SerializeField] private List<Worker> _workers;

    private BuildingPlan _newBasePlan;
    private Worker _newBaseBuilder;
    private Collider _collider;
    private ResourceStorage _resourceStorage;

    public event Action<Resource> ResourceLocated;
    public event Action<Resource> ResourceCollected;
    public event Action<ResourceBase> WorkerIsIdle;
    public event Action<ResourceBase> ResourceBaseBuilt;
    public event Action<Vector3?> OrderPositionChanged;

    public Collider Collider => _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _resourceStorage = GetComponent<ResourceStorage>();
    }

    private void OnEnable()
    {
        _resourceLocator.ResourceLocated += OnResourceLocated;

        foreach (Worker worker in _workers)
            SubscribeToEvents(worker);
    }

    private void OnDisable()
    {
        _resourceLocator.ResourceLocated -= OnResourceLocated;

        foreach (Worker worker in _workers)
            UnsubscribeFromEvents(worker);
    }

    public bool HasIdleWorkers() => GetIdleWorkers().Any();

    public bool TryOrder(Vector3 targetPosition, out string errorMessage)
    {
        if (_workers.Count < 2)
        {
            errorMessage = Texts.NotEnoughWorkersForNewBase;
            return false;
        }

        Bounds colliderBounds = _collider.bounds;
        Vector3 targetOffset = targetPosition - transform.position;
        Vector3 newColliderCenter = colliderBounds.center + targetOffset;

        if (_newBasePlan == null)
        {
            if (Physics.CheckBox(newColliderCenter, colliderBounds.extents, Quaternion.identity, _buildingLayers))
            {
                errorMessage = Texts.NotEnoughFreeSpaceForNewBuilding;
                return false;
            }

            _newBasePlan = Instantiate(_resourceBasePlanPrefab, targetPosition, transform.rotation);
            OrderPositionChanged?.Invoke(targetPosition);
            TrySpendResources();
            errorMessage = string.Empty;
            return true;
        }

        foreach (Collider collider in Physics.OverlapBox(newColliderCenter, colliderBounds.extents, Quaternion.identity, _buildingLayers))
        {
            if (collider.gameObject != _newBasePlan.gameObject)
            {
                errorMessage = Texts.NotEnoughFreeSpaceForNewBuilding;
                return false;
            }
        }

        _newBasePlan.transform.position = targetPosition;
        OrderPositionChanged?.Invoke(targetPosition);
        errorMessage = string.Empty;
        return true;
    }

    public void CollectResource(Resource resource)
    {
        Worker idleWorker = GetClosestIdleWorker(resource.transform.position);
        idleWorker.Collect(resource, this);
    }

    public void AddWorker(Worker worker)
    {
        SubscribeToEvents(worker);
        _workers.Add(worker);

        if (worker.IsIdle)
            WorkerIsIdle?.Invoke(this);
    }

    public void Highlight()
    {
        _highlightParticles.Play();
    }

    public void Dehighlight()
    {
        _highlightParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void SubscribeToEvents(Worker worker)
    {
        worker.ResourceDelivered += OnResourceCollected;
        worker.ResourceBaseBuilt += OnResourceBaseBuilt;
        worker.BecameIdle += OnWorkerIdle;
    }

    private void UnsubscribeFromEvents(Worker worker)
    {
        worker.ResourceDelivered -= OnResourceCollected;
        worker.ResourceBaseBuilt -= OnResourceBaseBuilt;
        worker.BecameIdle -= OnWorkerIdle;
    }

    private void OnResourceLocated(Resource resource)
    {
        ResourceLocated?.Invoke(resource);
    }

    private void OnResourceCollected(Resource resource)
    {
        _resourceStorage.AddResource(resource);
        ResourceCollected?.Invoke(resource);
        TrySpendResources();
    }

    private void OnResourceBaseBuilt(ResourceBase newBase, Worker builder)
    {
        Assert.IsNotNull(_newBasePlan);
        Assert.AreEqual(builder, _newBaseBuilder);
        _newBaseBuilder = null;
        Destroy(_newBasePlan.gameObject);
        OrderPositionChanged?.Invoke(null);
        UnsubscribeFromEvents(builder);
        _workers.Remove(builder);
        newBase.AddWorker(builder);
        ResourceBaseBuilt?.Invoke(newBase);
    }

    private void OnWorkerIdle(Worker worker)
    {
        TrySpendResources();

        if (worker.IsIdle)
            WorkerIsIdle?.Invoke(this);
    }

    private void TrySpendResources()
    {
        if (_newBasePlan != null && _newBaseBuilder == null)
        {
            if (_resourceStorage.ResourceCount < _newBaseCost)
                return;

            Worker idleWorker = TryGetIdleWorker();

            if (idleWorker == null)
                return;

            _resourceStorage.RemoveResources(_newBaseCost);
            idleWorker.BuildResourceBase(_newBasePlan);
            _newBaseBuilder = idleWorker;
        }

        while (_resourceStorage.ResourceCount >= _workerCost)
        {
            _resourceStorage.RemoveResources(_workerCost);
            Worker newWorker = Instantiate(_workerPrefab, _unitSpawnPoint.position, Quaternion.identity);
            newWorker.PlaySpawnEffects();
            AddWorker(newWorker);
        }
    }

    private IEnumerable<Worker> GetIdleWorkers() => _workers.Where(worker => worker.IsIdle);

    private Worker TryGetIdleWorker() => GetIdleWorkers().FirstOrDefault();

    private Worker GetClosestIdleWorker(Vector3 position) => GetIdleWorkers()
        .OrderBy(worker => worker.transform.position.SquaredDistanceTo(position))
        .First();
}
