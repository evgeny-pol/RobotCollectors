using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RobotMover))]
public class ResourceCollector : MonoBehaviour
{
    [SerializeField] RobotAnimator _animator;
    [SerializeField] private Transform _resourceAttachPoint;

    private Resource _targetResource;
    private ResourceBase _targetBase;
    private RobotMover _robotMover;

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

    public void Collect(Resource resource, ResourceBase resourceBase)
    {
        _targetResource = resource;
        _targetBase = resourceBase;
        IsIdle = false;
        StartCoroutine(MoveToResource());
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
        _targetBase.Collect(_targetResource);
    }

    private void OnObjectPlaced()
    {
        _targetResource = null;
        _targetBase = null;
        IsIdle = true;
    }

    private IEnumerator MoveToResource()
    {
        yield return _robotMover.MoveToObject(_targetResource);
        _animator.PickUpObject();
    }

    private IEnumerator MoveToBase()
    {
        yield return _robotMover.MoveToObject(_targetBase);
        _animator.PlaceDownObject();
    }
}
