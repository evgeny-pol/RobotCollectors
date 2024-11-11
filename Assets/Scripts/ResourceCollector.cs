using System.Collections;
using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    [SerializeField, Min(0)] private float _moveSpeed = 1;
    [SerializeField, Min(0)] private float _rotationSpeed = 90;
    [SerializeField] RobotAnimator _animator;
    [SerializeField] private Transform _resourceAttachPoint;

    private Resource _targetResource;
    private ResourceBase _targetBase;
    private GameObject _targetObject;

    public bool IsIdle { get; private set; } = true;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _targetObject)
            _targetObject = null;
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
        yield return MoveToObject(_targetResource.gameObject);
        _animator.PickUpObject();
    }

    private IEnumerator MoveToBase()
    {
        yield return MoveToObject(_targetBase.gameObject);
        _animator.PlaceDownObject();
    }

    private IEnumerator MoveToObject(GameObject targetObject)
    {
        _targetObject = targetObject;
        _animator.StartWalking();

        while (_targetObject != null)
        {
            Vector3 currentPosition = transform.position;
            Vector3 currentDirection = transform.forward;
            Vector3 targetPosition = _targetObject.transform.position;
            targetPosition.y = currentPosition.y;
            Vector3 toTargetPosition = (targetPosition - currentPosition).normalized;
            float directionDiff = Vector3.Angle(currentDirection, toTargetPosition);

            if (directionDiff > 0)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(toTargetPosition), _rotationSpeed * Time.fixedDeltaTime);
            }

            if (directionDiff < MathUtils.QuarterCircleDegrees)
            {
                float moveSpeed = _moveSpeed * (1 - directionDiff / MathUtils.QuarterCircleDegrees);
                transform.Translate(moveSpeed * Time.fixedDeltaTime * toTargetPosition, Space.World);
            }

            yield return CoroutineUtils.WaitForFixedUpdate;
        }

        _animator.StopWalking();
    }
}
