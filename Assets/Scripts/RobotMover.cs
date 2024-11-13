using System.Collections;
using UnityEngine;

public class RobotMover : MonoBehaviour
{
    [SerializeField, Min(0)] private float _moveSpeed = 5;
    [SerializeField, Min(0)] private float _rotationSpeed = 90;
    [SerializeField] private RobotAnimator _animator;

    private IColliderOwner _targetColliderOwner;

    private void OnTriggerEnter(Collider other)
    {
        if (_targetColliderOwner != null && _targetColliderOwner.Collider == other)
            _targetColliderOwner = null;
    }

    public IEnumerator MoveToObject(IColliderOwner targetObject)
    {
        _targetColliderOwner = targetObject;
        _animator.StartWalking();

        while (_targetColliderOwner != null)
        {
            Vector3 currentPosition = transform.position;
            Vector3 currentDirection = transform.forward;
            Vector3 targetPosition = _targetColliderOwner.Collider.transform.position;
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

            yield return null;
        }

        _animator.StopWalking();
    }
}
