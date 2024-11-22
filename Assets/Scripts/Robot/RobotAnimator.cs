using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RobotAnimator : MonoBehaviour
{
    private Animator _animator;

    public event Action ObjectGrabbed;
    public event Action ObjectPicked;
    public event Action ObjectReleased;
    public event Action ObjectPlaced;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void StartWalking()
    {
        _animator.SetBool(AnimatorParams.IsWalking, true);
    }

    public void StopWalking()
    {
        _animator.SetBool(AnimatorParams.IsWalking, false);
    }

    public void PickUpObject()
    {
        _animator.SetTrigger(AnimatorParams.PickUpObject);
    }

    public void PlaceDownObject()
    {
        _animator.SetTrigger(AnimatorParams.PlaceDownObject);
    }

    public void HandleObjectGrabbed()
    {
        ObjectGrabbed?.Invoke();
    }

    public void HandleObjectPicked()
    {
        ObjectPicked?.Invoke();
    }

    public void HandleObjectReleased()
    {
        ObjectReleased?.Invoke();
    }

    public void HandleObjectPlaced()
    {
        ObjectPlaced?.Invoke();
    }
}
