using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RobotMover))]
public class Worker : MonoBehaviour
{
    [SerializeField] private RobotAnimator _animator;
    [SerializeField] private Transform _resourceAttachPoint;
    [SerializeField] private ResourceBase _resourceBasePrefab;
    [SerializeField] private ParticleSystem _spawnParticles;

    private Resource _targetResource;
    private IColliderOwner _dropOffDestination;
    private RobotMover _robotMover;

    public event Action<Resource> ResourceDelivered;
    public event Action<ResourceBase, Worker> ResourceBaseBuilt;
    public event Action<Worker> BecameIdle;

    public bool IsIdle { get; private set; } = true;

    private void Awake()
    {
        _robotMover = GetComponent<RobotMover>();
    }

    private void OnEnable()
    {
        _animator.ObjectGrabbed += OnObjectGrabbed;
        _animator.ObjectPicked += OnObjectPicked;
        _animator.ObjectReleased += OnObjectReleased;
        _animator.ObjectPlaced += OnObjectPlaced;
    }

    private void OnDisable()
    {
        _animator.ObjectGrabbed -= OnObjectGrabbed;
        _animator.ObjectPicked -= OnObjectPicked;
        _animator.ObjectReleased -= OnObjectReleased;
        _animator.ObjectPlaced -= OnObjectPlaced;
    }

    public void PlaySpawnEffects()
    {
        _spawnParticles.Play();
    }

    public void Collect(Resource resource, IColliderOwner destination)
    {
        _targetResource = resource;
        _dropOffDestination = destination;
        IsIdle = false;
        StartCoroutine(MoveToResource());
    }

    public void BuildResourceBase(BuildingPlan buildingPlan)
    {
        IsIdle = false;
        StartCoroutine(MoveAndBuildResourceBase(buildingPlan));
    }

    private void OnObjectGrabbed()
    {
        _targetResource.Attach(_resourceAttachPoint);
    }

    private void OnObjectPicked()
    {
        StartCoroutine(MoveToBase());
    }

    private void OnObjectReleased()
    {
        _targetResource.Detach();
        ResourceDelivered?.Invoke(_targetResource);
        _targetResource = null;
        _dropOffDestination = null;
    }

    private void OnObjectPlaced()
    {
        IsIdle = true;
        BecameIdle?.Invoke(this);
    }

    private IEnumerator MoveToResource()
    {
        yield return _robotMover.MoveToObject(_targetResource);
        _animator.PickUpObject();
    }

    private IEnumerator MoveToBase()
    {
        yield return _robotMover.MoveToObject(_dropOffDestination);
        _animator.PlaceDownObject();
    }

    private IEnumerator MoveAndBuildResourceBase(BuildingPlan buildingPlan)
    {
        yield return _robotMover.MoveToObject(buildingPlan);
        ResourceBase newBase = Instantiate(_resourceBasePrefab, buildingPlan.transform.position, buildingPlan.transform.rotation);
        IsIdle = true;
        ResourceBaseBuilt?.Invoke(newBase, this);
        BecameIdle?.Invoke(this);
    }
}
